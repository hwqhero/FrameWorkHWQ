
public abstract class BaseNetHWQ
{
    public virtual List<byte> Serialize()
    {
        return null;
    }

    public virtual void Deserialize(BinaryReader dataStream)
    {

    }

    
    public virtual int CustomCode()
    {
        return -1;
    }

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
        return SendClientDataList(mainCMD, subCMD, DataTypeEnum.CustomObject, count, code, dateList);
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
        return SendClientDataList(mainCMD, subCMD, DataTypeEnum.CustomList, (short)list.Count, code, dateList);
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
        return SendClientDataList(mainCMD, subCMD, DataTypeEnum.HWQChar, (short)strList.Length, 0, dateList);
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
        return SendClientDataList(mainCMD, subCMD, DataTypeEnum.HWQInt, (short)intList.Length, 0, dateList);
    }


    public static byte[] WriteObject(byte mainCMD,byte subCMD,object obj)
    {
        if (obj is int[])
        {
            return SocketDateTool.WriteIntList(mainCMD, subCMD, obj as int[]);
        }
        else if (obj is string[])
        {

            return SocketDateTool.WriteStringList(mainCMD, subCMD, obj as string[]);
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
            return SendClientDataList(mainCMD, subCMD, DataTypeEnum.CustomList, (short)l.Count, code, dateList);
        }
        return null;
    }


    /// <summary>
    /// 发送数据给客户端
    /// </summary>
    /// <param name="mainCMD">主命令</param>
    /// <param name="subCMD">子命令</param>
    /// <param name="dataList">数据区</param>
    private static byte[] SendClientDataList(byte mainCMD, byte subCMD, DataTypeEnum typeEnum, short listCount, int code, List<byte> dataList)
    {
        List<byte> sendList = new List<byte>();
        sendList.AddRange(BitConverter.GetBytes(dataList.Count));
        sendList.Add(mainCMD);
        sendList.Add(subCMD);
        sendList.Add((byte)typeEnum);
        sendList.AddRange(BitConverter.GetBytes(listCount));
        sendList.AddRange(BitConverter.GetBytes(code));
        sendList.AddRange(dataList);
        return sendList.ToArray();
    }
}

public class ProtocolData
{
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
        dataType = (DataTypeEnum)data[6];
        dataCount = System.BitConverter.ToInt16(temp, 7);
        dataCode = System.BitConverter.ToInt32(temp, 9);
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






//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:2.0.50727.5420
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

namespace NetEntityHWQ
{
    using System;
    using System.IO;
    using System.Text;
    using System.Collections.Generic;
    using System.Collections;
    
    
    public class User : BaseNetHWQ
    {
        
        public String userName;
        
        public String pwd;
        
        public override List<byte> Serialize()
        {
            List<byte> tempList = new List<byte>();
		tempList.AddRange(BitConverter.GetBytes(true));
		s(tempList,userName);
		s(tempList,pwd);
		return tempList;

        }
        
        public override void Deserialize(BinaryReader br)
        {
		userName = d(br);
		pwd = d(br);

        }
        
        public override int CustomCode()
        {
            return 2046544;
        }
    }
    
    public class UserData : BaseNetHWQ
    {
        
        public Int32 userId;
        
        public List<UserBack> backList;
        
        public List<UserEquip> equipList;
        
        public override List<byte> Serialize()
        {
            List<byte> tempList = new List<byte>();
		tempList.AddRange(BitConverter.GetBytes(true));
		tempList.AddRange(BitConverter.GetBytes(userId));
		if(backList == null)
		{
		tempList.AddRange(BitConverter.GetBytes((short)0));
		}
		else
		{
		tempList.AddRange(BitConverter.GetBytes((short)backList.Count));
		foreach (UserBack temp in backList)
		{
			tempList.AddRange(temp.Serialize());
		}
		}
		if(equipList == null)
		{
		tempList.AddRange(BitConverter.GetBytes((short)0));
		}
		else
		{
		tempList.AddRange(BitConverter.GetBytes((short)equipList.Count));
		foreach (UserEquip temp in equipList)
		{
			tempList.AddRange(temp.Serialize());
		}
		}
		return tempList;

        }
        
