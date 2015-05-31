using System;
using System.Collections.Generic;
namespace UnitySocket.Pool
{
    internal sealed class SendPool
    {
        private List<SendObject> allObject;
        private List<SendObject> availableObject;
        private SendPool()
        {
            
        }

        public void Init()
        {
            allObject = new List<SendObject>();
            availableObject = new List<SendObject>();
            for (int i = 0; i < 100; i++)
            {
                SendObject so = new SendObject();
                allObject.Add(so);
                availableObject.Add(so);
            }
        }

        public static SendPool  Create()
        {
            SendPool s = new SendPool();
            s.Init();
            return s;
        }

        public SendObject Get()
        {
            SendObject so = null;

            if (availableObject.Count > 0)
            {
                so = availableObject[0];
                availableObject.RemoveAt(0);
            }
            else
            {
                so = new SendObject();
                allObject.Add(so);  
            }
            return so;
        }

        public void Recovery(SendObject obj)
        {
            availableObject.Add(obj);
        }
    }
}
