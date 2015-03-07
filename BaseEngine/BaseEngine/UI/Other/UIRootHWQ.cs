using UnityEngine;
using System.Collections;

public class UIRootHWQ : View
{
    internal static UIRootHWQ intance;
    protected override void Awake()
    {
        base.Awake();
        if (intance != null)
            DestroyImmediate(intance.gameObject, true);
        intance = this;
        DontDestroyOnLoad(gameObject);
    }

    public static void Create()
    {
        Instantiate(Resources.Load<UIRootHWQ>("UI/UIRoot/UI Root"));
    }
}
