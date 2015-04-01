using ServerEngine.Core;
using ServerEngine.OperationObject;
using ServerEngine.ServerClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetEntityHWQ;

namespace WorldServer
{
    class WorldServerMain
    {
        public static WorldServerMain Insantce
        {
            private set;
            get;
        }
        private ServerClient dataCenterClient;
        private SocketServer server;
        private WorldServerMain() { }

        private void Init() 
        {
            ConnectDataCenter();
        }


        private void ConnectDataCenter()
        {
            dataCenterClient = ServerClient.Create();
            dataCenterClient.connectFinish += dataCenterFinish;
            dataCenterClient.Connect("192.168.0.254", 8787);

        }

        private void dataCenterFinish()
        {
            server = SocketServer.CreateServer();
            server.Start("192.168.0.254", 8789);
            dataCenterClient.BindEventByCMD(this);
        }

        [ClientCMD(1,3,"")]
        public void FindRoleResult(OperationData od)
        {
            RoleResult rr = od.NetObject<RoleResult>();
            server.OperationUserList(list =>
            {
                SocketUser su = list.Find(obj => obj.GetIPCode() == rr.code);
                if (su != null)
                {
                    su.SendData(1, 1, rr);
                }
            });
        }

        [ClientCMD(1, 4,"")]
        public void b(OperationData od)
        {
            RoleResult rr = od.NetObject<RoleResult>();
            server.OperationUserList(list =>
            {

                SocketUser su = list.Find(obj => obj.GetIPCode() == rr.code);
                if (su != null)
                {
                    su.SendData(1, 2, rr);
                }
            });
        }

        public ServerClient GetDataCenter()
        {
            return dataCenterClient;
        }

        public static void Create()
        {
            if(Insantce == null)
            {
                Insantce = new WorldServerMain();
                Insantce.Init();
            }
        }
    }
}
