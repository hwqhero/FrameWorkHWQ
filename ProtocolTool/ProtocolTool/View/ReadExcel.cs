using NPOI.SS.UserModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace ProtocolTool.View
{
    public partial class ReadExcel : Form
    {
        object myobj;
        MethodInfo getList;
        Dictionary<int, DataSet> dsDic = new Dictionary<int, DataSet>();
        Assembly cr;
        public ReadExcel()
        {
            InitializeComponent();
        }

        private void ReadExcel_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Effect == DragDropEffects.Link)
            {
                string path = ((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();
                string ex = Path.GetExtension(path);
                if (ex == ".dll")
                {
                    if (File.Exists(path))
                    {
                        textBox1.Text = path;
                        Read(Assembly.LoadFrom(path));
                        //byte[] temp = File.ReadAllBytes(path);
                        //cd.Load(temp);
                        //textBox5.Text = cd.nameSpcae;
                        //ResetTreeView();
                    }
                    //textBox6.Text = path;
                }
            }
        }

        private bool ReadParent(Type t, string name)
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

        private void Read(Assembly aaa)
        {
            cr = aaa;
            treeView1.Nodes.Clear();
            Type c = aaa.GetType("ConfigData.ConfigManager", true);
            Type loadType = aaa.GetType("ConfigData.LoadType", true);
            FieldInfo dic = c.GetField("allDataList", BindingFlags.Static | BindingFlags.NonPublic);
            myobj = dic.GetValue(null);
            MethodInfo mi = c.GetMethod("Add", BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            MethodInfo to = c.GetMethod("ToBinary", BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            MethodInfo getCode = loadType.GetMethod("GetCode", BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            getList = c.GetMethod("GetList", BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public, null, new Type[] { typeof(Type) }, null);
            foreach (Type t in aaa.GetTypes())
            {
                if (!t.IsAbstract && ReadParent(t, "ConfigMetaData"))
                {
                    IList list = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(t));
                    if (File.Exists("Xlsx/" + t.Name + ".xlsx"))
                    {
                        DataSet ds = ExcelToTableForXLSX("Xlsx/" + t.Name + ".xlsx");
                        dsDic[int.Parse(getCode.Invoke(null, new object[] { t.Name }).ToString())] = ds;
                        treeView1.Nodes.Add(getCode.Invoke(null, new object[] { t.Name }).ToString(), t.Name);
                        for (int i = 1; i < ds.Tables[0].Rows.Count; i++)
                        {
                            object obj = Activator.CreateInstance(t);
                            int j = 0;
                            List<FieldInfo> tempList = new List<FieldInfo>();
                            if (!t.BaseType.Name.Equals("ConfigMetaData"))
                            {
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
                                        if (fi.FieldType == typeof(bool))
                                        {
                                            fi.SetValue(obj, Convert.ChangeType(int.Parse(@value.ToString()), fi.FieldType));
                                        }
                                        else
                                        {
                                            fi.SetValue(obj, Convert.ChangeType(@value, fi.FieldType));
                                        }

                                    }
                                    catch (Exception ex)
                                    {
                                        string m = ex.Message;
                                        //Console.WriteLine(@value + "<-->" + fi.FieldType + "<--->" + t.Name);
                                    }
                                    j++;
                                }
                            }
                            list.Add(obj);
                        }
                        mi.Invoke(null, new object[] { list, int.Parse(getCode.Invoke(null, new object[] { t.Name }).ToString()) });
                    }
                    else if (File.Exists("Xlsx/" + t.Name + ".csv"))
                    {
                        string[] lines = File.ReadAllLines("Xlsx/" + t.Name + ".csv");
                        DataSet ds = CSVToTable("Xlsx/" + t.Name + ".csv");
                        treeView1.Nodes.Add(getCode.Invoke(null, new object[] { t.Name }).ToString(), t.Name);
                        dsDic[int.Parse(getCode.Invoke(null, new object[] { t.Name }).ToString())] = ds;
                        for (int i = 1; i < lines.Length; i++)
                        {
                            object obj = Activator.CreateInstance(t);
                            string[] fieldValueList = lines[i].Split(',');
                            List<FieldInfo> tempList = new List<FieldInfo>();
                            if (!t.BaseType.Name.Equals("ConfigMetaData"))
                            {
                                FieldInfo[] fiListList = t.BaseType.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
                                tempList.AddRange(fiListList);
                            }
                            tempList.AddRange(t.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly));
                            int j = 0;
                            foreach (FieldInfo fi in tempList)
                            {
                                if (fi.GetCustomAttributes(false).Length == 0)
                                {
                                    if (fi.FieldType == typeof(bool))
                                    {
                                        fi.SetValue(obj, Convert.ChangeType(int.Parse(fieldValueList[j]), fi.FieldType));
                                    }
                                    else
                                    {
                                        fi.SetValue(obj, Convert.ChangeType(fieldValueList[j], fi.FieldType));
                                    }

                                    j++;
                                }

                            }
                            list.Add(obj);
                        }
                        mi.Invoke(null, new object[] { list, int.Parse(getCode.Invoke(null, new object[] { t.Name }).ToString()) });
                    }
                }
            }
            byte[] bbb = to.Invoke(null, null) as byte[];
            Console.WriteLine(myobj);
            File.WriteAllBytes("Data/ConfigData.hwq", bbb);
        }


        public DataSet CSVToTable(string file)
        {
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            string[] lines = File.ReadAllLines(file);
            string[] titles = lines[0].Split(',');
            for (int i = 0; i < titles.Length; i++)
            {
                DataColumn dc = new DataColumn();
                dc.DataType = typeof(string);
                dt.Columns.Add(dc);

            }
            for (int i = 0; i < lines.Length; i++)
            {
                DataRow dr = dt.NewRow();
                string[] rows = lines[i].Split(',');
                for (int m = 0; m < rows.Length; m++)
                {
                    dr[m] = rows[m];
                }
                dt.Rows.Add(dr);
            }
            ds.Merge(dt);
            return ds;
        }

        public DataSet ExcelToTableForXLSX(string file)
        {
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            using (FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read))
            {

                IWorkbook wb = WorkbookFactory.Create(fs);
                ISheet sheet = wb.GetSheetAt(0);
                IRow title = sheet.GetRow(0);
                int mm = 0;
                for (int i = 0; i < sheet.LastRowNum; i++)
                {
                    IRow row = sheet.GetRow(i);
                    if (row == null)
                        continue;
                    if (mm < row.LastCellNum)
                    {
                        mm = row.LastCellNum;
                    }
                }
                for (int j = 0; j < mm; j++)
                {
                    DataColumn dc = new DataColumn();
                    dc.DataType = typeof(string);
                    dt.Columns.Add(dc);
                }
                for (int i = 1; i <= sheet.LastRowNum; i++)
                {
                    IRow row = sheet.GetRow(i);
                    if (row == null)
                        continue;
                    DataRow dr = dt.NewRow();
                    for (int m = 0; m < mm; m++)
                    {
                        ICell cell = row.GetCell(m);
                        if (cell != null)
                        {
                            cell.SetCellType(CellType.String);
                            dr[m] = cell.StringCellValue;
                        }
                        else
                        {
                            dr[m] = string.Empty;
                        }
                    }
                    dt.Rows.Add(dr);
                }
                
            }
            
            ds.Merge(dt);
            return ds;
        }
        private void ReadExcel_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Link;
            else e.Effect = DragDropEffects.None;
        }

        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node.Name != null)
            {
                Type target = cr.GetType("ConfigData." + e.Node.Text);
                int a = int.Parse(e.Node.Name);
                Dictionary<int, IList> temps = myobj as Dictionary<int, IList>;
                dataGridView1.DataSource = dsDic[a].Tables[0];
                Console.WriteLine(dsDic[a]);
                List<FieldInfo> tempList = new List<FieldInfo>();
                if (!target.BaseType.Name.Equals("ConfigMetaData"))
                {
                    FieldInfo[] fiListList = target.BaseType.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
                    tempList.AddRange(fiListList);
                }
                tempList.AddRange(target.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly));
                DataTable dt = new DataTable();
                foreach (FieldInfo fi in tempList)
                {
                    DataColumn dc = new DataColumn();
                    dc.DataType = fi.FieldType;
                    dc.ColumnName = fi.Name;
                    dt.Columns.Add(dc);
                }
                foreach (object obj in temps[a])
                {
                    DataRow ddddr = dt.NewRow();
                    foreach (FieldInfo fi in tempList)
                    {

                        ddddr[fi.Name] = fi.GetValue(obj);
                    }
                    dt.Rows.Add(ddddr);
                }
                dataGridView2.DataSource = dt;
            }
        }
    }
}
