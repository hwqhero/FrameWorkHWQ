using Microsoft.CSharp;
using MySql.Data.MySqlClient;
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

class DataData
{
    /// <summary>
    /// 
    /// </summary>
    public string f_name;
    public string f_COMMENT;
}

    class MySqlCode
    {
        private static string conn = "Database='project';Data Source='hwqhero.vicp.cc';User Id='root';Password='project';charset='utf8';pooling=true";
        private MySqlConnection connection;

        public MySqlCode()
        {
            connection = new MySqlConnection(conn);
            connection.Open();
            MySqlCommand mc = connection.CreateCommand();
            mc.CommandText = "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'project'";
            MySqlDataReader reader = mc.ExecuteReader();
            List<string> tableName = new List<string>();
            Dictionary<string, List<DataData>> c = new Dictionary<string, List<DataData>>();
            StringBuilder sb = new StringBuilder();
            while (reader.Read())
            {
                if (reader.HasRows)
                {
                    tableName.Add(reader.GetString(0));
         
      
                }
            }
            reader.Close();

          


            foreach (string s in tableName)
            {
                CodeNamespace cn = NewNameSpace("Entity", "System", "System.IO", "System.Text", "System.Collections.Generic", "System.Collections");

                c.Add(s, new List<DataData>());
                GetRead("select  COLUMN_NAME,COLUMN_COMMENT   from Information_schema.columns  where table_Name = '" + s + "';", obj =>
                {
                    c[s].Add(new DataData() {
                         f_name = obj.GetString(0),
                         f_COMMENT = obj.GetString(1)
                    });
                });

                string className = GenVarName(s);
                CodeTypeDeclaration customerclass = new CodeTypeDeclaration(className);
                customerclass.IsClass = true;
                customerclass.TypeAttributes = TypeAttributes.Public;
                cn.Types.Add(customerclass);

                MySqlCommand mm = connection.CreateCommand();
                mm.CommandText = "select  *  from " + s;
                MySqlDataReader reader1 = mm.ExecuteReader();

                CodeMemberMethod AllSql = new CodeMemberMethod();
                AllSql.Name = "TableName";
                AllSql.Attributes =  MemberAttributes.Private | MemberAttributes.Static;
                AllSql.ReturnType = new CodeTypeReference(typeof(string));
                AllSql.Statements.Add(new CodeSnippetExpression("return \"" + s+"\""));
                   
                customerclass.Members.Add(AllSql);
                foreach (DataData mys in c[s])
                {
                    Type t = reader1.GetFieldType(mys.f_name);
                    CodeMemberField cmf = new CodeMemberField();
                    cmf.Attributes = MemberAttributes.Private;
                    cmf.Name = mys.f_name;
                    cmf.Type = new CodeTypeReference(t);
                    string vvvv = GenVarName(mys.f_name);
                    CodeMemberProperty property = new CodeMemberProperty();
                    property.Name = vvvv;
                    if(!string.IsNullOrEmpty(mys.f_COMMENT))
                    property.Comments.AddRange(new CodeCommentStatementCollection() {
                        new CodeCommentStatement("<summary>",true),
                        new CodeCommentStatement(mys.f_COMMENT, true),
                       new CodeCommentStatement("</summary>", true)
                    });
                    property.Attributes = MemberAttributes.Public | MemberAttributes.Final;
                    property.Type = new CodeTypeReference(t);
                    property.GetStatements.Add(new CodeMethodReturnStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), mys.f_name)));
                    property.SetStatements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), mys.f_name), new CodePropertySetValueReferenceExpression()));
                    customerclass.Members.Add(cmf);
                    customerclass.Members.Add(property);
                }
                //for (int i = 0; i < reader1.FieldCount; i++)
                //{
                //    Type t = reader1.GetFieldType(i);
                //    CodeMemberField cmf = new CodeMemberField();
                //    cmf.Attributes = MemberAttributes.Private;
                //    cmf.Name = c[s][i];
                //    cmf.Type = new CodeTypeReference(t);
                //    string vvvv = GenVarName(c[s][i]);
                //    CodeMemberProperty property = new CodeMemberProperty();
                //    property.Name = vvvv;
                //    property.Attributes = MemberAttributes.Public | MemberAttributes.Final;
                //    property.Type = new CodeTypeReference(t);
                //    property.GetStatements.Add(new CodeMethodReturnStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), c[s][i])));
                //    property.SetStatements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), c[s][i]), new CodePropertySetValueReferenceExpression()));
                //    customerclass.Members.Add(cmf);
                //    customerclass.Members.Add(property);
                //}
                StreamWriter sw = new StreamWriter(@".\\TableEntity\\" + className + ".cs");
                new CSharpCodeProvider().GenerateCodeFromNamespace(cn, sw, new CodeGeneratorOptions()
                {
                    BracingStyle = "C",
                    BlankLinesBetweenMembers = true,
                });
                sw.Flush();
                sw.Close();
                reader1.Close();
            }


    
        }

        private CodeNamespace NewNameSpace(string newNameSpace, params string[] importNameSpace)
        {
            CodeNamespace cn = new CodeNamespace(newNameSpace);
            foreach (string s in importNameSpace)
            {
                cn.Imports.Add(new CodeNamespaceImport(s));
            }
            return cn;
        }


        private void GetRead(string sql, System.Action<MySqlDataReader> operation)
        {
            MySqlCommand mm = connection.CreateCommand();
            mm.CommandText = sql;
            MySqlDataReader reader = mm.ExecuteReader();
            while (reader.Read())
            {
                if (reader.HasRows)
                {
                    if (operation != null)
                        operation(reader);
                }
            }
            reader.Close();
        }


        /// <summary>
        /// 将数据库中变量名改为驼峰命名
        /// 如 user_name 改为 UserName
        /// </summary>
        /// <param name="name">变量名</param>
        /// <returns></returns>
        public string GenVarName(string name)
        {
            string first = name.Substring(0, 1);
            name = name.Substring(1, name.Length - 1);
            name = first.ToUpper() + name;

            int index = name.IndexOf("_");
            while (index != -1)
            {
                if (name.Length >= index + 2)
                {
                    first = name.Substring(index + 1, 1);
                    string start = name.Substring(0, index);
                    string end = name.Substring(index + 2, name.Length - index - 2);
                    name = start + first.ToUpper() + end;

                    index = name.IndexOf("_");
                }
            }

            name = name.Replace("_", "");

            return name;
        }


    }

