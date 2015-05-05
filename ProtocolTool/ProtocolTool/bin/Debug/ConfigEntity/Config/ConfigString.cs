

public abstract class ConfigMetaData
{
    internal abstract int CustomCode();
    internal abstract void Serialize(BinaryWriter bw);
    internal abstract void Deserialize(BinaryReader br);
    internal abstract ConfigMetaData Clone();
}


public sealed class ConfigManager
{
    private static Dictionary<int, IList> allDataList = new Dictionary<int, IList>();

    private static void Add(IList list, int hc)
    {
        if (!allDataList.ContainsKey(hc))
        {
            allDataList.Add(hc, list);
        }
    }


    /// <summary>
    /// 从byte数组加载
    /// </summary>
    /// <param name="dataList"></param>
    public static void Load(byte[] dataList)
    {
        BinaryReader br = new BinaryReader(new MemoryStream(dataList));
        br.ReadString();
        int count = br.ReadInt32();
        for (int i = 0; i < count; i++)
        {
            int hc = br.ReadInt32();
            ConfigMetaData cmd = LoadType.Get(hc);
            IList list = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(cmd.GetType()));
            int listCount = br.ReadInt32();
            for (int j = 0; j < listCount; j++)
            {
                cmd = cmd.Clone();
                cmd.Deserialize(br);
                list.Add(cmd);
            }
            if (allDataList.ContainsKey(hc))
            {
                allDataList[hc] = list;
            }
            else
            {
                allDataList.Add(hc, list);
            }
        }
        br.Close();
        br = null;
        dataList = null;
        LoadType.Init();
        System.GC.Collect();
    }

    /// <summary>
    /// 转成byte数组
    /// </summary>
    /// <returns></returns>
    public static byte[] ToBinary()
    {
        MemoryStream ms = new MemoryStream();
        BinaryWriter bw = new BinaryWriter(ms);
        bw.Write("0");
        int count = 0;
        foreach (IList list in allDataList.Values)
        {
            if (list.Count > 0)
            {
                count++;
            }
        }
        bw.Write(count);
        foreach (IList list in allDataList.Values)
        {
            if (list.Count > 0)
            {
                bw.Write(((ConfigMetaData)list[0]).CustomCode());
                bw.Write(list.Count);
                foreach (ConfigMetaData cmd in list)
                {
                    cmd.Serialize(bw);
                }
            }
        }
        byte[] temp = ms.ToArray();
        ms.Close();
        bw.Close();
        ms = null;
        bw = null;
        return temp;
    }

    public static IList GetList(Type t)
    {
        int hc = LoadType.GetCode(t.Name);
        if (allDataList.ContainsKey(hc))
        {
            return allDataList[hc];
        }
        return new ArrayList();
    }

    public static List<T> GetList<T>() where T : ConfigMetaData
    {

        int hc = LoadType.GetCode(typeof(T).Name);
        if (allDataList.ContainsKey(hc))
        {
            return allDataList[hc] as List<T>;
        }
        return new List<T>();
    }

    /// <summary>
    /// 获得集合
    /// </summary>
    /// <typeparam name="T">类型</typeparam>
    /// <param name="callBack">存在次类型集合则回调</param>
    public static void GetList<T>(System.Action<List<T>> callBack) where T : ConfigMetaData
    {
        int hc = LoadType.GetCode(typeof(T).Name);
        if (allDataList.ContainsKey(hc))
        {
            callBack(allDataList[hc] as List<T>);
        }
    }
}