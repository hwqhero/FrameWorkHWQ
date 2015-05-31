using ServerEngine.ServerSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServerEngine.OperationObject;
using ServerEngine.Core;

namespace FileServer
{
    class FileSystem : BaseSystem
    {
        [SystemCMDAttr(1,1)]
        private void ReadFileList(IProtocol p, SocketUser su)
        {
            object obj = p.GetObject();
        }
    }
}
