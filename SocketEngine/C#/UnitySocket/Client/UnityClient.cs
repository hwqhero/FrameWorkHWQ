using NetEntityHWQ;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using UnitySocket.OperationObject;
using UnitySocket.Pool;

namespace UnitySocket.Client
{
    public sealed class UnityClient : MonoBehaviour
    {
        private static bool customDebug = false;
        private Dictionary<byte, Dictionary<byte, OperationEventObject>> operationDic = new Dictionary<byte, Dictionary<byte, OperationEventObject>>();
        private List<byte> m_receiveByteList = new List<byte>();
        private byte[] m_asyncReceiveBuffer;
        private ProtocolData protocolData;
        private SocketAsyncEventArgs m_receiveEventArgs;
        private Queue<ProtocolData> messageList = new Queue<ProtocolData>();
        private System.Action<object> connectEvent;
        private System.Action<object> disEvent;
        public event Action<SocketAsyncEventArgs> receiveEvent;
        private System.Action<object> connectFinishEvent;
        private string ip;
        private int port;
        private Thread sendThread;
        /// <summary>
        /// 未发送列表
        /// </summary>
        private List<SendObject> notSendList;
        /// <summary>
        /// 已发送列表
        /// </summary>
        private List<SendObject> sendList;
        private SendPool sp;

         /// <summary>
        /// 操作列表
        /// </summary>

        protected Socket socket;

        /// <summary>
        /// 绑定协议回调
        /// </summary>
        /// <param name="main"></param>
        /// <param name="sub"></param>
        /// <param name="oEvent"></param>
        public void BindEvent(byte main, byte sub, Action<OperationData> oEvent)
        {
            if (operationDic.ContainsKey(main))
            {
                if (!operationDic[main].ContainsKey(sub))
                {
                    operationDic[main].Add(sub, new OperationEventObject(oEvent));
                }
            }
            else
            {
                operationDic.Add(main, new Dictionary<byte, OperationEventObject>());
                operationDic[main].Add(sub, new OperationEventObject(oEvent));
            }
        }

        private void BeginReceive()
        {
            m_receiveEventArgs = new SocketAsyncEventArgs();
            m_receiveEventArgs.AcceptSocket = socket;
            m_receiveEventArgs.Completed += ReceiveFinish;
            m_asyncReceiveBuffer = new byte[256];
            m_receiveEventArgs.SetBuffer(m_asyncReceiveBuffer, 0, m_asyncReceiveBuffer.Length);
            Receive();

        }

        private void ReceiveFinish(object sender,SocketAsyncEventArgs e)
        {
            if (receiveEvent != null)
            {
                receiveEvent(e);
            }
            switch (e.SocketError)
            {
                case SocketError.ConnectionReset:
                    Close();
                    if (disEvent != null)
                        disEvent(e);
                    break;
                case SocketError.ConnectionAborted:
                    if (disEvent != null)
                        disEvent(e);
                    break;
            }
            switch (e.LastOperation)
            {
                case SocketAsyncOperation.Receive:
                    Receive(e);
                    break;
                    
            }
        }

        private void Receive()
        {
            try {
                socket.ReceiveAsync(m_receiveEventArgs);
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.Log(ex.Message);
            }
            
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
                        System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();
                        protocolData.Decode();
                        sw.Stop();
                        Log("解码时间---->" + sw.Elapsed.TotalMilliseconds + "ms");
                        messageList.Enqueue(protocolData);
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
            else
            {
                Log(receiveEventArgs.BytesTransferred + "<-->" + receiveEventArgs.SocketError);
                Close();
            }

        }

        private void AddConnectEvent(System.Action<object> e)
        {
            connectEvent = e;
        }

        private void Update()
        {
            if (connectEvent != null)
            {
                connectEvent(null);
                connectEvent = null;
            }


            while (messageList.Count > 0)
            {
                StartOperation(messageList.Dequeue());
            }
        }

        public void Close()
        {
            if (socket != null)
            {
                socket.Close();
            }
        }

    

        private void StartOperation(ProtocolData pd)
        {
            if (operationDic.ContainsKey(pd.mainCmd))
            {
                if (operationDic[pd.mainCmd].ContainsKey(pd.subCmd))
                {
                    operationDic[pd.mainCmd][pd.subCmd].Operation(OperationData.CreateByProtocol(pd, null));
                }
            }
        }
        private UnityClient()
        {

        }

