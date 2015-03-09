using System.Collections.Generic;
/// <summary>
/// 使用此特性属性有set方法
/// </summary>
public class CustomProperty : System.Attribute
{
    public string FieldName;
    public string targetFieldName;
    public CustomProperty(string f,string f1)
    {
        FieldName = f;
        targetFieldName = f1;
    }
}

public class Chapter
{
    public int id;
    public string name;
    [CustomProperty("id","id")]
    public List<Level> levelList;
}

public class Level
{
    public int id;
    public string name;
    [CustomProperty("id","id")]
    public Chapter chapter;
}

public class LevelCondition
{
    public int id;
    public int levelId;
    public int conditionType;
    public string conditionValue;
}

