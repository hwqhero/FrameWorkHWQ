﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;
using BaseEngine;
using BaseEngine.UI;
using ConfigData;
/// <summary>
/// 自定义入口
/// </summary>
public sealed class HWQEngine
{
    internal static bool log;
    internal static event System.Action entryPointEvent;
    internal static string dataPath = "ConfigData.hwq";
    private static GameObject go;

    private HWQEngine()
    {

    }

    private static void LoadConfigData(string path)
    {
        byte[] bb = FilePlatformTool.Instance.ReadFileToByte(path);
        if (bb != null)
            ConfigManager.Load(bb);
    }

    public static void MyReStart()
    {
        FilePlatformTool.Instance.ReStart();
    }

    /// <summary>
    /// enable engine log
    /// </summary>
    /// <param name="enable"></param>
    public static void EnableLog(bool enable)
    {
        log = enable;
    }

    internal static void Log(string message)
    {
        Log(message, null);
    }

    internal static void Log(string message,UnityEngine.Object obj)
    {
        if (log)
            UnityEngine.Debug.Log(message, obj);
    }

    private static void Create(Type[] types)
    {
        if (!go)
        {
            LoadConfigData(dataPath);
            go = new GameObject(string.Empty);
            go.AddComponent<WindowDispatch>();
            go.AddComponent<AsyncOperationTool>();
            go.AddComponent<DataCenter>();
        }

        float start = Time.realtimeSinceStartup;
        foreach (Type t in types)
        {
            if (!t.IsAbstract)
            {
                if (GetParent(t,typeof(BaseSystem)))
                {
                    Log("创建系统->>>" + t.Name);
                    go.AddComponent(t);
                }
                else if (GetParent(t,typeof(UIManger)))
                {
                    Log("创建UI管理->>>" + t.Name);
                    go.AddComponent(t);
                }
            }

        }
        Log("Main ---->" + (Time.realtimeSinceStartup));
        if (entryPointEvent != null)
        {
            entryPointEvent();
            entryPointEvent = null;
        }
        UnityEngine.Object.DontDestroyOnLoad(go);
    
    }
    /// <summary>
    /// 入口
    /// </summary>
    public static void Main()
    {
        Create(Assembly.GetCallingAssembly().GetTypes());
    }

    private static bool GetParent(Type t,Type target)
    {
        Type cur = t.BaseType;
        do
        {
            if (cur == target)
            {
                return true;
            }
            cur = cur.BaseType;
        } while (cur != null);
        return false;
    }

    public static void Main(System.Action<GameObject>  callback)
    {
        Create(Assembly.GetCallingAssembly().GetTypes());
        if (callback != null)
        {
            callback(go);
        }
    }
}

