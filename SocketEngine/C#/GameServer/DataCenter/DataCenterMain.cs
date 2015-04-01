using DataCenter.Tool;
using Entity;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using ServerEngine.Core;
using System.Threading;

namespace DataCenter
{
    public class DataCenterMain
    {
        private static string conn = "Database='project';Data Source='192.168.0.254';User Id='root';Password='project';charset='utf8';pooling=true";
        public static DataCenterMain Insance;
        private MySqlConnection connection;
        private SocketServer server;
        private Dictionary<int, IList> allDataDic = new Dictionary<int, IList>();
        private ConcurrentDictionary<int, IList> allDataCon = new ConcurrentDictionary<int, IList>();
        private void Init()
        {
            connection = new MySqlConnection(conn);
            connection.Open();
            Stopwatch s = Stopwatch.StartNew();
            ReadAllData();
            s.Stop();
            Console.WriteLine("初始化所有数据成功,耗时-->" + s.Elapsed.TotalMilliseconds + " ms");
            //ConcurrentQueue<MySqlConnection> myList = new ConcurrentQueue<MySqlConnection>();
            //for (int i = 0; i < 5; i++)
            //{
            //    MySqlConnection c = new MySqlConnection(conn);
            //    c.Open();
            //    myList.Enqueue(c);
            //}
            //s = Stopwatch.StartNew();
            //ReadAllData1(myList);
            //s.Stop();
            //Console.WriteLine("初始化所有数据成功,耗时-->" + s.Elapsed.TotalMilliseconds + " ms");
            server = SocketServer.CreateServer();
            server.Start("192.168.0.254", 8787);
        }


        private void ReadAllData()
        {
            foreach (Type t in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (t.Namespace!=null&&t.Namespace.Equals("Entity"))
                {
                    allDataDic.Add(t.GetHashCode(), Read(connection.CreateCommand(), t));
                }
            }
        }

        private void ReadAllData1(ConcurrentQueue<MySqlConnection> aaaaa)
        {
            Parallel.ForEach(Assembly.GetExecutingAssembly().GetTypes(), t =>
            {
                if (t.Namespace.Equals("Entity"))
                {
                    MySqlConnection c = null;
                    aaaaa.TryDequeue(out c);
                    allDataCon.TryAdd(t.GetHashCode(), Read(c.CreateCommand(), t));
                    c.Close();
                    MySqlConnection.ClearPool(c);
                }
            });
        }


        public IList Read(MySqlCommand c, Type t)
        {
            IList ilist = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(t));
            MethodInfo mi = t.GetMethod("TableName", BindingFlags.NonPublic | BindingFlags.Static);
            if (mi != null)
            {
                string sql = "select * from " + mi.Invoke(null, null);


                c.CommandText = sql;
                MySqlDataReader reader = c.ExecuteReader();

                FieldInfo[] fList = t.GetFields(BindingFlags.Instance | BindingFlags.NonPublic);
                while (reader.Read())
                {
                    object obj = Activator.CreateInstance(t);
                    foreach (FieldInfo f in fList)
                    {
                        f.SetValue(obj, reader[f.Name]);
                    }
                    ilist.Add(obj);
                }
                reader.Close();
            }
            return ilist;
        }

        public long Inster(string insert, params MySqlParameter[] p)
        {
            MySqlCommand c = connection.CreateCommand();
            c.CommandText = insert;
            c.Parameters.AddRange(p);
            try
            {
                int count = c.ExecuteNonQuery();
              
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                
            }
            return c.LastInsertedId;
        }


        /// <summary>
        /// 查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="conditionssql">条件sql   where id =</param>
        /// <returns></returns>
        public T GetObjectBySql<T>(string conditionssql,params MySqlParameter[] p) where T : new()
        {
            T t = new T();
            MySqlCommand c = connection.CreateCommand();
             MethodInfo mi = typeof(T).GetMethod("TableName", BindingFlags.NonPublic | BindingFlags.Static);
             c.CommandText = "select * from " + mi.Invoke(null, null) + "  " + conditionssql;
             c.Parameters.AddRange(p);
            MySqlDataReader reader = c.ExecuteReader();
            FieldInfo[] fList = typeof(T).GetFields(BindingFlags.Instance | BindingFlags.NonPublic);
            while (reader.Read())
            {
                foreach (FieldInfo f in fList)
                {
                    f.SetValue(t, reader[f.Name]);
                }
            }
            reader.Close();
            OperationTable<T>(list =>
            {
                list.Add(t);
            });
            return t;
        }




        public void OperationTable<T>(System.Action<List<T>> operationEvent)
        {

            int hc = typeof(T).GetHashCode();

            if (allDataDic.ContainsKey(hc))
            {
                if (operationEvent != null)
                {
                    operationEvent(allDataDic[hc] as List<T>);
                }
            }

        }

        public T FindByTable<T>(Predicate<T> match)
        {
            T t = default(T);
            OperationTable<T>(list => {
                t = list.Find(match);
            });
            return t;
        }






        private DataCenterMain()
        {
            Init();
        }

        public static DataCenterMain Create()
        {
            if (Insance == null)
            {
                Insance = new DataCenterMain();
            }
            return Insance;
        }

    }
}
