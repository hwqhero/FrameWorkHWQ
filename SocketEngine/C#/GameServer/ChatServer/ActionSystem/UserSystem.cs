using NetEntityHWQ;
using ServerEngine.Core;
using ServerEngine.OperationObject;
using ServerEngine.ServerSystem;
using System;
using System.Collections.Generic;

namespace ChatServer.ActionSystem
{
    class UserSystem : BaseSystem
    {
        private Dictionary<ulong, SocketUser> socketDic = new Dictionary<ulong, SocketUser>();
        [SystemCMDAttr(1, 1, "世界聊天")]
        private void WorldChat(OperationData od)
        {
            List<string> message = od.AsList<string>();
            if (message != null)
            {
            
                foreach (SocketUser su in GetAllUser())
                {
                    su.SendData(1, 1, message[0], message[1]);
                }
            }

        }
        [SystemCMDAttr(1,2,"存储SocketUser")]
        private void AddtoDic(OperationData od)
        {
            List<string> s=od.AsList<string>();
            ulong userid = Convert.ToUInt64(s[0]);
            Console.WriteLine("s[0]");
            if (!socketDic.ContainsKey(userid))
            {
                socketDic.Add(userid,od.User);
            }
        }
        [SystemCMDAttr(1,3,"私聊")]
        private void ChatToUser(OperationData od)
        {
            UserMessage um = od.NetObject<UserMessage>();
            
            if (!socketDic.ContainsKey(um.uesrId))
            {
                od.User.SendErrorCode(1,1,(short)3);
            }else
            {
                od.User.SendData(1, 1, um.roleName, um.message);
                socketDic[um.uesrId].SendData(1, 1, um.roleName, um.message);
            }
        }
    }
}
