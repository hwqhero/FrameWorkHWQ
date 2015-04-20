using UnityEngine;
using System.Collections;
using System.IO;
using System;

/// <summary>
/// 文件工具
/// </summary>
public class FilePlatformTool
{
    private static FilePlatformTool m_instance;
    private AndroidJavaObject jo;
    private string dataPath;
    private string persistentDataPath;
    private FilePlatformTool()
    { }

    private void Init()
    {
        dataPath = Application.dataPath;
        //dataPath = Application.persistentDataPath;
        if (!Application.isEditor)
        {
            AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
        }
        

#if UNITY_EDITOR
        dataPath = Application.dataPath;
        persistentDataPath = dataPath.Substring(0, dataPath.LastIndexOf('/')) + "/";
#elif UNITY_IPHONE
        dataPath = Application.dataPath + "/";
        persistentDataPath = Application.persistentDataPath+ "/";
#elif UNITY_ANDROID
 
#endif

    }

    public void ReStart()
    {
        if (jo != null)
            jo.Call("ReStartHWQ");
    }

    /// <summary>
    /// 读取文件
    /// </summary>
    /// <param name="path"></param>
    /// <param name="callBack"></param>
    public void LoadFile(string path,System.Action<BinaryReader> callBack)
    {
        #if UNITY_ANDROID
        BinaryReader br = new BinaryReader(new MemoryStream(jo.Call<byte[]>("GetFromAssets", path)));
        if (callBack != null)
        {
             callBack(br);
        }
        br.Close();
#else
        string iphonePath = IphonePath(path);
        if (string.IsNullOrEmpty(iphonePath))
        {
            Debug.Log(path + "<--不存在");
            return;
        }
        try
        {
            FileStream fs = File.OpenRead(iphonePath);
            BinaryReader br = new BinaryReader(fs);
            if (br == null)
            {
                Debug.Log(path + "<--不能将此路径文件转为二进制流");
                return;
            }
            if (callBack != null)
            {
                callBack(br);
            }
            br.Close();
        }
        catch (UnauthorizedAccessException e)
        {
            Debug.Log("目录没有访问权限"+e.Message);
        }
        catch (PathTooLongException e)
        {
            Debug.Log("路径超出了系统长度"+e.Message);
        }
        catch (NotSupportedException e)
        {
            Debug.Log("路径格式无效"+e.Message);
        }
        finally
        {
           
        }
#endif
       
    }

    /// <summary>
    /// 获取Asset文件
    /// </summary>
    /// <param name="path">路径</param>
    /// <returns></returns>
    public byte[] ReadFileToByte(string path)
    {
        byte[] tempList = null;
        try
        {
            if (jo != null)
                tempList = jo.Call<byte[]>("GetFromAssets", path);
            else
            {
                string iphonePath = IphonePath(path);
                if (string.IsNullOrEmpty(iphonePath))
                {
                    Debug.Log(path + "<--不存在");
                    return null;
                }
                return File.ReadAllBytes(iphonePath);
            }
        }
        catch (Exception ex)
        {
            HWQEngine.Log(ex.Message);
        }

        return tempList;
    }



    /// <summary>
    /// 读取AssetBundle
    /// </summary>
    /// <param name="path">相对路径</param>
    /// <param name="callBack">加载成功回调</param>
    /// <param name="unload">回调后是否删除AssetBundle</param>
    public void LoadAssetBundle(string path,System.Action<AssetBundle> callBack,bool unload)
    {
        //double curn = MenuWindowUI.MyUsedMemory();
        string iphonePath = IphonePath(path);
        if (string.IsNullOrEmpty(iphonePath))
        {
            Debug.Log(path + "<---不存在");
            return;
        }
        byte[] temp = null;
#if UNITY_ANDROID
        temp = jo.Call<byte[]>("GetFromAssets", path);
#else
        temp = File.ReadAllBytes(iphonePath);
#endif
        AssetBundle ab = AssetBundle.CreateFromMemoryImmediate(temp);
        temp = null;
        if (ab != null)
        {
            if (callBack != null)
            {
                callBack(ab);
            }
            if (unload)
            {
                ab.Unload(false);
            }
        }
    }

    /// <summary>
    /// 按行读取
    /// </summary>
    /// <param name="path">相对路径</param>
    /// <param name="callback">加载成功回调</param>
    public void ReadByLine(string path,System.Action<string[]> callback)
    {
        string iphonePath = IphonePath(path);
        if (string.IsNullOrEmpty(iphonePath))
        {
            Debug.Log(path + "<---不存在");
        }
        else
        {
            string[] temp = File.ReadAllLines(iphonePath);
            if (callback != null)
            {
                callback(temp);
            }
            temp = null;
        }
        
    }


    #region 跨平台路径
    /// <summary>
    /// 获得路径
    /// </summary>
    /// <returns></returns>
    public string GetPathByPlatform()
    {

        string filepath = dataPath + "/StreamingAssets/";
#if UNITY_EDITOR
        filepath = dataPath + "/StreamingAssets/";
#elif UNITY_IPHONE
            filepath = dataPath +"Raw/";
#elif UNITY_ANDROID
            filepath =  Application.streamingAssetsPath+"/";
#endif

        return filepath;
    }
    #endregion

    /// <summary>
    /// iphone路径
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    private string IphonePath(string path)
    {
        string temp = string.Empty;
        if (File.Exists(persistentDataPath + path))
        {
            temp = persistentDataPath + path;
        }
        else if (File.Exists(GetPathByPlatform() + path))
        {
            temp = GetPathByPlatform() + path;
        }
        return temp;
    }

    /// <summary>
    /// 单例
    /// </summary>
    public static FilePlatformTool Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = new FilePlatformTool();
                m_instance.Init();
            }
            return m_instance;
        }
    }

}
