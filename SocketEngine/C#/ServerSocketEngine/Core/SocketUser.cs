using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.IO;
using System.Collections;
using ServerEngine.Tool;
using System.Net;
using ServerEngine.OperationObject;

namespace ServerEngine.Core
{
    public sealed class SocketUser
    {
        public int id;
        private SocketAsyncEventArgs m_receiveEventArgs;
        private DateTime m_ConnectDataTime;
        private DateTime m_LastReceive;
        private byte[] m_asyncReceiveBuffer;
        private Socket socket;
        private Action<SocketUser> removeCall;
        private IProtocol protocolData;
        private Dictionary<string, object> blackboard = new Dictionary<string, object>();
        private IPEndPoint remotePoint;
        private int ipHashCode;
        public SocketUser(int id, SocketAsyncEventArgs acceptEventArgs, Action<SocketUser> removeCall, EventHandler<SocketAsyncEventArgs> c,IProtocol p)
        {

            this.id = id;
            this.protocolData = p;
            this.removeCall = removeCall;
            m_receiveEventArgs = new SocketAsyncEventArgs();
            socket = acceptEventArgs.AcceptSocket;
            remotePoint = acceptEventArgs.AcceptSocket.RemoteEndPoint as IPEndPoint;
            ipHashCode = remotePoint.GetHashCode();
            m_receiveEventArgs.UserToken = this;
            m_receiveEventArgs.AcceptSocket = socket;//任务
            m_receiveEventArgs.Completed += c;
            m_ConnectDataTime = DateTime.Now;
            m_asyncReceiveBuffer = new byte[8192];
            m_receiveEventArgs.SetBuffer(m_asyncReceiveBuffer, 0, m_asyncReceiveBuffer.Length);
            Recevie();
        }

        public IPEndPoint GetPoint()
        {
            return remotePoint;
        }

        /// <summary>
        /// 获取ip和端口唯一标识
        /// </summary>
        /// <returns></returns>
        public int GetIPCode()
        {
            return ipHashCode;
        }

        /// <summary>
        /// 写入到黑板
        /// </summary>
        /// <param name="key"></param>
        /// <param name="obj"></param>
        public void Write(string key, object obj)
        {
            if (!blackboard.ContainsKey(key))
            {
                blackboard.Add(key, obj);
            }
            else
            {
                blackboard[key] = obj;
            }
        }


        public void Read<T>(string key,System.Action<T> callBack)
        {
            T t = default(T);
            if (blackboard.ContainsKey(key))
            {
                if(blackboard[key] is T)
                {
                    t = (T)blackboard[key];
                }
            }
            if (callBack != null)
            {
                if (t != null)
                {
                    callBack(t);
                }
            }
        }

        public T Read<T>(string key)
        {
            T t = default(T);
            if (blackboard.ContainsKey(key))
            {
                if (blackboard[key] is T)
                {
                    t = (T)blackboard[key];
                }
            }
            return t;
        }

        private void Recevie()
        {
            try
            {
                socket.ReceiveAsync(m_receiveEventArgs);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        /// <summary>
        /// 接收函数
        /// </summary>
        /// <param name="receiveEventArgs"></param>
        internal void Receive(SocketAsyncEventArgs receiveEventArgs)
        {
            if (socket == null)
                return;
            m_LastReceive = DateTime.Now;
            if (receiveEventArgs.BytesTransferred > 0 && receiveEventArgs.SocketError == SocketError.Success)
            {
                protocolData.AddByte(receiveEventArgs.Buffer, receiveEventArgs.Offset, receiveEventArgs.BytesTransferred);
                if (protocolData.CheckData())
                {
                    SocketServer.BeginOperation(this, protocolData);
                }
                Recevie();
            }
            else
            {
                removeCall(this);
            }
        }

        public void Close()
        {
            removeCall(this);
        }

        internal void Clear()
        {
            if (socket != null)
            {
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
            }
               
        }

        ///// <summary>
        ///// 发送单个对象
        ///// </summary>
        ///// <param name="mainCMD"></param>
        ///// <param name="subCMD"></param>
        ///// <param name="bdHWQ"></param>
        ///// <returns>错误码</returns>
        //public SocketError SendData(byte mainCMD, byte subCMD, BaseNetHWQ bdHWQ)
        //{
        //    return SendData(SocketDateTool.WriteObject(mainCMD, subCMD, bdHWQ));
        //}

        ///// <summary>
        ///// 发送列表
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="mainCMD"></param>
        ///// <param name="subCMD"></param>
        ///// <param name="list"></param>
        ///// <returns>错误码</returns>
        //public SocketError SendData<T>(byte mainCMD, byte subCMD, List<T> list) where T:BaseNetHWQ
        //{
        //    return SendData(SocketDateTool.WriteList<T>(mainCMD, subCMD, list));
        //}

        ///// <summary>
        ///// 发送字符串集合
        ///// </summary>
        ///// <param name="mainCMD"></param>
        ///// <param name="subCMD"></param>
        ///// <param name="strList"></param>
        ///// <returns>错误码</returns>
        //public SocketError SendData(byte mainCMD, byte subCMD, params string[] strList)
        //{
        //    return SendData(SocketDateTool.WriteStringList(mainCMD, subCMD, strList));
        //}

        //public SocketError SendErrorCode(byte mainCMD, byte subCMD, short errorCode)
        //{
        //    return SendData(SocketDateTool.WriteErrorCode(mainCMD, subCMD, errorCode));
        //}

        ///// <summary>
        ///// 发送数字集合
        ///// </summary>
        ///// <param name="mainCMD"></param>
        ///// <param name="subCMD"></param>
        ///// <param name="intList"></param>
        ///// <returns>错误码</returns>
        //public SocketError SendData(byte mainCMD, byte subCMD, params int[] intList)
        //{
        //    return SendData(SocketDateTool.WriteIntList(mainCMD, subCMD, intList));
        //}

        public SocketError SendData(byte[] dataList)
        {
            SocketError se = SocketError.Success;
            socket.Send(dataList, 0, dataList.Length, SocketFlags.None, out se);
            return se;
        }

    }
}
