using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;
using ServerEngine.Core;

namespace FileServer
{
    public class FileServer
    {
        public static FileServer Insance;
        private SocketServer ss;
        public List<MyFile> files = new List<MyFile>();
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
            ss = SocketServer.CreateServer();
            ss.connectUser += ss_connectUser;
            ss.Start("192.168.18.88", 8889);
            ss.BindProtocol(() => {

                return new ProtocolData();
            });
        }

        void ss_connectUser(SocketUser obj)
        {
            
        }

        public static void Create()
        {
            if (Insance == null)
                new FileServer();
        }
    }
}
