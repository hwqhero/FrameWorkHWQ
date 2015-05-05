using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.CodeDom;
using System.Reflection;
using Microsoft.CSharp;
using System.CodeDom.Compiler;

namespace ProtocolTool.Model
{
    [Serializable]
    public class DataClass
    {
        private StringBuilder tempsb = new StringBuilder();
        private string outPaht = @".\\ConfigEntity\\Out\\Config.cs";
        private string entityString;
        private const string tabstr = "\t";
        public string nameSpcae;
        public List<CustomClass> classList;

        public void ToBinary(string path)
        {
            FileStream fs = new FileStream(path, FileMode.OpenOrCreate);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, this);
            fs.Close();
        }

        public void Load(byte[] tempList)
        {
            MemoryStream ms = new MemoryStream(tempList);
            BinaryFormatter bf = new BinaryFormatter();
            DataClass dc = (DataClass)bf.Deserialize(ms);
            nameSpcae = dc.nameSpcae;
            classList = dc.classList;
        }

        public void GenerateCode(string savePathDll)
        {
            Assembly aaaa = typeof(int).Assembly;
            if (string.IsNullOrEmpty(savePathDll))
                savePathDll = "HWQConfigData.dll";
            StreamReader sr1 = new StreamReader(@".\\ConfigEntity\\Config\\ConfigString.cs");
            entityString = sr1.ReadToEnd();
            sr1.Close();
            CodeCompileUnit ccu = GeneratingCode.GenerationCodeUnit.CreateCodeCompileUnit();
            CodeNamespace cn = NewNameSpace(nameSpcae, "System", "System.IO", "System.Text", "System.Collections.Generic", "System.Collections");
            ccu.Namespaces.Add(cn);

            StringBuilder create = new StringBuilder();
            StringBuilder createre = new StringBuilder();
            CodeTypeDeclaration attrClass = CreateClass("SkipAttr");
            attrClass.BaseTypes.Add("System.Attribute");
            cn.Types.Add(attrClass);
            StringBuilder init = new StringBuilder();
            createre.AppendLine("int i = 0;");
            createre.AppendLine("switch(name){");
            create.AppendLine("ConfigMetaData cmd = null;");
            create.AppendLine("switch(hc){");
            foreach (CustomClass cc in classList)
            {
                create.AppendLine("case " + cc.GetHashCode() + " :");
                create.AppendLine("cmd = new " + cc.name + "();");
                create.AppendLine("break;");
                createre.AppendLine("case \"" + cc.name + "\" :");
                createre.AppendLine("i = " + cc.GetHashCode() + ";");
                createre.AppendLine("break;");
                CodeTypeDeclaration customerclass = CreateClass(cc.name);
                customerclass.Comments.Add(new CodeCommentStatement("<summary>", true));
                if (!string.IsNullOrEmpty(cc.desc))
                    customerclass.Comments.Add(new CodeCommentStatement(cc.desc, true));
                customerclass.Comments.Add(new CodeCommentStatement("</summary>", true));
                cn.Types.Add(customerclass);
                customerclass.Members.Add(CreateMethod("Clone", new CodeTypeReference("ConfigMetaData"), "return new " + cc.name + "();"));
                customerclass.Members.Add(CreateMethod("CustomCode", new CodeTypeReference(typeof(int)), "return " + cc.GetHashCode() + ";"));
                StringBuilder SMethod = new StringBuilder();
                SMethod.AppendLine("");
                StringBuilder DMethod = new StringBuilder();
                DMethod.AppendLine("");
                if (!string.IsNullOrEmpty(cc.baseName))
                {
                    SMethod.AppendLine("base.Serialize(bw);");
                    DMethod.AppendLine("base.Deserialize(br);");
                    customerclass.BaseTypes.Add(cc.baseName);
                }
                else
                {
                    customerclass.BaseTypes.Add("ConfigMetaData");
                }
                bool isInist = false;
                List<string> tempList = new List<string>();
                if (!string.IsNullOrEmpty(cc.baseName))
                {
                    CustomClass ccccc = classList.Find(obj => obj.name == cc.baseName);
                    if (ccccc != null)
                    {
                        foreach (CustomField cf in ccccc.fieldList)
                        {
                            if (cf.isRelation)
                            {
                                foreach (CustomRelation cr in cf.relationList)
                                {
                                    if (!tempList.Contains(cf.type))
                                    {
                                        tempList.Add(cf.type);
                                    }
                                }
                            }

                        }
                    }
                }
                foreach (CustomField cf in cc.fieldList)
                {
                    if (cf.isRelation)
                    {
                        foreach (CustomRelation cr in cf.relationList)
                        {
                            if (!tempList.Contains(cf.type))
                            {
                                tempList.Add(cf.type);
                            }
                        }
                    }

                }
                if (tempList.Count > 0)
                {
                    init.AppendLine("foreach(" + cc.name + " temp in ConfigManager.GetList<" + cc.name + ">())");
                    init.AppendLine("{");

                    foreach (string k in tempList)
                    {
                        List<CustomField> cfList = new List<CustomField>();
                        if (!string.IsNullOrEmpty(cc.baseName))
                        {
                            CustomClass baseClass = classList.Find(obj => obj.name == cc.baseName);
                            cfList.AddRange(baseClass.fieldList.FindAll(obj => obj.type == k));
                        }
                        else
                        {
                            cfList.AddRange(cc.fieldList.FindAll(obj => obj.type == k));
                        }
                        foreach (CustomField cf in cfList)
                        {
                            //if (!string.IsNullOrEmpty(cc.baseName))
                            //{
                            //    CustomClass baseClass = classList.Find(obj => obj.name == cc.baseName);
                            //    cf = baseClass.fieldList.Find(obj => obj.type == k);
                            //}
                            //else
                            //{
                            //    cf = cc.fieldList.Find(obj => obj.type == k);
                            //}
                            //if (!cf.isList)
                            //{
                            //    Console.WriteLine("a");
                            //}
                            List<CustomRelation> relationList = new List<CustomRelation>();
                            List<CustomRelation> relationList1 = new List<CustomRelation>();
                            relationList.AddRange(cf.relationList);
                            relationList1.AddRange(cf.relationList);
                            bool isString = false;
                            foreach (CustomRelation cr in relationList)
                            {
                                CustomField myFiled = cc.fieldList.Find(obj => obj.name == cr.myFieldName);
                                if (!string.IsNullOrEmpty(cc.baseName))
                                {
                                    CustomClass baseClass = classList.Find(obj => obj.name == cc.baseName);
                                    myFiled = baseClass.fieldList.Find(obj => obj.name == cr.myFieldName);
                                }
                                if (myFiled.type == "String")
                                {
                                    init.AppendLine("string[] sList = temp." + cr.myFieldName + ".Split('&');");
                                    relationList1.Remove(cr);
                                    isString = true;
                                }
                            }

                            relationList.Clear();
                            relationList.AddRange(relationList1);
                            init.Append("temp." + cf.name + "= ConfigManager.GetList<" + cf.type + ">()");
                            if (cf.isList)
                            {
                                init.AppendLine(".FindAll(");
                            }
                            else
                            {
                                init.Append(".Find(");
                            }
                            if (isString)
                            {
                                init.AppendLine("delegate(" + cf.type + " obj){ ");
                                init.Append(" foreach (string mys in sList){ int id = int.Parse(mys);return ");
                            }
                            else
                            {
                                init.AppendLine("delegate(" + cf.type + " obj){ ");
                                init.Append(" return ");
                            }
                            foreach (CustomRelation cr in cf.relationList)
                            {
                                CustomField myFiled = cc.fieldList.Find(obj => obj.name == cr.myFieldName);
                                if (!string.IsNullOrEmpty(cc.baseName))
                                {
                                    CustomClass baseClass = classList.Find(obj => obj.name == cc.baseName);
                                    myFiled = baseClass.fieldList.Find(obj => obj.name == cr.myFieldName);
                                }
                                if (myFiled.type == "String")
                                {
                                    init.Append("obj." + cr.targetFieldName + " == id && ");
                                }
                                else
                                {
                                    init.Append("obj." + cr.targetFieldName + " == temp." + cr.myFieldName + " && ");
                                }
                            }
                            init.Length -= 3;
                            if (isString)
                            {
                                init.Append(";} ");
                                init.AppendLine();
                                init.Append("return false;});");
                            }
                            else
                            {
                                init.AppendLine(";});");
                            }

                        }
                    }
                    init.AppendLine("}");
                }
              
                

                

                foreach (CustomField cf in cc.fieldList)
                {

                    CodeMemberField cmf = new CodeMemberField();
                    cmf.Attributes = MemberAttributes.Assembly;
                    cmf.Name = cf.name;

                    if (cf.isRelation)
                    {
                        cmf.CustomAttributes.Add(new CodeAttributeDeclaration("SkipAttr"));
                    }

                    if (cf.isList)
                    {
                        cmf.Type = new CodeTypeReference("List<" + cf.type + ">");
                    }
                    else
                    {
                        if (cf.isRelation)
                        {
                            cmf.Type = new CodeTypeReference(cf.type);
                        }
                        else
                            cmf.Type = new CodeTypeReference(GetTypeByAsss(aaaa, cf.type));
                    }
                    CodeMemberProperty property = new CodeMemberProperty();
                    string otherFiName = cf.name.Substring(1);
                    property.Name = UpString(cf.name);
                    property.Attributes = MemberAttributes.Public | MemberAttributes.Final;
                    property.Comments.Add(new CodeCommentStatement("<summary>", true));
                    if (!string.IsNullOrEmpty(cf.desc))
                        property.Comments.Add(new CodeCommentStatement(cf.desc, true));
                    property.Comments.Add(new CodeCommentStatement("</summary>", true));
                    property.Type = cmf.Type;
                    property.GetStatements.Add(new CodeMethodReturnStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), cf.name)));
                    customerclass.Members.Add(cmf);
                    customerclass.Members.Add(property);
                    if (!isInist)
                    {
                        ReadSerialize(SMethod, cf);
                        ReadDeserialize(DMethod, cf);
                    }
                }
                if (isInist)
                    init.AppendLine("}");
                customerclass.Members.Add(CreateMethod("Serialize", null, SMethod.ToString(), new CodeParameterDeclarationExpression("BinaryWriter", "bw")));
                customerclass.Members.Add(CreateMethod("Deserialize", null, DMethod.ToString(), new CodeParameterDeclarationExpression("BinaryReader", "br")));
            }
            create.AppendLine("}");
            create.AppendLine("return cmd;");
            createre.AppendLine("}");
            createre.AppendLine("return i;");

            CodeTypeDeclaration ctd = new CodeTypeDeclaration("LoadType");
            ctd.TypeAttributes = TypeAttributes.NestedAssembly;
            CodeMemberMethod ccc = CreateMethod("Get", new CodeTypeReference("ConfigMetaData"), create.ToString(), new CodeParameterDeclarationExpression(typeof(int), "hc"));
            ccc.Attributes = MemberAttributes.Assembly | MemberAttributes.Static;
            ctd.Members.Add(ccc);

            CodeMemberMethod ccc1 = CreateMethod("GetCode", new CodeTypeReference(typeof(int)), createre.ToString(), new CodeParameterDeclarationExpression(typeof(string), "name"));
            ccc1.Attributes = MemberAttributes.Assembly | MemberAttributes.Static;
            ctd.Members.Add(ccc1);
            if (init.Length > 0)
            {
                Console.WriteLine("");
            }
            CodeMemberMethod ccc13333333 = CreateMethod("Init", null, init.ToString());
            ccc13333333.Attributes = MemberAttributes.Assembly | MemberAttributes.Static;
            ctd.Members.Add(ccc13333333);
            cn.Types.Add(ctd);
            CompilerResults ce = GeneratingCode.GenerationCodeUnit.GenerationUnit(ccu, outPaht, entityString, savePathDll);
           foreach (CompilerError cee in ce.Errors)
           {
               Console.WriteLine(cee.ErrorText);
           }
        }

        private string UpString(string s)
        {
            string other = s.Substring(1);
            return s.Substring(0, 1).ToUpper() + other;
        }


        private Type GetTypeByAsss(Assembly aaaa, string name)
        {
            foreach (Type t in aaaa.GetTypes())
            {
                if (t.Name == name)
                {
                    return t;
                }
            }
            return null;
        }

        private CodeNamespace NewNameSpace(string newNameSpace, params string[] importNameSpace)
        {
            return GeneratingCode.GenerationCodeUnit.NewNameSpace(newNameSpace, importNameSpace);
        }


        private CodeTypeDeclaration CreateClass(string name)
        {
            return GeneratingCode.GenerationCodeUnit.CreateClass(name);
        }


        private CodeMemberMethod CreateMethod(string mehtodName, CodeTypeReference returnType, string mehtodStatement, params CodeParameterDeclarationExpression[] plist)
        {
            CodeMemberMethod c = new CodeMemberMethod();
            c.Name = mehtodName;
            c.Attributes = MemberAttributes.Override | MemberAttributes.Assembly;
            if (returnType != null)
                c.ReturnType = returnType;
            c.Statements.Add(new CodeSnippetStatement(mehtodStatement));
            c.Parameters.AddRange(plist);
            return GeneratingCode.GenerationCodeUnit.CreateMethod(mehtodName, returnType, mehtodStatement, plist);
        }


        private void ReadSerialize(StringBuilder sb, CustomField cf)
        {
            if (cf.isList)
            {
                spacejoin(sb, "if(" + cf.name + " == null)");
                tempsb.Length = 0;
                spacejoin(tempsb, "bw.Write((short)0);", 4);
                FlowerSlogan(sb, tempsb);
                spacejoin(sb, "else");
                FlowerSloganLeft(sb);
                spacejoin(sb, "bw.Write((short)" + cf.name + ".Count);", 4);
                spacejoin(sb, "foreach (" + cf.type + " temp in " + cf.name + ")", 4);
                tempsb.Length = 0;

                if (cf.isRelation)
                {
                    spacejoin(tempsb, "temp.Serialize(bw);", 3);
                }
                else
                {
                    spacejoin(tempsb, "bw.Write(temp);", 3);
                }
                FlowerSlogan(sb, tempsb);
                FlowerSloganRigth(sb);
            }
            else if (cf.type == "string")
            {
                spacejoin(sb, "bw.Write(" + cf.name + ");");
            }
            else if (cf.isRelation)
            {
                spacejoin(sb, "if(" + cf.name + "!=null)");
                tempsb.Length = 0;
                spacejoin(tempsb, "bw.Write(true);", 3);
                spacejoin(tempsb, cf.name + ".Serialize(bw);", 3);
                FlowerSlogan(sb, tempsb);
                spacejoin(sb, "else");
                tempsb.Length = 0;
                spacejoin(tempsb, "bw.Write(false);", 3);
                FlowerSlogan(sb, tempsb);
            }
            else if (cf.type == "DateTime")
            {
                spacejoin(sb, "bw.Write(" + cf.name + ".ToBinary());");
            }
            else
            {
                spacejoin(sb, "bw.Write(" + cf.name + ");");
            }
        }



        private void ReadDeserialize(StringBuilder sb, CustomField cf)
        {
            if (cf.isList)
            {
                spacejoin(sb, "int " + cf.name + "Count = br.ReadInt16();");
                spacejoin(sb, cf.name + " = new List<" + cf.type + ">();");
                spacejoin(sb, "for(int i = 0;i<" + cf.name + "Count;i++)");
                FlowerSloganLeft(sb);
                if (cf.isRelation)
                {
                    spacejoin(sb, "if (br.ReadBoolean())");
                    tempsb.Length = 0;
                    spacejoin(tempsb, cf.type + "  obj = new " + cf.type + "();", 3);
                    spacejoin(tempsb, "obj.Deserialize(br);", 3);
                    spacejoin(tempsb, cf.name + ".Add(obj);", 3);
                    FlowerSlogan(sb, tempsb);
                }
                else
                {
                    spacejoin(sb, cf.name + ".Add(br.Read" + cf.type + "());", 3);
                }
                FlowerSloganRigth(sb);
            }
            else if (cf.type == "DateTime")
            {
                spacejoin(sb, cf.name + " =DateTime.FromBinary(br.ReadInt64());");
            }
            else if (cf.type == "string")
            {
                spacejoin(sb, cf.name + " = br.ReadString();");
            }
            else if (cf.isRelation)
            {
                spacejoin(sb, "if(br.ReadBoolean())");
                tempsb.Length = 0;
                spacejoin(tempsb, cf.name + " = new " + cf.type + "();", 3);
                spacejoin(tempsb, cf.name + ".Deserialize(br);", 4);
                FlowerSlogan(sb, tempsb);
            }
            else
            {
                spacejoin(sb, Primary(cf.name, cf.type));
            }
        }

        private string Primary(string fieldName, string TypeName)
        {
            return fieldName + " = br.Read" + TypeName + "();";
        }


        private void spacejoin(StringBuilder sb, string text, int count = 3)
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
            sb.Append(tabstr + tabstr + tabstr);
            sb.AppendLine("{");
        }

        /// <summary>
        /// 右边花括号
        /// </summary>
        /// <param name="sb"></param>
        private void FlowerSloganRigth(StringBuilder sb)
        {
            sb.Append(tabstr + tabstr + tabstr);
            sb.AppendLine("}");
        }

    }
}
