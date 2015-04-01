using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace GameCenter
{
    class Program
    {
        delegate bool ConsoleCtrlDelegate(int dwCtrlType);
        [DllImport("kernel32.dll")]
        private static extern bool SetConsoleCtrlHandler(ConsoleCtrlDelegate HandlerRoutine, bool Add);
        static void Main(string[] args)
        {
            //Process DataCenter = new Process();//Process.Start(@"..\..\..\DataCenter\bin\Debug\DataCenter.exe");
            //DataCenter.StartInfo.FileName = @"..\..\..\DataCenter\bin\Debug\DataCenter.exe";
            //DataCenter.StartInfo.CreateNoWindow = true;
            //DataCenter.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            //DataCenter.Start();


            //SetConsoleCtrlHandler(a =>
            //{
            //    DataCenter.Kill();
            //    return true;
            //}, true);
            //Console.ReadLine();
        }
    }
}
