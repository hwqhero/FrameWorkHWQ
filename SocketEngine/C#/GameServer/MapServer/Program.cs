using org.critterai.nav;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MapServerEngine
{
    class Program
    {



        static void Main(string[] args)
        {
            new MapServerEngine().Start();
            Console.ReadLine();
        }
    }
}
