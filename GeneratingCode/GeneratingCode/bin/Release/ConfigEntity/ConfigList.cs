using System.Collections.Generic;
/// <summary>
/// 使用此特性属性有set方法
/// </summary>
public class CustomProperty : System.Attribute
{
    public string FieldName;
    public string FieldName1;
    public CustomProperty(string f,string f1)
    {
        FieldName = f;
        FieldName1 = f1;
    }
}

public class Chapter
{
    public int id;
    public string name;
}

public class Level
{
    public int id;
    public string name;
}

public class LevelCondition
{
    public int id;
    public int levelId;
    public int conditionType;
    public string conditionValue;
}

