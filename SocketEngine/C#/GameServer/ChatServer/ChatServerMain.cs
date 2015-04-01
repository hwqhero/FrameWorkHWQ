using ServerEngine.Core;
using ServerEngine.ServerClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer
{
    
    class ChatServerMain
    {
        public static ChatServerMain Insance;
        private ServerClient dataCenterClient; 
        private SocketServer server;

        private void Init()
        {

            ConnectDataCenter();
        }

        /// <summary>
        /// 连接数据中心
        /// </summary>
        private void ConnectDataCenter()
        {
            dataCenterClient = ServerClient.Create();
            dataCenterClient.connectFinish += dataCenterFinish;
            dataCenterClient.Connect("192.168.0.254", 8787);

        }
        public void dataCenterFinish()
        {

            server = SocketServer.CreateServer();
            server.connectUser += ConnectUser;
            server.Start("192.168.0.254", 8791);
        }

        private void ConnectUser(SocketUser obj)
        {

        }

        public static void Create()
        {
            if (Insance == null)
            {
                Insance = new ChatServerMain();
                Insance.Init();
            }
        }
     

    }
            

}
