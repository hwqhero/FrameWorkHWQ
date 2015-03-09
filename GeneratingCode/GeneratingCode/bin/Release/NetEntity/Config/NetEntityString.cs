
public abstract class BaseNetHWQ
{
    internal abstract List<byte> Serialize();
    internal abstract void Deserialize(BinaryReader dataStream);
    internal abstract int CustomCode();

    protected string d(BinaryReader dataStream)
    {
        return Encoding.UTF8.GetString(dataStream.ReadBytes(dataStream.ReadInt32()));
    }

    protected void s(List<byte> tempList, string s)
    {
        byte[] temp = Encoding.UTF8.GetBytes(string.IsNullOrEmpty(s) ? string.Empty : s);
        tempList.AddRange(BitConverter.GetBytes(temp.Length));
        tempList.AddRange(temp);
    }

}

internal enum DataTypeEnum : byte
{
    CustomObject,
    CustomList,
    HWQChar,
    HWQInt,
    HWQFloat,
    HWQDouble,
    HWQShort,
    HWQLong
}

public class SocketDateTool
{
    /// <summary>
    /// 发送对象
    /// </summary>
    /// <param name="bdHWQ"></param>
    public static byte[] WriteObject(byte mainCMD, byte subCMD, BaseNetHWQ bdHWQ)
    {
        List<byte> dateList = new List<byte>();
        short count = 0;
        int code = 0;
        if (bdHWQ != null)
        {
            count = 1;
            code = bdHWQ.CustomCode();
            dateList.AddRange(bdHWQ.Serialize());
        }
        return SendClientDataList(mainCMD, subCMD, DataTypeEnum.CustomObject, count, code, dateList, 0);
    }

    /// <summary>
    /// 发送集合
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    public static byte[] WriteList<T>(byte mainCMD, byte subCMD, List<T> list) where T : BaseNetHWQ
    {
        List<byte> dateList = new List<byte>();
        int code = 0;
        if (list.Count > 0)
        {
            code = list[0].CustomCode();
            foreach (BaseNetHWQ bd in list)
            {
                dateList.AddRange(bd.Serialize());
            }
        }
        return SendClientDataList(mainCMD, subCMD, DataTypeEnum.CustomList, (short)list.Count, code, dateList, 0);
    }

    /// <summary>
    /// 发送字符集合
    /// </summary>
    /// <param name="strList"></param>
    public static byte[] WriteStringList(byte mainCMD, byte subCMD, params string[] strList)
    {
        List<byte> dateList = new List<byte>();
        foreach (string s in strList)
        {
            byte[] tempList = Encoding.UTF8.GetBytes(s);
            dateList.AddRange(BitConverter.GetBytes(tempList.Length));
            dateList.AddRange(tempList);
        }
        return SendClientDataList(mainCMD, subCMD, DataTypeEnum.HWQChar, (short)strList.Length, 0, dateList, 0);
    }

    /// <summary>
    /// 发送数值集合
    /// </summary>
    /// <param name="intList"></param>
    public static byte[] WriteIntList(byte mainCMD, byte subCMD, params int[] intList)
    {
        List<byte> dateList = new List<byte>();
        foreach (int i in intList)
        {
            dateList.AddRange(BitConverter.GetBytes(i));
        }
        return SendClientDataList(mainCMD, subCMD, DataTypeEnum.HWQInt, (short)intList.Length, 0, dateList, 0);
    }

    public static byte[] WriteErrorCode(byte mainCMD, byte subCMD, short errorCode)
    {
        return SendClientDataList(mainCMD, subCMD, DataTypeEnum.HWQInt, 0, 0, new List<byte>(), errorCode);
    }


    public static byte[] WriteObject(byte mainCMD, byte subCMD, object obj)
    {
        if (obj is int[])
        {
            return SocketDateTool.WriteIntList(mainCMD, subCMD, obj as int[]);
        }
        else if (obj is string[])
        {

            return SocketDateTool.WriteStringList(mainCMD, subCMD, obj as string[]);
        }
        else if (obj is long[])
        {

        }
        else if (obj is BaseNetHWQ)
        {
            return SocketDateTool.WriteObject(mainCMD, subCMD, obj as BaseNetHWQ);
        }
        else if (obj is IList)
        {
            List<byte> dateList = new List<byte>();
            List<BaseNetHWQ> l = obj as List<BaseNetHWQ>;
            int code = 0;
            if (l.Count > 0)
            {
                code = l[0].CustomCode();
                foreach (BaseNetHWQ bd in l)
                {
                    dateList.AddRange(bd.Serialize());
                }
            }
            return SendClientDataList(mainCMD, subCMD, DataTypeEnum.CustomList, (short)l.Count, code, dateList, 0);
        }
        return null;
    }


