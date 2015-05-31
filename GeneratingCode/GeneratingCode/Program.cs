
using GeneratingCode;
using Microsoft.CSharp;
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
namespace MyConvert
{
    class Program
    {
        private const string tabstr = "\t";
        private StringBuilder tempsb = new StringBuilder();
        private string outPaht = @".\\NetEntity\\Out\\Net.cs";
        private string NetEntityString;
        private string TableEntityString;
        private Assembly netAssembly;
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            //foreach (Type t in Assembly.GetExecutingAssembly().GetTypes())
            //{
            //    Console.WriteLine(t.Name);
            //}
            //return;
            //Console.WriteLine("".Length +"");
            //return;
            //Stopwatch s = Stopwatch.StartNew();
            //s.Stop();
            // Console.WriteLine(s.Elapsed.TotalMilliseconds);
            Stopwatch s1 = Stopwatch.StartNew();
            new Program().ConvertForm();
            new ConfigCode();
            new MySqlCode();
            s1.Stop();
            Console.WriteLine(s1.Elapsed.TotalMilliseconds);
           
          
            
        }

        private string ReadFileToString(string filePath) 
        {
            string t = string.Empty;
            StreamReader sr1 = new StreamReader(filePath);
            t = sr1.ReadToEnd();
            sr1.Close();
            return t;
        }

        private Assembly LoadAssembly(CompilerParameters cp,string path)
        {
            CSharpCodeProvider cs = new CSharpCodeProvider();
            CompilerResults cr =  cs.CompileAssemblyFromFile(cp, path);
            return cr.CompiledAssembly;
        }

        private void ReadNetFile()
        {
            NetEntityString = ReadFileToString(@".\\NetEntity\\Config\\NetEntityString.cs");
            netAssembly = LoadAssembly(new CompilerParameters(new string[] { "System.dll", "mscorlib.dll" })
            { 
                 IncludeDebugInformation = false, GenerateExecutable = false,GenerateInMemory = true
            }, @".\\NetEntity\\NetEntityList.cs");


            CodeCompileUnit ccu = new CodeCompileUnit();
            CodeNamespace cn = NewNameSpace("NetEntityHWQ", "System", "System.IO", "System.Text", "System.Collections.Generic", "System.Collections");
            ccu.Namespaces.Add(cn);

            StringBuilder createstring = new StringBuilder();
            createstring.AppendLine("BaseNetHWQ data = null;");
            createstring.AppendLine("switch(code){");
            try
            {
                //foreach (Type t in ab.GetTypes())
                //{
                foreach (Type t in netAssembly.GetTypes())
                {
                    if (!t.IsClass || t == typeof(String) || t.IsAbstract)
                        continue;
                    createstring.AppendLine("case " + t.GetHashCode() + ":");
                    createstring.AppendLine("data = new " + t.Name + "();");
                    createstring.AppendLine("break;");
                    CodeTypeDeclaration customerclass = new CodeTypeDeclaration(t.Name);
                    customerclass.IsClass = true;
                    customerclass.TypeAttributes = TypeAttributes.Public;
                    cn.Types.Add(customerclass);

                    CodeMemberMethod CustomCode = new CodeMemberMethod();
                    CustomCode.Name = "CustomCode";
                    CustomCode.Attributes = MemberAttributes.Override | MemberAttributes.Assembly;
                    CustomCode.ReturnType = new CodeTypeReference(typeof(int));
                    CustomCode.Statements.Add(new CodeSnippetExpression("return " + t.GetHashCode() + ""));
                    CodeMemberMethod Serialize = new CodeMemberMethod();
                    Serialize.Name = "Serialize";
                    Serialize.Attributes = MemberAttributes.Override | MemberAttributes.Assembly;
                    Serialize.ReturnType = new CodeTypeReference("List<byte>");

                    customerclass.BaseTypes.Add("BaseNetHWQ");
                    CodeMemberMethod Deserialize = new CodeMemberMethod();
                    Deserialize.Name = "Deserialize";
                    Deserialize.Attributes = MemberAttributes.Override | MemberAttributes.Assembly;
                    Deserialize.Parameters.Add(new CodeParameterDeclarationExpression("BinaryReader", "br"));
                    Serialize.Statements.Add(new CodeSnippetExpression("List<byte> tempList = new List<byte>()"));
                    StringBuilder sb = new StringBuilder();
                    StringBuilder sb2 = new StringBuilder();
                    spacejoin(sb, AddRange("true"));
                    foreach (FieldInfo fi in t.GetFields(BindingFlags.Public | BindingFlags.Instance))
                    {
                        Console.WriteLine(t.Name + "<-------------------->" + fi.Name + "<--->");

                        CodeMemberField cmf = new CodeMemberField();
                        cmf.Attributes = MemberAttributes.Public;
                        cmf.Name = fi.Name;
                        if (fi.FieldType.IsGenericType)
                        {

                            cmf.Type = new CodeTypeReference("List<" + fi.FieldType.GetGenericArguments()[0].Name + ">");
                        }
                        else
                        {

                            cmf.Type = new CodeTypeReference(fi.FieldType.Name);
                        }

                        customerclass.Members.Add(cmf);
                        ReadSerialize(sb, fi);
                        ReadDeserialize(sb2, fi);
                    }
                    spacejoin(sb, "return tempList;");
                    Serialize.Statements.Add(new CodeSnippetStatement(sb.ToString()));
                    Deserialize.Statements.Add(new CodeSnippetStatement(sb2.ToString()));
                    customerclass.Members.Add(Serialize);
                    customerclass.Members.Add(Deserialize);
                    customerclass.Members.Add(CustomCode);
                }

                createstring.AppendLine("}");
                createstring.AppendLine("return data;");
                cn.Types.Add(NewClass("DataFactory", TypeAttributes.NestedAssembly, NewMethod("CreateObject", MemberAttributes.Assembly | MemberAttributes.Static, createstring.ToString(), "BaseNetHWQ", new CodeParameterDeclarationExpression(typeof(int), "code"))));
                StreamWriter sw = new StreamWriter(outPaht);
                new CSharpCodeProvider().GenerateCodeFromCompileUnit(ccu, sw, new CodeGeneratorOptions()
                {
                    BracingStyle = "C",
                    BlankLinesBetweenMembers = true,
                });
                sw.Flush();
                sw.Close();

                StreamReader sr = new StreamReader(outPaht);
                string mydata = sr.ReadToEnd();
                sr.Close();
                mydata = mydata.Insert(mydata.LastIndexOf('}'), NetEntityString);

                StreamWriter sw1 = new StreamWriter(outPaht);
                sw1.Write(mydata);
                sw1.Flush();
                sw1.Close();
   
                CSharpCodeProvider provider = new CSharpCodeProvider();
                CompilerParameters cp = new CompilerParameters(new string[] { "mscorlib.dll", "System.Xml.dll", "System.Data.dll" }, "HWQData.dll", false);
                cp.GenerateExecutable = false;
                CompilerResults cr = provider.CompileAssemblyFromSource(cp, mydata);
                foreach (CompilerError ce in cr.Errors)
                {
                    Console.WriteLine(ce.ErrorText);
                    // MessageBox.Show(ce.ErrorText);
                }
                //File.Delete(@".\\MyData.cs");
                // Application.Exit();
            }
            catch (FileLoadException fle)
            {
                //MessageBox.Show(fle + "");
            }
        }


