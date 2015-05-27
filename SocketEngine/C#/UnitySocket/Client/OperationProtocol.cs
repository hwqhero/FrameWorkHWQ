using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnitySocket.Client
{
    public abstract class OperationProtocol
    {
        private UnityClient uc;
        public abstract int ProtocolId();
        public abstract OperationProtocol Clone();
        protected void Operation()
        {
            if (uc != null)
                uc.AddMessage(this);
        }
        protected abstract byte MainCMD();
        protected abstract byte SubCMD();
        internal int GetCMD()
        {
            return MainCMD() << 8 | SubCMD();
        }
        internal void SetUnityClient(UnityClient client)
        {
            uc = client;
        }
    }
}
