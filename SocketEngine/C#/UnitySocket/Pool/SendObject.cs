using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Text;
using UnitySocket.Client;

namespace UnitySocket.Pool
{
    public class SendObject
    {
        public event Func<SendObject, byte[]> ContoByte;
        public SendObject Change(byte main, byte sub, object sendObj)
        {
            this.main = main;
            this.sub = sub;
            this.sendObj = sendObj;
            return this;
        }

        public byte main;
        public byte sub;
        public object sendObj;

        public SocketError SendToServer(Socket server)
        {
            Stopwatch s = Stopwatch.StartNew();
            byte[] sendData = null;
            if (ContoByte != null)
            {
                sendData = ContoByte(this);
            }
            s.Stop();
            UnityClient.Log("序列化时间--->" + s.Elapsed.TotalMilliseconds + "  ms");
            SocketError se = SocketError.Success;
            server.Send(sendData, 0, sendData.Length, SocketFlags.None, out se);
            return se;
        }

        //public void SendToServer(TcpClient server)
        //{
        //    Stopwatch s = Stopwatch.StartNew();
        //    byte[] sendData = SocketDateTool.WriteObject(main, sub, sendObj);
        //    s.Stop();
        //    server.GetStream().Write(sendData, 0, sendData.Length);
        //    UnityClient.Log("序列化时间--->" + s.Elapsed.TotalMilliseconds + "  ms");
        //}
    }
}