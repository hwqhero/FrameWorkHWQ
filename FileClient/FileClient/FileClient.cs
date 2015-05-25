using NetEntityHWQ;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace FileClient
{
    class FileClient
    {
        public string ip = "192.168.18.88";
        public int port = 8889;
        public Socket client = new Socket(SocketType.Stream, ProtocolType.Tcp);
        private List<byte> m_receiveByteList = new List<byte>();
        public static FileClient Insance;
        private SocketAsyncEventArgs read;
        public FileClient()
        {
            Insance = this;
            client.Connect(IPAddress.Parse(ip), port);
            read = new SocketAsyncEventArgs();
            read.AcceptSocket = client;
            read.Completed += IO;
            read.SetBuffer(new byte[2048], 0, 2048);
            client.ReceiveAsync(read);
        }

        public void IO(object sendj,SocketAsyncEventArgs e)
        {
            lock (e)
            {
                switch (e.LastOperation)
                {
                    case SocketAsyncOperation.Receive:
                        Receive(e);
                        break;
                    case SocketAsyncOperation.Send:
                        break;
                }
            }
        }

        internal void Receive(SocketAsyncEventArgs receiveEventArgs)
        {
            if (receiveEventArgs.BytesTransferred > 0 && receiveEventArgs.SocketError == SocketError.Success)
            {
                for (int i = receiveEventArgs.Offset; i < receiveEventArgs.BytesTransferred; i++)
                {
                    m_receiveByteList.Add(receiveEventArgs.Buffer[i]);
                }
                while (m_receiveByteList.Count >= 8)
                {
                    int legnth = BitConverter.ToInt32(m_receiveByteList.ToArray(), 0);
                    int cmd = BitConverter.ToInt32(m_receiveByteList.ToArray(), 4);
                    switch (cmd)
                    {
                        case 1:
                            Check("File\\monster.unity3d");
                            break;
                        case 2:
                            if (m_receiveByteList.Count >= 8 + legnth)
                            {
                                byte[] a = new byte[legnth ];
                                m_receiveByteList.CopyTo(8, a, 0, legnth);
                                BinaryReader br = new BinaryReader(new MemoryStream(a));
                                int d = br.ReadInt32();
                                if (d == 200)
                                    DownLoad("File\\monster.unity3d");
                            }
                            break;

                        case 3:
                            Form1.insance.aaaa(m_receiveByteList.Count - 8, legnth);
                            break;
                    }

                    if (m_receiveByteList.Count >= 8 + legnth)
                    {
                        m_receiveByteList.RemoveRange(0, 8 + legnth);
                    }
                    else
                    {
                        break;
                    }
                   
                }
                client.ReceiveAsync(read);
            }
            else
            {

            }
        }

        public void Login(string name, string pwd)
        {
            List<byte> sendList = new List<byte>();
            byte[] tempList = SocketDateTool.WriteObject(new LoginResult()
            {
            userID = 0,
            worldIP = "asdf",
            wroldProt = 9090
            });
            sendList.AddRange(BitConverter.GetBytes(tempList.Length - 7));
            sendList.Add((byte)1);
            sendList.Add((byte)1);
            sendList.AddRange(tempList);
            client.Send(sendList.ToArray());
        }

        public void Check(string path)
        {
            MemoryStream ms = new MemoryStream();
            MemoryStream ms1 = new MemoryStream();
            BinaryWriter bw = new BinaryWriter(ms);
            bw.Write(path);
            BinaryWriter bw1 = new BinaryWriter(ms1);
            bw1.Write((int)ms.Length);
            bw1.Write(2);
            bw1.Write(ms.ToArray());
            byte[] a = ms1.ToArray();
            client.Send(ms1.ToArray());
        }

        public void DownLoad(string path)
        {
            MemoryStream ms = new MemoryStream();
            MemoryStream ms1 = new MemoryStream();
            BinaryWriter bw = new BinaryWriter(ms);
            bw.Write(path);
            BinaryWriter bw1 = new BinaryWriter(ms1);
            bw1.Write((int)ms.Length);
            bw1.Write(3);
            bw1.Write(ms.ToArray());
            byte[] a = ms1.ToArray();
            client.Send(ms1.ToArray());
        }
    }
}
