using ServerEngine.OperationObject;
using ServerEngine.ServerSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entity;
using NetEntityHWQ;

namespace DataCenter.ActionHWQ
{
    class MapSystem : BaseSystem
    {
        [SystemCMDAttr(2, 1, "查询地图")]
        private void FindMap(OperationData od)
        {
            
            DataCenterMain.Insance.OperationTable<RpgMapT>(list => {

                foreach (RpgMapT rm in list)
                {
                    RpgMapData rmd = new RpgMapData();
                    list.Find(obj => obj.MapId == 1);
                }
            });
        }
    }
}