        public override void Deserialize(BinaryReader br)
        {
		userId = br.ReadInt32();
		int backListCount = br.ReadInt16();
		backList = new List<UserBack>();
		for(int i = 0;i<backListCount;i++)
		{
		if (br.ReadBoolean())
		{
			UserBack  obj = new UserBack();
			obj.Deserialize(br);
			backList.Add(obj);
		}
		}
		int equipListCount = br.ReadInt16();
		equipList = new List<UserEquip>();
		for(int i = 0;i<equipListCount;i++)
		{
		if (br.ReadBoolean())
		{
			UserEquip  obj = new UserEquip();
			obj.Deserialize(br);
			equipList.Add(obj);
		}
		}

        }
        
        public override int CustomCode()
        {
            return 2046776;
        }
    }
    
    public class UserBack : BaseNetHWQ
    {
        
        public Int32 backId;
        
        public Int32 userId;
        
        public Int32 itemId;
        
        public Int32 gridNum;
        
        public Int32 isEquip;
        
        public List<Int32> tt;
        
        public override List<byte> Serialize()
        {
            List<byte> tempList = new List<byte>();
		tempList.AddRange(BitConverter.GetBytes(true));
		tempList.AddRange(BitConverter.GetBytes(backId));
		tempList.AddRange(BitConverter.GetBytes(userId));
		tempList.AddRange(BitConverter.GetBytes(itemId));
		tempList.AddRange(BitConverter.GetBytes(gridNum));
		tempList.AddRange(BitConverter.GetBytes(isEquip));
		if(tt == null)
		{
		tempList.AddRange(BitConverter.GetBytes((short)0));
		}
		else
		{
		tempList.AddRange(BitConverter.GetBytes((short)tt.Count));
		foreach (Int32 temp in tt)
		{
			tempList.AddRange(BitConverter.GetBytes(temp));
		}
		}
		return tempList;

        }
        
        public override void Deserialize(BinaryReader br)
        {
		backId = br.ReadInt32();
		userId = br.ReadInt32();
		itemId = br.ReadInt32();
		gridNum = br.ReadInt32();
		isEquip = br.ReadInt32();
		int ttCount = br.ReadInt16();
		tt = new List<Int32>();
		for(int i = 0;i<ttCount;i++)
		{
			tt.Add(br.ReadInt32());
		}

        }
        
        public override int CustomCode()
        {
            return 2047056;
        }
    }
    
    public class UserEquip : BaseNetHWQ
    {
        
        public Int32 equipId;
        
        public Int32 userId;
        
        public Int32 backId;
        
        public Int32 placeType;
        
        public override List<byte> Serialize()
        {
            List<byte> tempList = new List<byte>();
		tempList.AddRange(BitConverter.GetBytes(true));
		tempList.AddRange(BitConverter.GetBytes(equipId));
		tempList.AddRange(BitConverter.GetBytes(userId));
		tempList.AddRange(BitConverter.GetBytes(backId));
		tempList.AddRange(BitConverter.GetBytes(placeType));
		return tempList;

        }
        
        public override void Deserialize(BinaryReader br)
        {
		equipId = br.ReadInt32();
		userId = br.ReadInt32();
		backId = br.ReadInt32();
		placeType = br.ReadInt32();

        }
        
        public override int CustomCode()
        {
            return 2047280;
        }
    }
    
    internal class DataFactory
    {
        
        internal static BaseNetHWQ CreateObject(int code)
        {
BaseNetHWQ data = null;
switch(code){
case 2046544:
data = new User();
break;
case 2046776:
data = new UserData();
break;
case 2047056:
data = new UserBack();
break;
case 2047280:
data = new UserEquip();
break;
}
return data;

        }
    }
}
