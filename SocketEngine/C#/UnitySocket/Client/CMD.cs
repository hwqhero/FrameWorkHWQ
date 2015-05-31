using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnitySocket.Client
{
    public class CMD :System.Attribute
    {
        public byte m;
        public byte s;
        public bool isUnityMainThread;
        public CMD(byte m, byte s, bool isUnityMainThread)
        {
            this.m = m;
            this.s = s;
            this.isUnityMainThread = isUnityMainThread;
        }
    }
}
