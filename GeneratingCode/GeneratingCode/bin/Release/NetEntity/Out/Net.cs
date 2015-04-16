//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:2.0.50727.5485
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
    
    
    public class LoginResult : BaseNetHWQ
    {
        
        public UInt64 userID;
        
        public String worldIP;
        
        public Int32 wroldProt;
        
        internal override List<byte> Serialize()
        {
            List<byte> tempList = new List<byte>();
		tempList.AddRange(BitConverter.GetBytes(true));
		tempList.AddRange(BitConverter.GetBytes(userID));
		s(tempList,worldIP);
		tempList.AddRange(BitConverter.GetBytes(wroldProt));
		return tempList;

        }
        
        internal override void Deserialize(BinaryReader br)
        {
		userID = br.ReadUInt64();
		worldIP = d(br);
		wroldProt = br.ReadInt32();

        }
        
        internal override int CustomCode()
        {
            return 1916392;
        }
    }
    
    public class RpgMapData : BaseNetHWQ
    {
        
        public UInt32 mapId;
        
        public String mapName;
        
        public Single x;
        
        public Single y;
        
        public Single z;
        
        public String mapResourceName;
        
        internal override List<byte> Serialize()
        {
            List<byte> tempList = new List<byte>();
		tempList.AddRange(BitConverter.GetBytes(true));
		tempList.AddRange(BitConverter.GetBytes(mapId));
		s(tempList,mapName);
		tempList.AddRange(BitConverter.GetBytes(x));
		tempList.AddRange(BitConverter.GetBytes(y));
		tempList.AddRange(BitConverter.GetBytes(z));
		s(tempList,mapResourceName);
		return tempList;

        }
        
        internal override void Deserialize(BinaryReader br)
        {
		mapId = br.ReadUInt32();
		mapName = d(br);
		x = br.ReadSingle();
		y = br.ReadSingle();
		z = br.ReadSingle();
		mapResourceName = d(br);

        }
        
        internal override int CustomCode()
        {
            return 1916672;
        }
    }
    
    public class RigureConfig : BaseNetHWQ
    {
        
        public Byte id;
        
        public UInt32 hp;
        
        public UInt32 attack;
        
        public UInt32 defense;
        
        public UInt32 attackSpeed;
        
        public UInt32 moveSpeed;
        
        public List<Int32> skillList;
        
        internal override List<byte> Serialize()
        {
            List<byte> tempList = new List<byte>();
		tempList.AddRange(BitConverter.GetBytes(true));
		tempList.Add(id);
		tempList.AddRange(BitConverter.GetBytes(hp));
		tempList.AddRange(BitConverter.GetBytes(attack));
		tempList.AddRange(BitConverter.GetBytes(defense));
		tempList.AddRange(BitConverter.GetBytes(attackSpeed));
		tempList.AddRange(BitConverter.GetBytes(moveSpeed));
		if(skillList == null)
		{
		tempList.AddRange(BitConverter.GetBytes((short)0));
		}
		else
		{
		tempList.AddRange(BitConverter.GetBytes((short)skillList.Count));
		foreach (Int32 temp in skillList)
		{
			tempList.AddRange(BitConverter.GetBytes(temp));
		}
		}
		return tempList;

        }
        
        internal override void Deserialize(BinaryReader br)
        {
		id = br.ReadByte();
		hp = br.ReadUInt32();
		attack = br.ReadUInt32();
		defense = br.ReadUInt32();
		attackSpeed = br.ReadUInt32();
		moveSpeed = br.ReadUInt32();
		int skillListCount = br.ReadInt16();
		skillList = new List<Int32>();
		for(int i = 0;i<skillListCount;i++)
		{
			skillList.Add(br.ReadInt32());
		}

        }
        
        internal override int CustomCode()
        {
            return 1916968;
        }
    }
    
    public class SyncPoint : BaseNetHWQ
    {
        
        public Single x;
        
        public Single y;
        
        public Single z;
        
        public Single rx;
        
        public Single ry;
        
        public Single rz;
        
        internal override List<byte> Serialize()
        {
            List<byte> tempList = new List<byte>();
		tempList.AddRange(BitConverter.GetBytes(true));
		tempList.AddRange(BitConverter.GetBytes(x));
		tempList.AddRange(BitConverter.GetBytes(y));
		tempList.AddRange(BitConverter.GetBytes(z));
		tempList.AddRange(BitConverter.GetBytes(rx));
		tempList.AddRange(BitConverter.GetBytes(ry));
		tempList.AddRange(BitConverter.GetBytes(rz));
		return tempList;

        }
        
        internal override void Deserialize(BinaryReader br)
        {
		x = br.ReadSingle();
		y = br.ReadSingle();
		z = br.ReadSingle();
		rx = br.ReadSingle();
		ry = br.ReadSingle();
		rz = br.ReadSingle();

        }
        
        internal override int CustomCode()
        {
            return 1917224;
        }
    }
    
    public class QueryUser : BaseNetHWQ
    {
        
        public String userName;
        
        public String pwd;
        
        public Int32 clientID;
        
        internal override List<byte> Serialize()
        {
            List<byte> tempList = new List<byte>();
		tempList.AddRange(BitConverter.GetBytes(true));
		s(tempList,userName);
		s(tempList,pwd);
		tempList.AddRange(BitConverter.GetBytes(clientID));
		return tempList;

        }
        
        internal override void Deserialize(BinaryReader br)
        {
		userName = d(br);
		pwd = d(br);
		clientID = br.ReadInt32();

        }
        
        internal override int CustomCode()
        {
            return 1917456;
        }
    }
    
    public class QueryUserResult : BaseNetHWQ
    {
        
        public Int32 clientID;
        
        public Int32 errorCode;
        
        public UInt64 userID;
        
        public String name;
        
        public String pwd;
        
        public Int32 enable;
        
        public Int32 online;
        
        internal override List<byte> Serialize()
        {
            List<byte> tempList = new List<byte>();
		tempList.AddRange(BitConverter.GetBytes(true));
		tempList.AddRange(BitConverter.GetBytes(clientID));
		tempList.AddRange(BitConverter.GetBytes(errorCode));
		tempList.AddRange(BitConverter.GetBytes(userID));
		s(tempList,name);
		s(tempList,pwd);
		tempList.AddRange(BitConverter.GetBytes(enable));
		tempList.AddRange(BitConverter.GetBytes(online));
		return tempList;

        }
        
        internal override void Deserialize(BinaryReader br)
        {
		clientID = br.ReadInt32();
		errorCode = br.ReadInt32();
		userID = br.ReadUInt64();
		name = d(br);
		pwd = d(br);
		enable = br.ReadInt32();
		online = br.ReadInt32();

        }
        
        internal override int CustomCode()
        {
            return 1917752;
        }
    }
    
    public class UserData : BaseNetHWQ
    {
        
        public UInt64 userId;
        
        public UInt64 userRoleId;
        
        public UInt32 userBackLimit;
        
        public List<UserBack> backList;
        
        public List<UserEquip> equipList;
        
        public List<UserRole> roleList;
        
        internal override List<byte> Serialize()
        {
            List<byte> tempList = new List<byte>();
		tempList.AddRange(BitConverter.GetBytes(true));
		tempList.AddRange(BitConverter.GetBytes(userId));
		tempList.AddRange(BitConverter.GetBytes(userRoleId));
		tempList.AddRange(BitConverter.GetBytes(userBackLimit));
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
		if(roleList == null)
		{
		tempList.AddRange(BitConverter.GetBytes((short)0));
		}
		else
		{
		tempList.AddRange(BitConverter.GetBytes((short)roleList.Count));
		foreach (UserRole temp in roleList)
		{
			tempList.AddRange(temp.Serialize());
		}
		}
		return tempList;

        }
        
        internal override void Deserialize(BinaryReader br)
        {
		userId = br.ReadUInt64();
		userRoleId = br.ReadUInt64();
		userBackLimit = br.ReadUInt32();
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
		int roleListCount = br.ReadInt16();
		roleList = new List<UserRole>();
		for(int i = 0;i<roleListCount;i++)
		{
		if (br.ReadBoolean())
		{
			UserRole  obj = new UserRole();
			obj.Deserialize(br);
			roleList.Add(obj);
		}
		}

        }
        
        internal override int CustomCode()
        {
            return 1918032;
        }
    }
    
    public class UserBack : BaseNetHWQ
    {
        
        public UInt64 backId;
        
        public UInt64 userId;
        
        public UInt32 itemId;
        
        public UInt32 gridNum;
        
        public Boolean isEquip;
        
        internal override List<byte> Serialize()
        {
            List<byte> tempList = new List<byte>();
		tempList.AddRange(BitConverter.GetBytes(true));
		tempList.AddRange(BitConverter.GetBytes(backId));
		tempList.AddRange(BitConverter.GetBytes(userId));
		tempList.AddRange(BitConverter.GetBytes(itemId));
		tempList.AddRange(BitConverter.GetBytes(gridNum));
		tempList.AddRange(BitConverter.GetBytes(isEquip));
		return tempList;

        }
        
        internal override void Deserialize(BinaryReader br)
        {
		backId = br.ReadUInt64();
		userId = br.ReadUInt64();
		itemId = br.ReadUInt32();
		gridNum = br.ReadUInt32();
		isEquip = br.ReadBoolean();

        }
        
        internal override int CustomCode()
        {
            return 1918272;
        }
    }
    
    public class UserEquip : BaseNetHWQ
    {
        
        public UInt64 equipId;
        
        public UInt64 userId;
        
        public UInt64 backId;
        
        public UInt32 placeType;
        
        internal override List<byte> Serialize()
        {
            List<byte> tempList = new List<byte>();
		tempList.AddRange(BitConverter.GetBytes(true));
		tempList.AddRange(BitConverter.GetBytes(equipId));
		tempList.AddRange(BitConverter.GetBytes(userId));
		tempList.AddRange(BitConverter.GetBytes(backId));
		tempList.AddRange(BitConverter.GetBytes(placeType));
		return tempList;

        }
        
        internal override void Deserialize(BinaryReader br)
        {
		equipId = br.ReadUInt64();
		userId = br.ReadUInt64();
		backId = br.ReadUInt64();
		placeType = br.ReadUInt32();

        }
        
        internal override int CustomCode()
        {
            return 1918496;
        }
    }
    
    public class UserRole : BaseNetHWQ
    {
        
        public UInt64 userId;
        
        public UInt64 roleId;
        
        public UInt32 roleLevel;
        
        public String roleName;
        
        public UInt32 exp;
        
        public Byte rigureId;
        
        public Byte rigureId1;
        
        internal override List<byte> Serialize()
        {
            List<byte> tempList = new List<byte>();
		tempList.AddRange(BitConverter.GetBytes(true));
		tempList.AddRange(BitConverter.GetBytes(userId));
		tempList.AddRange(BitConverter.GetBytes(roleId));
		tempList.AddRange(BitConverter.GetBytes(roleLevel));
		s(tempList,roleName);
		tempList.AddRange(BitConverter.GetBytes(exp));
		tempList.Add(rigureId);
		tempList.Add(rigureId1);
		return tempList;

        }
        
        internal override void Deserialize(BinaryReader br)
        {
		userId = br.ReadUInt64();
		roleId = br.ReadUInt64();
		roleLevel = br.ReadUInt32();
		roleName = d(br);
		exp = br.ReadUInt32();
		rigureId = br.ReadByte();
		rigureId1 = br.ReadByte();

        }
        
        internal override int CustomCode()
        {
            return 1918792;
        }
    }
    
    internal class DataFactory
    {
        
        internal static BaseNetHWQ CreateObject(int code)
        {
BaseNetHWQ data = null;
switch(code){
case 1916392:
data = new LoginResult();
break;
case 1916672:
data = new RpgMapData();
break;
case 1916968:
data = new RigureConfig();
break;
case 1917224:
data = new SyncPoint();
break;
case 1917456:
data = new QueryUser();
break;
case 1917752:
data = new QueryUserResult();
break;
case 1918032:
data = new UserData();
break;
case 1918272:
data = new UserBack();
break;
case 1918496:
data = new UserEquip();
break;
case 1918792:
data = new UserRole();
break;
}
return data;

        }
    }

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






}
