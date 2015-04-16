using System.Collections.Generic;
/// <summary>
/// 
/// </summary>
public class CustomProperty : System.Attribute
{
    public string FieldName;
    public string targetFieldName;
    public CustomProperty(string f, string f1)
    {
        FieldName = f;
        targetFieldName = f1;
    }
}

public class SplitString : System.Attribute
{
    public string stringName;
    public string targetId;
    public SplitString(string f, string f1)
    {
        stringName = f;
        targetId = f1;
    }
}

public class SplitString2 : System.Attribute
{
    public string[] tempString;
    public SplitString2(params string[] t)
    {
        tempString = t;
    }
}

/// <summary>
/// 
/// </summary>
public class C : System.Attribute
{
    public string[] tempString;
    public C(params string[] t)
    {
        tempString = t;
    }
}

public class Chapter
{
    public int id;
    public string name;
    [CustomProperty("id", "chapterId")]
    public List<Level> levelList;
}

public class Level
{
    public int id;
    public int chapterId;
    public string name;
    [CustomProperty("chapterId", "id")]
    public Chapter chapter;
}

public class LevelCondition
{
    public int id;
    public int levelId;
    public int conditionType;
    public string conditionValue;
}

public class MonsterConfigData : HeroConfigData
{
    public float attackInterval;
}

public class HeroConfigData
{
    public int roleId;
    public string name;
    public string titleName;
    public int physicsAttack;
    public int magicAttack;
    public int defense;
    public int magicResistance;
    public int pCritical;
    public int mCritical;
    public float attackSpeed;
    public int releaseSpeed;
    public int maxHP;
    public int maxMP;
    public int rating;
    public int dodge;
    public int hardValue;
    public int deftValue;
    public int repHP;
    public int repMP;
    public float moveSpeed;
    public int attackType;
    public float warnRange;
    public float attackRange;
    public int turnSpeed;
    public float attackAngle;
    public float warnAngel;
    [CustomProperty("roleId", "roleid")]
    public List<NormalAttackData> normalAttackList;
}

public class NormalAttackData
{
    public int roleid;
    public int index;
    public float frontTime;
    public float cancelTime;
    public float afterTime;
    public float bulletSpeed;
    public int hitType;
    public string effectName;
    public int direction;
    public float x;
    public float y;
    public float z;
}


public class SkillData
{
    public int skillId;
    public string skillName;
    public string skillDesc;
    [CustomProperty("skillId", "skillId")]
    public List<SkillCondition> conditionList;
    [CustomProperty("skillId", "skillId")]
    public List<SkillAffect> affectList;
}

public class SkillCondition
{
    public int skillId;
    public int type;
    public float value;
    public float percent;
    public int logicSymbol;
}


public class SkillEffect
{
    public int skillId;
    public int affectId;
    public int type;
    public float value;
    public float percent;
    public string timeId;
    [SplitString2("timeId", "timeid", "skillId", "skillId")]
    public List<SkillTime> timeList;
}

public class SkillBuff
{
    public int skillId;
    public int affectId;
    public int type;
    public string timeId;
    [SplitString2("timeId", "timeid","skillId","skillId")]
    public List<SkillTime> timeList;
}

public class SkillTime
{
    public int skillId;
    public int timeid;
    public int type;
    public float time;
}


public class SkillAffect
{
    public int skillId;
    public int affectId;
    public int type;
    public int rangeType;
    public float rangeAngle;
    public float rangeRadis;
    [C("affectId", "affectId", "skillId", "skillId")]
    public List<SkillEffect> effectList;
}



