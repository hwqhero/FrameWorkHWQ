using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConverterToBin
{
    class ReadExcel : ReadData
    {
        public static DataSet Load1(string path)
        {
            string strConn = "Provider=Microsoft.ACE.OLEDB.12.0;" + "Data Source=" + path + ";" + "Extended Properties=\"Excel 12.0;HDR=YES\"";
            OleDbConnection conn = new OleDbConnection(strConn);
           
            conn.Open();
            DataTable dt = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { });
            object dc = dt.Rows[0][2];
            string strExcel = "";
            OleDbDataAdapter myCommand = null;
            DataSet ds = null;
            strExcel = "select * from ["+dc.ToString()+"]";
            myCommand = new OleDbDataAdapter(strExcel, strConn);
            ds = new DataSet();
            myCommand.Fill(ds);

            conn.Close();
            return ds;
        }
    }
}
