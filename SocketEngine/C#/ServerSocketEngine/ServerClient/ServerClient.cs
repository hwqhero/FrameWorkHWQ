using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using ServerEngine.Core;
using ServerEngine.OperationObject;
using NetEntityHWQ;
using System.Reflection;

namespace ServerEngine.ServerClient
{
    /// <summary>
    /// 服务器之间通讯
    /// </summary>
    public class ServerClient
    {
        private Socket client;
        private SocketAsyncEventArgs m_receiveEventArgs;
        public event Action connectFinish;
        public event Action disconnectEvent;
        private List<byte> m_receiveByteList = new List<byte>();
        private ProtocolData protocolData;
        private byte[] m_asyncReceiveBuffer;
        Dictionary<byte, Dictionary<byte, Action<OperationData>>> operationDic = new Dictionary<byte, Dictionary<byte, Action<OperationData>>>();
        public void Connect(string ip,int port)
        {
            client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                IPAddress[] iphe = Dns.GetHostAddresses(ip);
                client.Connect(iphe[0], port);
                if (connectFinish != null)
                {
                    connectFinish();
                }
                m_receiveEventArgs = new SocketAsyncEventArgs();
                m_receiveEventArgs.AcceptSocket = client;
                m_receiveEventArgs.Completed += ReceiveFinish;
                m_asyncReceiveBuffer = new byte[8192];
                m_receiveEventArgs.SetBuffer(m_asyncReceiveBuffer, 0, m_asyncReceiveBuffer.Length);
                Receive();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public static ServerClient Create()
        {
            return new ServerClient();
        }


        public void BindEventByCMD(params object[] objList)
        {
            foreach (object obj in objList)
            {
                foreach (MethodInfo mi in obj.GetType().GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly | BindingFlags.Public))
                {
                    ClientCMD sa = mi.GetCustomAttribute(typeof(ClientCMD)) as ClientCMD;
                    if (sa != null)
                    {
                        if (operationDic.ContainsKey(sa.m))
                        {
                            if (!operationDic[sa.m].ContainsKey(sa.s))
                            {
                                operationDic[sa.m].Add(sa.s, (Action<OperationData>)mi.CreateDelegate(typeof(Action<OperationData>), obj));
                            }
                        }
                        else
                        {
                            operationDic.Add(sa.m, new Dictionary<byte, Action<OperationData>>());
                            operationDic[sa.m].Add(sa.s, (Action<OperationData>)mi.CreateDelegate(typeof(Action<OperationData>), obj));
                        }
                    }
                }
            }
        }


        private void ReceiveFinish(object sender, SocketAsyncEventArgs e)
        {
            switch (e.LastOperation)
            {
                case SocketAsyncOperation.Receive:
                    Receive(e);
                    break;
            }
        }

        private void Receive()
        {
            client.ReceiveAsync(m_receiveEventArgs);
        }

        private void Receive(SocketAsyncEventArgs receiveEventArgs)
        {
            if (receiveEventArgs.BytesTransferred > 0 && receiveEventArgs.SocketError == SocketError.Success)
            {
                for (int i = receiveEventArgs.Offset; i < receiveEventArgs.BytesTransferred; i++)
                {
                    m_receiveByteList.Add(receiveEventArgs.Buffer[i]);
                }
                while (m_receiveByteList.Count >= ProtocolData.headCount)
                {
                    if (protocolData == null)
                    {
                        protocolData = new ProtocolData(m_receiveByteList.GetRange(0, ProtocolData.headCount));
                    }
                    if (m_receiveByteList.Count >= protocolData.length + ProtocolData.headCount)
                    {
                        protocolData.dataList = m_receiveByteList.GetRange(ProtocolData.headCount, protocolData.length).ToArray();
                        protocolData.Decode();
                        OperationCMD(protocolData);
                        m_receiveByteList.RemoveRange(0, protocolData.length + ProtocolData.headCount);
                        protocolData = null;
                    }
                    else
                    {
                        break;
                    }
                }
                Receive();
            } 
     
        }

        private void OperationCMD(ProtocolData pd)
        {
            if (operationDic.ContainsKey(pd.mainCmd))
            {
                if (operationDic[pd.mainCmd].ContainsKey(pd.subCmd))
                {
                    operationDic[pd.mainCmd][pd.subCmd](OperationData.Create(pd, null));
                    return;
                }
            }
        }


        /// <summary>
        /// 发送单个对象
        /// </summary>
        /// <param name="mainCMD"></param>
        /// <param name="subCMD"></param>
        /// <param name="bdHWQ"></param>
        /// <returns>错误码</returns>
        public SocketError SendData(byte mainCMD, byte subCMD, BaseNetHWQ bdHWQ)
        {
            return SendData(SocketDateTool.WriteObject(mainCMD, subCMD, bdHWQ));
        }

        /// <summary>
        /// 发送列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="mainCMD"></param>
        /// <param name="subCMD"></param>
        /// <param name="list"></param>
        /// <returns>错误码</returns>
        public SocketError SendData<T>(byte mainCMD, byte subCMD, List<T> list) where T : BaseNetHWQ
        {
            return SendData(SocketDateTool.WriteList<T>(mainCMD, subCMD, list));
        }

        /// <summary>
        /// 发送字符串集合
        /// </summary>
        /// <param name="mainCMD"></param>
        /// <param name="subCMD"></param>
        /// <param name="strList"></param>
        /// <returns>错误码</returns>
        public SocketError SendData(byte mainCMD, byte subCMD, params string[] strList)
        {
            return SendData(SocketDateTool.WriteStringList(mainCMD, subCMD, strList));
        }

        /// <summary>
        /// 发送数字集合
        /// </summary>
        /// <param name="mainCMD"></param>
        /// <param name="subCMD"></param>
        /// <param name="intList"></param>
        /// <returns>错误码</returns>
        public SocketError SendData(byte mainCMD, byte subCMD, params int[] intList)
        {
            return SendData(SocketDateTool.WriteIntList(mainCMD, subCMD, intList));
        }

        private SocketError SendData(byte[] dataList)
        {
            SocketError se = SocketError.Success;
            client.Send(dataList, 0, dataList.Length, SocketFlags.None, out se);
            return se;
        }
    }
}
