using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileServer
{
    public class MyFile
    {
        public string filePath;
        public string fileName;
        public byte[] datas;

        public void Load(string filePath,string fileName)
        {
            this.filePath = filePath;
            this.fileName = fileName;
            datas = File.ReadAllBytes(filePath);
        }
    }
}
