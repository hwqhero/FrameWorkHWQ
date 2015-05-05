using Microsoft.CSharp;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.IO;
using System.Reflection;
namespace GeneratingCode
{
    public class GenerationCodeUnit
    {
        private GenerationCodeUnit(){}
        public static CodeCompileUnit CreateCodeCompileUnit()
        {
            return new CodeCompileUnit();
        }

        public static CodeNamespace NewNameSpace(string newNameSpace, params string[] importNameSpace)
        {
            CodeNamespace cn = new CodeNamespace(newNameSpace);
            foreach (string s in importNameSpace)
            {
                cn.Imports.Add(new CodeNamespaceImport(s));
            }
            return cn;
        }


        public static CodeTypeDeclaration CreateClass(string name)
        {
            CodeTypeDeclaration c = new CodeTypeDeclaration(name);
            c.IsClass = true;
            c.TypeAttributes = TypeAttributes.Public;
            return c;
        }


        public static CodeMemberMethod CreateMethod(string mehtodName, CodeTypeReference returnType, string mehtodStatement, params CodeParameterDeclarationExpression[] plist)
        {
            CodeMemberMethod c = new CodeMemberMethod();
            c.Name = mehtodName;
            c.Attributes = MemberAttributes.Override | MemberAttributes.Assembly;
            if (returnType != null)
                c.ReturnType = returnType;
            c.Statements.Add(new CodeSnippetStatement(mehtodStatement));
            c.Parameters.AddRange(plist);
            return c;
        }
        public static CompilerResults GenerationUnit(CodeCompileUnit ccu, string outPaht, string entityString, string savePathDll)
        {
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
            mydata = mydata.Insert(mydata.LastIndexOf('}'), entityString);
            StreamWriter sw1 = new StreamWriter(outPaht);
            sw1.Write(mydata);
            sw1.Flush();
            sw1.Close();
            CSharpCodeProvider provider = new CSharpCodeProvider();
            CompilerParameters cp = new CompilerParameters(new string[] { "mscorlib.dll", "System.Data.dll" }, savePathDll, false);
            cp.GenerateExecutable = false;
            cp.CompilerOptions = "/doc:" + savePathDll.Replace(Path.GetExtension(savePathDll), ".xml");
            cp.IncludeDebugInformation = true;
            return provider.CompileAssemblyFromSource(cp, mydata);
        }
    }
}
