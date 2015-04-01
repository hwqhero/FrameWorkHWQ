using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerEngine.OperationObject
{
    public class SystemCMDAttr : Attribute
    {
        internal byte m;
        internal byte s;
        private void Init(byte main, byte sub)
        {
            m = main;
            s = sub;
        }


        public SystemCMDAttr(byte main, byte sub)
        {
            Init(main, sub);
        }

        public SystemCMDAttr(byte main, byte sub, string annotate)
        {
            Init(main, sub);
        }
    }
}
