namespace UnitySocket.Client
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
        public abstract void AddByte(byte[] b, int offset, int count,UnityClient client);

        /// <summary>
        /// 获得协议类型
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        protected OperationProtocol GetProtocol(int id,UnityClient client)
        {
            return client.GetProtocol(id);
        }
    }
}
