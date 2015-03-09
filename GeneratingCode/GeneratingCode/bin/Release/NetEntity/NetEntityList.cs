using System;
using System.Collections;
using System.Collections.Generic;



public class LoginResult
{
    public ulong userID;
    public string worldIP;
    public int wroldProt;
}

public class RpgMapData
{
    public uint mapId;
    public string mapName;
    public float x;
    public float y;
    public float z;
    public string mapResourceName;
}

public class RigureConfig
{
    public byte id;
    public uint hp;
    public uint attack;
    public uint defense;
    public uint attackSpeed;
    public uint moveSpeed;
    public List<int> skillList;
}


public class SyncPoint
{
    public float x;
    public float y;
    public float z;
    public float rx;
    public float ry;
    public float rz;
}

public class QueryUser
{
    /// <summary>
    /// 用户名
    /// </summary>
    public string userName;
    /// <summary>
    /// 密码
    /// </summary>
    public string pwd;

    public int clientID;
}

public class QueryUserResult
{
    public int clientID;
    public int errorCode;
    public ulong userID;
    public string name;
    public string pwd;
    public int enable;
    public int online;

}

public class UserData
{
    public ulong userId;
    public ulong userRoleId;
    public uint userBackLimit;
    public List<UserBack> backList;
    public List<UserEquip> equipList;
    public List<UserRole> roleList;
}



public class UserBack
{
    public ulong backId;
    public ulong userId;
    public uint itemId;
    public uint gridNum;
    public bool isEquip;
}

public class UserEquip
{
    public ulong equipId;
    public ulong userId;
    public ulong backId;
    public uint placeType;
}

public class UserRole
{
    public ulong userId;
    public ulong roleId;
    public uint roleLevel;
    public string roleName;
    public uint exp;
    public byte rigureId;
    public byte rigureId1;
}

