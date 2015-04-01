﻿using NetEntityHWQ;
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
         public SendObject Change(byte main, byte sub, object obj, object sendObj)
         {
             this.main = main;
             this.sub = sub;
             this.obj = obj;
             this.sendObj = sendObj;
             return this;
         }

         private byte main;
         private byte sub;
         private object obj;
         private object sendObj;
         public object GetObject(byte main, byte sub,out bool check)
         {
             check = false;
             if (this.main == main && this.sub == sub)
             {
                 check = true;
                 return obj;
             }
             return null;
         }

         public SocketError SendToServer(Socket server)
         {
             Stopwatch s = Stopwatch.StartNew();
             byte[] sendData = SocketDateTool.WriteObject(main, sub, sendObj);
             s.Stop();
             UnityClient.Log("序列化时间--->" + s.Elapsed.TotalMilliseconds + "  ms");
             SocketError se = SocketError.Success;
             server.Send(sendData, 0, sendData.Length, SocketFlags.None, out se);
             return se;
         }

         public void SendToServer(TcpClient server)
         {
             Stopwatch s = Stopwatch.StartNew();
             byte[] sendData = SocketDateTool.WriteObject(main, sub, sendObj);
             s.Stop();
             server.GetStream().Write(sendData, 0, sendData.Length);
             UnityClient.Log("序列化时间--->" + s.Elapsed.TotalMilliseconds + "  ms");
         }
    }
}