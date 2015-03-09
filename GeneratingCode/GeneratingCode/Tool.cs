using Microsoft.CSharp;
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace GeneratingCode
{


    class Tool
    {

        /// <summary>
        /// 读取文件
        /// </summary>
        /// <param name="filepath">文件路径</param>
        /// <returns></returns>
        public static string ReadFileToString(string filepath)
        {
            string t = string.Empty;
            StreamReader sr1 = new StreamReader(filepath);
            t = sr1.ReadToEnd();
            sr1.Close();
            return t;
        }

        public static Assembly LoadAssembly(CompilerParameters cp, string path)
        {
            CSharpCodeProvider cs = new CSharpCodeProvider();
            CompilerResults cr = cs.CompileAssemblyFromFile(cp, path);
            foreach (CompilerError ce in cr.Errors)
            {
                Console.WriteLine(ce);
            }
            return cr.CompiledAssembly;
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

    }
}
