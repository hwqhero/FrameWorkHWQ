using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServerEngine.Core;
using org.critterai.nav;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using org.critterai;
using MapServerEngine.Cell;
using System.Reflection;
namespace MapServerEngine
{
    public class MapServerEngine
    {
        private SocketServer server;
        public PathCorridor pathcorridor;
        NavmeshQuery nq;
        CrowdManager crowd;
        public void Start()
        {

            server = SocketServer.CreateServer();
            server.connectUser += ConnectUser;
            server.Start("192.168.0.254", 8790);


            //Navmesh nm;

            //FileStream fs = null;
            //BinaryFormatter formatter = new BinaryFormatter();
            //fs = new FileStream("CAIBakedNavmesh.navmesh", FileMode.Open);
            //System.Object obj = formatter.Deserialize(fs);
            //Console.WriteLine(Navmesh.Create((byte[])obj, out nm));


            //NavmeshQuery.Create(nm, 2048, out nq);
            //crowd = CrowdManager.Create(2000, 0.4f, nm);
            //pathcorridor = new PathCorridor(2048, 2000, nq, crowd.QueryFilter);
            //NavmeshPoint start;
            //nq.GetNearestPoint(new Vector3(-2.94f, 0, -0.02f), new Vector3(1, 1, 1), crowd.QueryFilter, out start);
            //pathcorridor.Reset(start);
            //ChangeMove(new Vector3(3.84f, 0, 0.28f));

            //Console.WriteLine(pathcorridor.Corners.verts.Length);
            //foreach (Vector3 v in pathcorridor.Corners.verts)
            //{
            //    Console.WriteLine(string.Format("[{0:F7}, {1:F7}, {2:F7}]"
            //    , v.x, v.y, v.z));
            //}

        }


        private NavStatus ChangeMove(Vector3 position)
        {

            NavmeshPoint result;
            NavStatus ns = nq.GetNearestPoint(position, new Vector3(1, 1, 1), crowd.QueryFilter, out result);
            if (ns == NavStatus.Sucess)
            {
                pathcorridor.MoveTarget(result.point);
            }
            return ns;
        }

        public void ConnectUser(SocketUser su)
        {
            su.Write("cell", new CellNode());
        }



    }
}
