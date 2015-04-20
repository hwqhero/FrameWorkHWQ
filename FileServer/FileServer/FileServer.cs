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
    public class FileServer
    {
        public static FileServer Insance;

        public List<MyFile> files = new List<MyFile>();
        public Socket server = new Socket(SocketType.Stream, ProtocolType.Tcp);
        private FileServer()
        {
            Insance = this;
            string[] fileList = Directory.GetFiles("File", "*.*", SearchOption.AllDirectories);
            foreach (string s in fileList)
            {
                MyFile f = new MyFile();
                f.Load(s, Path.GetFileName(s));
                files.Add(f);
            }
            server.Bind(new IPEndPoint(IPAddress.Any, 9090));
            server.Listen(10000);
            SocketAsyncEventArgs asea = new SocketAsyncEventArgs();
            asea.Completed += IO_C;
            server.AcceptAsync(asea);
        }



        private void IO_C(object obj, SocketAsyncEventArgs e)
        {
            new Client().Init(e);
            SocketAsyncEventArgs asea = new SocketAsyncEventArgs();
            asea.Completed += IO_C;
            server.AcceptAsync(asea);
        }


        public static void Create()
        {
            new FileServer();
        }
    }
}
