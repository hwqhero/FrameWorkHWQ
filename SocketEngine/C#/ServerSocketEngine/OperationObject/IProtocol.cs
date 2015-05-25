namespace ServerEngine.OperationObject
{
    public interface IProtocol
    {
        /// <summary>
        /// 接收字节
        /// </summary>
        /// <param name="b">字节数组</param>
        /// <param name="offset">偏移</param>
        /// <param name="count">长度</param>
        void AddByte(byte[] b, int offset, int count);
        /// <summary>
        /// 验证包
        /// </summary>
        /// <returns></returns>
        bool CheckData();
        /// <summary>
        /// 获得对象
        /// </summary>
        /// <returns></returns>
        object GetObject();
        byte GetMainCMD();
        byte GetSubCMD();
    }
}
