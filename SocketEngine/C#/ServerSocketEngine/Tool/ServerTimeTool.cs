using ServerEngine.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerEngine.Tool
{
    public class ServerTimeTool
    {
        private static ServerTimeTool Instance;

        private ServerTimeTool()
        {

        }

        internal static void Create()
        {
            if (Instance == null)
            {
                Instance = new ServerTimeTool();
            }
        }

       

        /// <summary>
        /// 获取服务器运行时间
        /// </summary>
        /// <returns></returns>
        public static float GetRunTime()
        {
            TimeSpan tempTime = DateTime.Now - SocketServer.startTime;
            return (float)(tempTime.Milliseconds / 1000.0f);
        }
    }
}
