using ServerEngine.Core;
namespace ServerEngine.OperationObject
{
    /// <summary>
    /// 协议控制器
    /// </summary>
    public abstract class ProtocolController
    {
        /// <summary>
        /// 接收字节
        /// </summary>
        /// <param name="b">字节数组</param>
        /// <param name="offset">偏移</param>
        /// <param name="count">长度</param>
        public abstract void AddByte(byte[] b, int offset, int count, SocketUser user);

        /// <summary>
        /// 获得协议类型
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        protected OperationProtocol GetProtocol(int id)
        {
            return null;
        }

        /// <summary>
        /// 复制对象
        /// </summary>
        /// <returns></returns>
        public abstract ProtocolController Clone();
    }
}
