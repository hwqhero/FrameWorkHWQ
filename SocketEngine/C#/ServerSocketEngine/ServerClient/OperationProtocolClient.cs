using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerEngine.ServerClient
{
    public abstract class OperationProtocolClient
    {
        protected byte mainCMD;
        protected byte subCMD;
        public abstract int ProtocolId();
        public abstract OperationProtocolClient Clone();

        public abstract void Decode(List<byte> tempList,ServerClient sc);
        protected void Operation(ServerClient sc)
        {
            if (sc != null)
            {
                sc.OperationCMD(this);
            }
        }

        protected virtual byte MainCMD()
        {
            return mainCMD;
        }
        protected virtual byte SubCMD()
        {
            return subCMD;
        }
        internal int GetCMD()
        {
            return mainCMD << 8 | subCMD();
        }
    }
}