        private void Awake()
        {
            notSendList = new List<SendObject>();
            sendList = new List<SendObject>();
            sp = SendPool.Create();
            sendThread = new Thread(SendDataToServer);
        }

        private void SendDataToServer()
        {
       
            while (true)
            {
                try
                {
                    if (notSendList.Count > 0)
                    {
                        SendObject so = notSendList[0];
                        notSendList.RemoveAt(0);
                        SocketError se = so.SendToServer(socket);
                        Log(notSendList.Count + "----" + se);
                        sp.Recovery(so);
                    }
                    if (notSendList.Count == 0)
                    {
                        Thread.Sleep(10);
                    }
                }
                catch (Exception ex)
                {
                    Debug.Log(ex.Message);
                }
                finally
                {
                   
                }
           
            }
        }

        private object BindObject(ProtocolData pd)
        {
            bool temp = false;
            for (int i = 0; i < sendList.Count; i++)
            {
                object obj = sendList[i].GetObject(pd.mainCmd, pd.subCmd, out temp);
                if (temp)
                {
                    return obj;
                }
            }
            return null;
        }

        private void ConnectCompleted(object obj, SocketAsyncEventArgs e)
        {
            if (e.SocketError == SocketError.Success)
            {
                if (connectFinishEvent != null)
                {
                    AddConnectEvent(o =>
                    {

                        connectFinishEvent(e);
                    });
                    if (!sendThread.IsAlive)
                    sendThread.Start();
                    BeginReceive();
                }
            }
     
        }


        public void BindConnectEvnet(System.Action<object> connectFinishEvent)
        {
            this.connectFinishEvent = connectFinishEvent;
        }

        public void Connect(string ip, int port)
        {
            this.ip = ip;
            this.port = port;
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress[] iphe = Dns.GetHostAddresses(ip);
            SocketAsyncEventArgs saea = new SocketAsyncEventArgs();
            saea.RemoteEndPoint = new IPEndPoint(iphe[0], port);
            saea.Completed += ConnectCompleted;
            socket.ConnectAsync(saea);
        }

        /// <summary>
        /// 发送单个对象
        /// </summary>
        /// <param name="mainCMD"></param>
        /// <param name="subCMD"></param>
        /// <param name="bdHWQ"></param>
        /// <returns>错误码</returns>
        public void SendData(byte mainCMD, byte subCMD, BaseNetHWQ bdHWQ, object objList = null)
        {
            notSendList.Add(sp.Get().Change(mainCMD, subCMD, objList, bdHWQ));
        }

        /// <summary>
        /// 发送列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="mainCMD"></param>
        /// <param name="subCMD"></param>
        /// <param name="list"></param>
        /// <returns>错误码</returns>
        public void SendData<T>(byte mainCMD, byte subCMD, List<T> list, object objList = null) where T : BaseNetHWQ
        {
            notSendList.Add(sp.Get().Change(mainCMD, subCMD, objList, list));
        }

        /// <summary>
        /// 发送字符串集合
        /// </summary>
        /// <param name="mainCMD"></param>
        /// <param name="subCMD"></param>
        /// <param name="strList"></param>
        /// <returns>错误码</returns>
        public void SendData(byte mainCMD, byte subCMD, params string[] strList)
        {
            notSendList.Add(sp.Get().Change(mainCMD, subCMD, null, strList));
        }

        /// <summary>
        /// 发送数字集合
        /// </summary>
        /// <param name="mainCMD"></param>
        /// <param name="subCMD"></param>
        /// <param name="intList"></param>
        /// <returns>错误码</returns>
        public void SendData(byte mainCMD, byte subCMD, params int[] intList)
        {
            notSendList.Add(sp.Get().Change(mainCMD, subCMD, null, intList));
        }

        public void Dis()
        {
            OnDestroy();
        }

        private void OnDestroy()
        {
            if (socket != null)
                socket.Close();
            if (sendThread != null)
            {
                sendThread.Abort();
            }
        }

        public void EnableDebug(bool debug)
        {
            customDebug = debug;
        }

        internal static void Log(string message)
        {
            if (customDebug)
            {
                UnityEngine.Debug.Log(message);
            }
        }

        public void Delete()
        {
            DestroyImmediate(gameObject, true);
        }

        public static UnityClient Create()
        {
            GameObject go = new GameObject(string.Empty);
            DontDestroyOnLoad(go);
            return go.AddComponent<UnityClient>();   
        }
    }
}