        /// <summary>
        /// 创建新命名空间
        /// </summary>
        /// <param name="newNameSpace">名称</param>
        /// <param name="importNameSpace">导入的命名空间</param>
        private CodeNamespace NewNameSpace(string newNameSpace, params string[] importNameSpace)
        {
            CodeNamespace cn = new CodeNamespace(newNameSpace);
            foreach (string s in importNameSpace)
            {
                cn.Imports.Add(new CodeNamespaceImport(s));
            }
            return cn;
        }

        private CodeTypeDeclaration NewClass(string className, TypeAttributes ta, params CodeMemberMethod[] methods)
        {
            CodeTypeDeclaration t = new CodeTypeDeclaration(className);
            t.IsClass = true;
            t.TypeAttributes = ta;
            t.Members.AddRange(methods);
            return t;
        }

        private CodeMemberMethod NewMethod(string methodName,MemberAttributes ma, string m,string returnTypeName ,params CodeParameterDeclarationExpression[] parm)
        {
            CodeMemberMethod cmm = new CodeMemberMethod();
            cmm.Statements.Add(new CodeSnippetStatement(m));
            cmm.Name = methodName;
            cmm.Attributes = MemberAttributes.Assembly | MemberAttributes.Static;
            cmm.Parameters.AddRange(parm);
            cmm.ReturnType = new CodeTypeReference(returnTypeName);
            return cmm;
        }


        public void ConvertForm()
        {
            ReadNetFile();
        }



