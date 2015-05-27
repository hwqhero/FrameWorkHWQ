#define  Unity
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
#if Unity
using UnityEngine;
#endif
using UnitySocket.Pool;

namespace UnitySocket.Client
{
#if Unity
    public sealed class UnityClient : MonoBehaviour
#else
    public sealed class UnityClient
#endif
    {
        private static bool customDebug = false;
        private Dictionary<int, OperationEventObject> operationDic = new Dictionary<int, OperationEventObject>();
        private Dictionary<int, OperationProtocol> operationProtocolDic = new Dictionary<int, OperationProtocol>();
        private List<byte> m_receiveByteList = new List<byte>();
        private byte[] m_asyncReceiveBuffer;
        private IProtocol protocolData;
        private SocketAsyncEventArgs m_receiveEventArgs;
        private Queue<OperationProtocol> messageList = new Queue<OperationProtocol>();
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
        private SendPool sp;
        private Socket socket;

        /// <summary>
        /// 绑定协议回调
        /// </summary>
        /// <param name="main">主协议</param>
        /// <param name="sub">子协议</param>
        /// <param name="callBack">回调函数</param>
        /// <param name="isUnityMainThread">是否在unity线程执行回调   默认是</param>
        public void BindEvent(byte main, byte sub, Action<OperationProtocol> callBack, bool isUnityMainThread = true)
        {
            int cmd = main << 8 | sub;
            operationDic[cmd] = new OperationEventObject(callBack) { IsUnityMainThread = isUnityMainThread };
        }

        public void CreateOperation<T>() where T : OperationProtocol,new()
        {
            T t = new T();
            t.SetUnityClient(this);
            operationProtocolDic[t.ProtocolId()] = t;
        }

        public void SetProto(IProtocol p)
        {
            protocolData = p;
            protocolData.GetProtocol = GetProtocol;
        }

        private OperationProtocol GetProtocol(int id)
        {
            return operationProtocolDic[id];
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
                if (protocolData != null)
                    protocolData.AddByte(receiveEventArgs.Buffer, receiveEventArgs.Offset, receiveEventArgs.BytesTransferred);
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

        internal void AddMessage(OperationProtocol op)
        {
            int cmd = op.GetCMD();
            if (operationDic.ContainsKey(cmd))
            {
                if (operationDic[cmd].IsUnityMainThread)
                {
                    messageList.Enqueue(op);
                }
                else
                {
                    StartOperation(op);
                }
            }
        }

        private void StartOperation(OperationProtocol op)
        {
            int cmd = op.GetCMD();
            if (operationDic.ContainsKey(cmd))
            {
                operationDic[cmd].Operation(op);
            }
        }
        private UnityClient()
        {

        }

        private void Awake()
        {
            notSendList = new List<SendObject>();
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
                    Log(ex.Message);
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
            #if Unity
            if (customDebug)
            {
                UnityEngine.Debug.Log(message);
            }
            #endif
        }

        public void Delete()
        {
            #if Unity
            DestroyImmediate(gameObject, true);
            #endif
        }

        public static UnityClient Create()
        {
            #if Unity
            GameObject go = new GameObject(string.Empty);
            DontDestroyOnLoad(go);
            return go.AddComponent<UnityClient>();   
            #else
            return new UnityClient();
            #endif
        }
    }
}
