using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Net.Sockets;
namespace FileServer
{
    public class Client
    {
        public Socket s;
        public SocketAsyncEventArgs read;
        public List<byte> m_receiveByteList = new List<byte>();

        public void Init(SocketAsyncEventArgs client)
        {
            s = client.AcceptSocket;
            read = new SocketAsyncEventArgs();
            read.AcceptSocket = s;
            read.Completed += IO_C;
            read.SetBuffer(new byte[2048], 0, 2048);
            s.ReceiveAsync(read);
        }


        private void IO_C(object sender, SocketAsyncEventArgs e)
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
                    if (m_receiveByteList.Count >= 8 + legnth)
                    {
                        byte[] a = new byte[legnth + 4];
                        m_receiveByteList.CopyTo(4, a, 0, legnth + 4);
                        BinaryReader br = new BinaryReader(new MemoryStream(a));
                        int cmd = br.ReadInt32();
                        switch (cmd)
                        {
                            case 1:
                                string name = br.ReadString();
                                string pwd = br.ReadString();
                                SendData(1, new byte[1]);
                                break;
                            case 2://检查文件

                                string filePath = br.ReadString();
                                MyFile file = FileServer.Insance.files.Find(obj =>
                                {
                                    return obj.filePath == filePath;
                                });
                                MemoryStream me = new MemoryStream();
                                BinaryWriter bw = new BinaryWriter(me);
                                if (file != null)
                                {
                                    bw.Write(200);
                                }
                                else
                                {
                                    bw.Write(404);
                                }
                                SendData(2, me.ToArray());
                                break;
                            case 3:
                                string filePath1 = br.ReadString();
                                MyFile file1 = FileServer.Insance.files.Find(obj =>
                                {
                                    return obj.filePath == filePath1;
                                });
                                SendData(3, file1.datas);
                                break;
                        }
                    }
                    m_receiveByteList.RemoveRange(0, 8 + legnth);
                }
                s.ReceiveAsync(read);
            }
            else
            {
               
            }
        }

        public void SendData(int cmd, byte[] datas)
        {
            MemoryStream ms = new MemoryStream();
            ms.Write(BitConverter.GetBytes(datas.Length), 0, BitConverter.GetBytes(datas.Length).Length);
            ms.Write(BitConverter.GetBytes(cmd), 0, BitConverter.GetBytes(cmd).Length);
            ms.Write(datas, 0, datas.Length);
            s.Send(ms.ToArray());
        }
    }
}
