using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnitySocket.Client
{
    public abstract class OperationProtocol
    {
        protected byte mainCMD;
        protected byte subCMD;
        public abstract object ProtocolId();
        public abstract OperationProtocol Clone();

        public abstract void Decode(List<byte> tempList, UnityClient uc);
        protected void Operation(UnityClient uc)
        {
            if (uc != null)
                uc.AddMessage(this);
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
            return mainCMD << 8 | subCMD;
        }
    }
}
