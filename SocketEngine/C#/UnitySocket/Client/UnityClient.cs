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
        private IProtocol protocolData;
        private SocketAsyncEventArgs m_receiveEventArgs;
        private Queue<IProtocol> messageList = new Queue<IProtocol>();
        private System.Action<object> connectEvent;
        private System.Action<object> disEvent;
        private System.Func<IProtocol> createProtocol;
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
        private Socket socket;

        /// <summary>
        /// 绑定协议回调
        /// </summary>
        /// <param name="main"></param>
        /// <param name="sub"></param>
        /// <param name="oEvent"></param>
        public void BindEvent(byte main, byte sub, Action<IProtocol> oEvent)
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

        public void BindProto(System.Func<IProtocol> p)
        {
            createProtocol += p;
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
                protocolData.AddByte(receiveEventArgs.Buffer, receiveEventArgs.Offset, receiveEventArgs.BytesTransferred);
                if (protocolData.CheckData())
                {
                    messageList.Enqueue(protocolData);
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

    

        private void StartOperation(IProtocol pd)
        {
            if (operationDic.ContainsKey(pd.GetMainCMD()))
            {
                if (operationDic[pd.GetMainCMD()].ContainsKey(pd.GetSubCMD()))
                {
                    operationDic[pd.GetMainCMD()][pd.GetSubCMD()].Operation(pd);
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
                    if (createProtocol != null)
                    {
                        protocolData = createProtocol();
                    }
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
