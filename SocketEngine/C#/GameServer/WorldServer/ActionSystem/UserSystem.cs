using NetEntityHWQ;
using ServerEngine.OperationObject;
using ServerEngine.ServerSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldServer.ActionSystem
{
    class UserSystem : BaseSystem
    {
        [SystemCMDAttr(1, 1)]
        private void FindRole(OperationData od)
        {
            RoleResult rr = od.NetObject<RoleResult>();
            rr.code = od.User.GetIPCode();
            WorldServerMain.Insantce.GetDataCenter().SendData(1, 3, rr);
        }

        [SystemCMDAttr(1, 2)]
        private void CreateRole(OperationData od)
        {
            CreateRoleData crd = od.NetObject<CreateRoleData>();
            crd.code = od.User.GetIPCode();
            WorldServerMain.Insantce.GetDataCenter().SendData(1, 4, crd);
        }

        [SystemCMDAttr(1, 3, "进入游戏")]
        private void EnterGame(OperationData od)
        {
            EnterResult er = new EnterResult();
            er.mapIp = "119.123.182.235";
            er.port = 8790;
            od.User.SendData(1, 3, er);
        }
        
    }
}
