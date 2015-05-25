using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using ServerEngine.Core;
using ServerEngine.OperationObject;
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
        private IProtocol protocolData;
        private byte[] m_asyncReceiveBuffer;
        Dictionary<byte, Dictionary<byte, Action<IProtocol>>> operationDic = new Dictionary<byte, Dictionary<byte, Action<IProtocol>>>();
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

        public void BindProtocol(IProtocol p)
        {
            protocolData = p;
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
                                operationDic[sa.m].Add(sa.s, (Action<IProtocol>)mi.CreateDelegate(typeof(Action<IProtocol>), obj));
                            }
                        }
                        else
                        {
                            operationDic.Add(sa.m, new Dictionary<byte, Action<IProtocol>>());
                            operationDic[sa.m].Add(sa.s, (Action<IProtocol>)mi.CreateDelegate(typeof(Action<IProtocol>), obj));
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
                protocolData.AddByte(receiveEventArgs.Buffer, receiveEventArgs.Offset, receiveEventArgs.BytesTransferred, obj => {

                    OperationCMD(protocolData);
                });
                Receive();
            } 
     
        }

        private void OperationCMD(IProtocol pd)
        {
            if (operationDic.ContainsKey(pd.GetMainCMD()))
            {
                if (operationDic[pd.GetMainCMD()].ContainsKey(pd.GetSubCMD()))
                {
                    operationDic[pd.GetMainCMD()][pd.GetSubCMD()](pd);
                    return;
                }
            }
        }


        public SocketError SendData(byte[] dataList)
        {
            SocketError se = SocketError.Success;
            client.Send(dataList, 0, dataList.Length, SocketFlags.None, out se);
            return se;
        }
    }
}
