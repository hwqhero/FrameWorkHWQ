using NetEntityHWQ;
using ServerEngine.OperationObject;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileServer
{

    public class ProtocolData : IProtocol
    {
        private List<byte> receiveList = new List<byte>();
        public const int headCount = 13;
        /// <summary>
        /// 数据区长度
        /// </summary>
        public int length;
        /// <summary>
        /// 主命令
        /// </summary>
        public byte mainCmd;
        /// <summary>
        /// 子命令
        /// </summary>
        public byte subCmd;
        /// <summary>
        /// 数据区类型 
        /// </summary>
        internal byte dataType;
        /// <summary>
        /// 数据数量
        /// </summary>
        public short dataCount;
        /// <summary>
        /// 数据类型
        /// </summary>
        public int dataCode;
        /// <summary>
        /// 数据区
        /// </summary>
        public byte[] dataList;

        public object baseData;

        private bool CheckHead;
       
        void IProtocol.AddByte(byte[] b, int offset, int count,System.Action<IProtocol> finish)
        {
            for (int i = offset; i < count; i++)
            {
                receiveList.Add(b[i]);
            }
            while (receiveList.Count >= headCount)
            {
                if (!CheckHead)
                {
                    BinaryReader br = new BinaryReader(new MemoryStream(receiveList.ToArray()));
                    length = br.ReadInt32();
                    mainCmd = br.ReadByte();
                    subCmd = br.ReadByte();
                    dataType = br.ReadByte();
                    dataCount = br.ReadInt16();
                    dataCode = br.ReadInt32();
                    CheckHead = true;
                }

                if (receiveList.Count >= headCount + length)
                {
                    byte[] temp = receiveList.ToArray();
                    dataList = new byte[length];
                    System.Array.Copy(temp, headCount, dataList, 0, length);
                    baseData = SocketDateTool.Decode(dataType, dataCount, dataList, dataCode);
                    if (finish != null)
                    {
                        finish(this);
                    }
                    receiveList.RemoveRange(0, headCount + length);
                    CheckHead = false;
                }
                else
                {
                    break;
                }
            }


        }

        byte IProtocol.GetMainCMD()
        {
            return mainCmd;
        }

        object IProtocol.GetObject()
        {
            return baseData;
        }

        byte IProtocol.GetSubCMD()
        {
            return subCmd;
        }

        public static byte[] DataList(byte m,byte s,object obj)
        {
            List<byte> sendList = new List<byte>();
            byte[] tempList = SocketDateTool.WriteObject(obj);
            sendList.AddRange(BitConverter.GetBytes(tempList.Length - 7));
            sendList.Add(m);
            sendList.Add(s);
            sendList.AddRange(tempList);
            return sendList.ToArray();
        }
    }


}
