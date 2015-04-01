using ServerEngine.OperationObject;
using ServerEngine.ServerSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapServerEngine.MapSystem
{
    class PingSystem : BaseSystem
    {
        [SystemCMDAttr(25, 25)]
        private void TestPing(OperationData od)
        {
            od.User.SendData(255, 255, 1, 1);
        }
    }
}