    /// <summary>
    /// 发送数据给客户端
    /// </summary>
    /// <param name="mainCMD">主命令</param>
    /// <param name="subCMD">子命令</param>
    /// <param name="dataList">数据区</param>
    private static byte[] SendClientDataList(byte mainCMD, byte subCMD, DataTypeEnum typeEnum, short listCount, int code, List<byte> dataList, short errorCode)
    {
        List<byte> sendList = new List<byte>();
        
        sendList.AddRange(BitConverter.GetBytes(dataList.Count));
        sendList.Add(mainCMD);
        sendList.Add(subCMD);
        sendList.AddRange(BitConverter.GetBytes(DateTime.Now.ToBinary()));
        sendList.AddRange(BitConverter.GetBytes(errorCode));
        sendList.Add((byte)typeEnum);
        sendList.AddRange(BitConverter.GetBytes(listCount));
        sendList.AddRange(BitConverter.GetBytes(code));
        sendList.AddRange(dataList);
        return sendList.ToArray();
    }
}

public class ProtocolData
{
    public const int headCount = 23;
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
    /// 发送时间
    /// </summary>
    public long sendTime;
    /// <summary>
    /// 错误码
    /// </summary>
    public short errorCode;
    /// <summary>
    /// 数据区类型 
    /// </summary>
    internal DataTypeEnum dataType;
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

    public ProtocolData(List<byte> data)
    {
        byte[] temp = data.ToArray();
        length = System.BitConverter.ToInt32(temp, 0);
        mainCmd = data[4];
        subCmd = data[5];
        sendTime = System.BitConverter.ToInt64(temp, 6);
        errorCode = System.BitConverter.ToInt16(temp, 14);
        dataType = (DataTypeEnum)data[16];
        dataCount = System.BitConverter.ToInt16(temp, 17);
        dataCode = System.BitConverter.ToInt32(temp, 19);
    }

    /// <summary>
    /// 解码
    /// </summary>
    public void Decode()
    {
        switch (dataType)
        {
            case DataTypeEnum.CustomObject:
                DecodeObject();
                break;
            case DataTypeEnum.CustomList:
                DecodeList();
                break;
            case DataTypeEnum.HWQChar:
                DecodeHWQChar();
                break;
            case DataTypeEnum.HWQInt:
                DecodeHWQInt();
                break;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    private void DecodeObject()
    {
        if (dataCount == 1)
        {
            BinaryReader br = new BinaryReader(new MemoryStream(dataList));
            if (br.ReadBoolean())
            {
                BaseNetHWQ bdhwq = DataFactory.CreateObject(dataCode);
                bdhwq.Deserialize(br);
                baseData = bdhwq;
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    private void DecodeList()
    {
        List<BaseNetHWQ> tempList = new List<BaseNetHWQ>();
        BinaryReader br = new BinaryReader(new MemoryStream(dataList));
        for (int i = 0; i < dataCount; i++)
        {
            if (br.ReadBoolean())
            {
                BaseNetHWQ bdhwq = DataFactory.CreateObject(dataCode);
                bdhwq.Deserialize(br);
                tempList.Add(bdhwq);
            }
        }
        baseData = tempList;
    }

    /// <summary>
    /// 
    /// </summary>
    private void DecodeHWQChar()
    {
        List<string> tempList = new List<string>();
        BinaryReader br = new BinaryReader(new MemoryStream(dataList));
        for (int i = 0; i < dataCount; i++)
        {
            tempList.Add(System.Text.Encoding.UTF8.GetString(br.ReadBytes(br.ReadInt32())));
        }
        baseData = tempList;
    }

    /// <summary>
    /// 解码
    /// </summary>
    private void DecodeHWQInt()
    {
        List<int> tempList = new List<int>();
        BinaryReader br = new BinaryReader(new MemoryStream(dataList));
        for (int i = 0; i < dataCount; i++)
        {
            tempList.Add(br.ReadInt32());
        }
        baseData = tempList;
    }
}






