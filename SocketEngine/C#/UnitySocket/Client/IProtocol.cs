namespace UnitySocket.Client
{
    public interface IProtocol
    {
        System.Func<int,OperationProtocol> GetProtocol;
        /// <summary>
        /// 接收字节
        /// </summary>
        /// <param name="b">字节数组</param>
        /// <param name="offset">偏移</param>
        /// <param name="count">长度</param>
        void AddByte(byte[] b, int offset, int count);
    }
}
