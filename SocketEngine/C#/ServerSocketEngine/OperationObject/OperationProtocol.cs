using ServerEngine.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerEngine.OperationObject
{
    public abstract class OperationProtocol
    {
        protected byte mainCMD;
        protected byte subCMD;
        public abstract int ProtocolId();
        public abstract OperationProtocol Clone();

        protected void Operation(SocketUser user)
        {
            SocketServer.BeginOperation(user, this);
        }

        public abstract void Decode(List<byte> tempList, SocketUser user);

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
            return mainCMD << 8 | subCMD;
        }
    }
}
