using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerEngine.ServerClient
{
    /// <summary>
    /// 命令处理绑定
    /// </summary>
    public class ClientCMD : Attribute
    {
        internal byte m;
        internal byte s;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="main">主命令</param>
        /// <param name="sub">子命令</param>
        /// <param name="annotate">注释</param>
        public ClientCMD(byte main,byte sub,string annotate)
        {
            m = main;
            s = sub;
        }

        public ClientCMD(byte main, byte sub)
        {
            m = main;
            s = sub;
        }
    }
}
