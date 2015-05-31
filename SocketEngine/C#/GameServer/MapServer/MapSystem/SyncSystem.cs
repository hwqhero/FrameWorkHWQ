using MapServerEngine.Cell;
using NetEntityHWQ;
using ServerEngine.Core;
using ServerEngine.OperationObject;
using ServerEngine.ServerSystem;
using System;
using System.Collections.Generic;

namespace MapServerEngine.MapSystem
{
    class SyncSystem : BaseSystem
    {
        [SystemCMDAttr(1, 1)]
        private void SyncPoint(OperationData od)
        {
            CellNode cn = od.User.Read<CellNode>("cell");
            cn.point = od.NetObject<SyncPoint>();
            Console.WriteLine(cn.point.userId + "<-->" + od.User.GetIPCode());
            foreach (SocketUser su in GetAllUser())
            {
                su.SendData(1, 1, cn.point);
            }

        }

        [SystemCMDAttr(1, 2)]
        private void Online(OperationData od)
        {

            SyncPoint p = od.NetObject<SyncPoint>();
            CellNode cn = od.User.Read<CellNode>("cell");
            cn.point = p;
            foreach (SocketUser su in GetAllUser())
            {
                su.SendData(1, 2, p);
            }
        }

        [SystemCMDAttr(1, 3)]
        private void Endpoint(OperationData od)
        {
            SyncPoint p = od.NetObject<SyncPoint>();
            foreach (SocketUser su in GetAllUser())
            {
                su.SendData(1, 3, p);
            }
        }

        [SystemCMDAttr(1, 4)]
        private void RequestUserList(OperationData od)
        {
           
            foreach (SocketUser su in GetAllUser())
            {
                if (su != od.User)
                {
                    CellNode cn = su.Read<CellNode>("cell");
                    od.User.SendData(1, 4, cn.point);
                } 
            }
        }
    }
}
