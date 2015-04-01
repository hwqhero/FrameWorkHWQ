using ServerEngine.OperationObject;
using ServerEngine.ServerSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetEntityHWQ;
using ServerEngine.Core;
using System.Net;

namespace LoginServer.ActionSystem
{
    class LoginSystem : BaseSystem
    {
        [SystemCMDAttr(1, 1, "查询用户")]
        private void Login(OperationData od)
        {
            List<string> user = od.AsList<string>();
            QueryUser qu = Create(user[0], user[1], od.User.GetIPCode());
            od.User.Write("login", qu);
            LoginServerMain.Insance.QueryUser(qu);
        }



        [SystemCMDAttr(1, 2, "注册用户")]
        private void Register(OperationData od)
        {
            List<string> user = od.AsList<string>();
            LoginServerMain.Insance.RegisterUser(Create(user[0], user[1], od.User.GetIPCode()));
        }

        private QueryUser Create(string name, string pwd, int hashCode)
        {
            QueryUser qu = new QueryUser();
            qu.clientID = hashCode;
            qu.userName = name;
            qu.pwd = pwd;
            return qu;
        }

        public void RegisterResult(QueryUserResult qur)
        {
            SocketUser su = FindUserObject(obj =>
            {
                return qur.clientID == obj.GetIPCode();
            });
            if (su != null)
            {
                if (qur.errorCode != 0)
                {
                    su.SendErrorCode(1, 2, (short)qur.errorCode);
                }
                else
                {
                    su.SendData(1, 2, qur);
                }
                su.Close();
            }

        }

        public void LoginResult(QueryUserResult qur)
        {

            SocketUser su = FindUserObject(obj =>
            {
                return qur.clientID == obj.GetIPCode();
            });
            if (su != null)
            {

                QueryUser qu = su.Read<QueryUser>("login");
                if (qu != null)
                {
                    if (!qu.userName.Equals(qur.name))
                    {
                        su.SendErrorCode(1, 1, (short)1);
                    }
                    else if (!qu.pwd.Equals(qur.pwd))
                    {
                        su.SendErrorCode(1, 1, (short)2);
                    }
                    else
                    {
                        LoginResult lr = new NetEntityHWQ.LoginResult();
                        lr.userID = qur.userID;
                        lr.worldIP = "119.123.182.235";
                        lr.wroldProt = 8789;
                        su.SendData(1, 1, lr);
                    }
                }
                su.Close();
            }

        }
    }
}