        private void ReadSerialize(StringBuilder sb, FieldInfo fi)
        {
            if (fi.FieldType.IsGenericType)
            {
                spacejoin(sb, "if(" + fi.Name + " == null)");
                tempsb.Length = 0;
                spacejoin(tempsb, AddRange("(short)0"));
                FlowerSlogan(sb, tempsb);
                spacejoin(sb, "else");
                FlowerSloganLeft(sb);
                spacejoin(sb, AddRange("(short)" + fi.Name + ".Count"));
                spacejoin(sb, "foreach (" + fi.FieldType.GetGenericArguments()[0].Name + " temp in " + fi.Name + ")");
                tempsb.Length = 0;

                if (fi.FieldType.GetGenericArguments()[0].IsClass)
                {
                    spacejoin(tempsb, Add("temp.Serialize()"), 3);
                }
                else
                {
                    spacejoin(tempsb, AddRange("temp"), 3);
                }
                FlowerSlogan(sb, tempsb);
                FlowerSloganRigth(sb);
            }
            else if (fi.FieldType == typeof(String))
            {
                spacejoin(sb, "s(tempList," + fi.Name + ");");
            }
            else if (fi.FieldType == typeof(byte))
            {
                spacejoin(sb, AddSingle(fi.Name));
            }
            else if (fi.FieldType.IsClass)
            {
                spacejoin(sb, "if(" + fi.Name + "!=null)");
                tempsb.Length = 0;
                spacejoin(tempsb, AddRange("true"), 3);
                spacejoin(tempsb, Add(fi.Name + ".Serialize()"), 3);
                FlowerSlogan(sb, tempsb);
                spacejoin(sb, "else");
                tempsb.Length = 0;
                spacejoin(tempsb, AddRange("false"), 3);
                FlowerSlogan(sb, tempsb);
            }
            else if (fi.FieldType == typeof(DateTime))
            {
                spacejoin(sb, AddRange(fi.Name + ".ToBinary()"));
            }
            else if (fi.FieldType.IsEnum)
            {

                spacejoin(sb, AddRange("(int)" + fi.Name));
            }
            else
            {
                spacejoin(sb, AddRange(fi.Name));
            }
        }

        private void ReadDeserialize(StringBuilder sb, FieldInfo fi)
        {
            if (fi.FieldType.IsGenericType)
            {

                spacejoin(sb, "int " + fi.Name + "Count = br.ReadInt16();");
                spacejoin(sb, fi.Name + " = new List<" + fi.FieldType.GetGenericArguments()[0].Name + ">();");
                spacejoin(sb, "for(int i = 0;i<" + fi.Name + "Count;i++)");
                FlowerSloganLeft(sb);
                if (fi.FieldType.GetGenericArguments()[0].IsClass)
                {
                    spacejoin(sb, "if (br.ReadBoolean())");
                    tempsb.Length = 0;
                    spacejoin(tempsb, fi.FieldType.GetGenericArguments()[0].Name + "  obj = new " + fi.FieldType.GetGenericArguments()[0].Name + "();", 3);
                    spacejoin(tempsb, "obj.Deserialize(br);", 3);
                    spacejoin(tempsb, fi.Name + ".Add(obj);", 3);
                    FlowerSlogan(sb, tempsb);
                }
                else
                {
                    spacejoin(sb, fi.Name + ".Add(br.Read" + fi.FieldType.GetGenericArguments()[0].Name + "());", 3);
                }
                FlowerSloganRigth(sb);
            }
            else if (fi.FieldType == typeof(DateTime))
            {
                spacejoin(sb, fi.Name + " =DateTime.FromBinary(br.ReadInt64());");
            }
            else if (fi.FieldType == typeof(String))
            {
                spacejoin(sb, fi.Name + " = d(br);");
            }
            else if (fi.FieldType.IsClass)
            {
                spacejoin(sb, "if(br.ReadBoolean())");
                tempsb.Length = 0;
                spacejoin(tempsb, fi.Name + " = new " + fi.FieldType.Name + "();", 3);
                spacejoin(tempsb, fi.Name + ".Deserialize(br);", 3);
                FlowerSlogan(sb, tempsb);
            }
            else if (fi.FieldType.IsEnum)
            {
                spacejoin(sb, fi.Name + " = (" + fi.FieldType.Name + ")br.ReadInt32();");
            }
            else
            {
                spacejoin(sb, Primary(fi.Name, fi.FieldType.Name));
            }
        }

        private string Primary(string fieldName, string TypeName)
        {
            return fieldName + " = br.Read" + TypeName + "();";
        }

        private string AddRange(string fieldName)
        {
            return "tempList.AddRange(BitConverter.GetBytes(" + fieldName + "));";
        }

        private string Add(string fieldName)
        {
            return "tempList.AddRange(" + fieldName + ");";
        }

        private string AddSingle(string fieldName)
        {
            return "tempList.Add(" + fieldName + ");";
        }

        private void spacejoin(StringBuilder sb, string text, int count = 2)
        {
            for (int i = 0; i < count; i++)
            {
                sb.Append(tabstr);
            }
            sb.AppendLine(text);
        }

        private void FlowerSlogan(StringBuilder sb, StringBuilder tempsb)
        {

            FlowerSloganLeft(sb);
            sb.Append(tempsb.ToString());
            FlowerSloganRigth(sb);
        }

        /// <summary>
        /// 左边花括号
        /// </summary>
        /// <param name="sb"></param>
        private void FlowerSloganLeft(StringBuilder sb)
        {
            sb.Append(tabstr + tabstr);
            sb.AppendLine("{");
        }

        /// <summary>
        /// 右边花括号
        /// </summary>
        /// <param name="sb"></param>
        private void FlowerSloganRigth(StringBuilder sb)
        {
            sb.Append(tabstr + tabstr);
            sb.AppendLine("}");
        }

    }
}
