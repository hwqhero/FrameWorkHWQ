using System.Collections.Generic;
namespace UnitySocket.Client
{
    /// <summary>
    /// 协议控制器
    /// </summary>
    internal sealed class ProtocolController
    {
        private List<byte> receiveList = new List<byte>();
        private OperationProtocol op;
        /// <summary>
        /// 接收字节
        /// </summary>
        /// <param name="b">字节数组</param>
        /// <param name="offset">偏移</param>
        /// <param name="count">长度</param>
        internal void AddByte(byte[] b, int offset, int count, UnityClient client)
        {
            for (int i = offset; i < count; i++)
            {
                receiveList.Add(b[i]);
            }
            while (true)
            {
                if (op == null)
                {
                    if (receiveList.Count >= 4)
                    {
                        int id = System.BitConverter.ToInt32(receiveList.ToArray(), 0);
                        receiveList.RemoveRange(0, 4);
                        op = client.GetProtocol(id);
                    }
                }
                if (op != null)
                {
                    if (op.Decode(receiveList, client))
                    {
                        break;
                    }
                    else
                    {
                        op = null;
                    }

                }
                else
                {
                    break;
                }
            }
        }
    }
}
