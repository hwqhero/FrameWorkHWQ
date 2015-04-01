using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DataCenter.Tool
{
    public class ReaderHelper
    {
        public static List<T> Read<T>(MySqlCommand c,string sql) where T:new()
        {
            List<T> list = new List<T>();
            c.CommandText = sql;
            MySqlDataReader reader = c.ExecuteReader();
          
            FieldInfo[] fList = typeof(T).GetFields(BindingFlags.Instance|BindingFlags.NonPublic);
            while (reader.Read())
            {
                T t = new T();
                foreach (FieldInfo f in fList)
                {
                    f.SetValue(t, reader[f.Name]);
                }
                list.Add(t);
            }
            reader.Close();
            return list;
        }

    }
}
