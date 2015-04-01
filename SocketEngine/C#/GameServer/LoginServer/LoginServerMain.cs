using LoginServer.ActionSystem;
using NetEntityHWQ;
using ServerEngine.Core;
using ServerEngine.OperationObject;
using ServerEngine.ServerClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LoginServer
{
    class LoginServerMain
    {
        public static LoginServerMain Insance;
        private ServerClient dataCenterClient;
        private SocketServer server;

        private LoginServerMain()
        {

        }

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

        private void dataCenterFinish()
        {
            server = SocketServer.CreateServer();
            server.Start("192.168.0.254", 8788);
            dataCenterClient.BindEventByCMD(this);
        }

        public void QueryUser(QueryUser qu)
        {
            if (string.IsNullOrEmpty(qu.userName) || string.IsNullOrEmpty(qu.pwd))
            {
                return;
            }
            dataCenterClient.SendData(1, 1, qu);
        }

        public void RegisterUser(QueryUser qu)
        {
            if (string.IsNullOrEmpty(qu.userName) || string.IsNullOrEmpty(qu.pwd))
            {
                return;
            }
            dataCenterClient.SendData(1, 2, qu);
        }

        [ClientCMD(1, 2)]
        private void RegisterResult(OperationData od)
        {
            server.GetSystem<LoginSystem>().RegisterResult(od.NetObject<QueryUserResult>());
        }

        [ClientCMD(1,1)]
        private void QueryResult(OperationData od)
        {
            server.GetSystem<LoginSystem>().LoginResult(od.NetObject<QueryUserResult>());
        }

        public static void Create()
        {
            if (Insance == null)
            {
                Insance = new LoginServerMain();
                Insance.Init();
            }
        }
    }
}
