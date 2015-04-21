using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ConverterToBin
{
    class Program
    {
        static void Main(string[] args)
        {
            Stopwatch sw = Stopwatch.StartNew();
            Assembly aaa = Assembly.LoadFrom("HWQConfigData.dll");
            Type c = aaa.GetType("ConfigData.ConfigManager", true);
            FieldInfo dic = c.GetField("allDataList", BindingFlags.Static | BindingFlags.NonPublic);
            object myobj = dic.GetValue(null);
            MethodInfo mi = c.GetMethod("Add", BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            MethodInfo to = c.GetMethod("ToBinary", BindingFlags.Static|BindingFlags.Instance|BindingFlags.NonPublic|BindingFlags.Public);
            foreach (Type t in aaa.GetTypes())
            {
                if (!t.IsAbstract && ReadParent(t,"ConfigMetaData"))
                {
                    IList list = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(t));
                    if (File.Exists("Xlsx/" + t.Name + ".xlsx"))
                    {
                        DataSet ds = ReadExcel.Load1("Xlsx/" + t.Name + ".xlsx");
                        for (int i = 1; i < ds.Tables[0].Rows.Count; i++)
                        {
                            object obj = Activator.CreateInstance(t);
                            int j = 0;
                            List<FieldInfo> tempList = new List<FieldInfo>();
                            if (!t.BaseType.Name.Equals("ConfigMetaData")) {
                                FieldInfo[] fiListList = t.BaseType.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
                                tempList.AddRange(fiListList);
                            }
                            tempList.AddRange(t.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly));
                            foreach (FieldInfo fi in tempList.ToArray())
                            {
                                if (fi.GetCustomAttributes(false).Length == 0)
                                {
                                    DataRow dr = ds.Tables[0].Rows[i];
                                    object @value = dr[j];
                                    try
                                    {
                                        fi.SetValue(obj, Convert.ChangeType(@value, fi.FieldType));
                                    }
                                    catch (Exception ex)
                                    {

                                    }
                                    j++;
                                }
                            }
                            list.Add(obj);
                        }
                        mi.Invoke(null, new object[] { list, t.GetHashCode() });
                    }
                    else if (File.Exists("Xlsx/" + t.Name + ".csv"))
                    {
                        string[] lines = File.ReadAllLines("Xlsx/" + t.Name + ".csv");
                        foreach (string line in lines)
                        {
                            object obj = Activator.CreateInstance(t);
                            string[] fieldValueList = line.Split(',');
                            List<FieldInfo> tempList = new List<FieldInfo>();
                            if (!t.BaseType.Name.Equals("ConfigMetaData"))
                            {
                                FieldInfo[] fiListList = t.BaseType.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
                                tempList.AddRange(fiListList);
                            }
                            tempList.AddRange(t.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly));
                            for (int j = 0; j < tempList.Count; j++)
                            {
                                if (tempList[j].GetCustomAttributes(false).Length == 0)
                                {
                                    tempList[j].SetValue(obj, Convert.ChangeType(fieldValueList[j], tempList[j].FieldType));
                                }
                            }
                            list.Add(obj);
                        }
                        mi.Invoke(null, new object[] { list, t.GetHashCode() });
                    }
                }
              
            }
            byte[] bbb = to.Invoke(null, null) as byte[];
            Console.WriteLine(myobj);
            File.WriteAllBytes("Data/ConfigData.hwq", bbb);
            sw.Stop();
            Console.WriteLine("耗时---->"+sw.Elapsed.TotalMilliseconds+"  ms<----");
        }

        static bool ReadParent(Type t , string name)
        {
            Type tt = t;
            do
            {
                if (tt.BaseType.Name.Equals(name))
                {
                    return true;
                }
                tt = tt.BaseType;
            } while (tt.BaseType != null);
            return false;
        }
    }

 
}
