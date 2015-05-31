using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConfigData;
namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            ConfigManager.Load(File.ReadAllBytes("ConfigData.hwq"));
            List<Chapter> temp = ConfigManager.GetList<Chapter>();
            Console.WriteLine(temp); 
        }
    }
}
