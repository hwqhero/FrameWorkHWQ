using UnityEngine;
using System.Collections;
using BaseEngine;
/// <summary>
/// NGUI UI Root
/// </summary>
public sealed class UIRootHWQ : MetaHWQ
{

    private UIRootHWQ() { }

    internal static UIRootHWQ intance;
    private new void Awake()
    {
        if (intance != null)
            DestroyImmediate(intance.gameObject, true);
        intance = this;
        DontDestroyOnLoad(gameObject);
    }
}


