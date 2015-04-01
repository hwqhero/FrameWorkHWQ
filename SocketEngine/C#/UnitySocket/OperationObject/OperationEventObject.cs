using NetEntityHWQ;
using System;
using UnitySocket.Client;


namespace UnitySocket.OperationObject
{
    internal class OperationEventObject
    {
        private Action<OperationData> operationEvent;

        public OperationEventObject(Action<OperationData> e)
        {
            operationEvent = e;
        }

        public void Operation(OperationData od)
        {
            if (operationEvent != null)
            {
                operationEvent(od);
            }
        }

        
    }
}
