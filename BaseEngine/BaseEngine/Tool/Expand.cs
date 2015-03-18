using UnityEngine;
using System.Collections;

/// <summary>
/// 拓展类
/// </summary>
public static class Expand  {

    public static T InstantiateByPrefab<T>(this MonoBehaviour mb,string path) where T:Object
    {
        T t = Resources.Load<T>(path);
        if (!t)
            Debug.Log("没有找到预设----->" + path + "<-->" + mb);
        return Object.Instantiate(t) as T;
    }
}
