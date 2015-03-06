using UnityEngine;
using System.Collections;

public class ResourcesExpandTool
{
    public static T Instantiate<T>(string path, Vector3 position) where T : Object
    {
        T t = Resources.Load<T>(path);
        if (t != null)
        {
            return Object.Instantiate(t, position, Quaternion.identity) as T;
        }
        return default(T);
    }

    public static T Instantiate<T>(string path, Vector3 position,Transform parent) where T : Object
    {
        T t = Resources.Load<T>(path);
        if (t != null)
        {
            T entry = Object.Instantiate(t, position, Quaternion.identity) as T;
            GameObject go = entry as GameObject;
            go.transform.SetParent(parent);
            return Object.Instantiate(t, position, Quaternion.identity) as T;
        }
        return default(T);
    }
}
