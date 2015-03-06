using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ViewSingle : View
{

    private static Dictionary<int, ViewSingle> viewDic = new Dictionary<int, ViewSingle>();

    private Transform myTf;
    protected virtual void Awake()
    {
        myTf = transform;
    }




    protected Transform MyTF
    {
        get
        {
            if (myTf == null)
            {
                myTf = transform;
            }
            return myTf;
        }
    }

    public static T GetSingle<T>() where T : ViewSingle
    {
        int hc = typeof(T).GetHashCode();
        if (viewDic.ContainsKey(hc))
        {
            if (viewDic[hc] != null)
            {
                return viewDic[hc] as T;
            }
        }
        return default(T);
    }
}
