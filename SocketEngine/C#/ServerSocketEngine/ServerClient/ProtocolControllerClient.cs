using ServerEngine.OperationObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerEngine.ServerClient
{
    public abstract class ProtocolControllerClient
    {
        /// <summary>
        /// 接收字节
        /// </summary>
        /// <param name="b">字节数组</param>
        /// <param name="offset">偏移</param>
        /// <param name="count">长度</param>
        public abstract void AddByte(byte[] b, int offset, int count, ServerClient sc);
        protected OperationProtocolClient GetProtocol(int id, ServerClient sc)
        {
            return sc.GetProtocol(id);
        }
    }
}
