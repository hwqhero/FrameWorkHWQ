using UnityEngine;
using System.Collections;
using BaseEngine;
public sealed class UIRootHWQ : MetaHWQ
{

    private UIRootHWQ() { }

    internal static UIRootHWQ intance;
    protected override void Awake()
    {
        base.Awake();
        if (intance != null)
            DestroyImmediate(intance.gameObject, true);
        intance = this;
        DontDestroyOnLoad(gameObject);
    }
}


