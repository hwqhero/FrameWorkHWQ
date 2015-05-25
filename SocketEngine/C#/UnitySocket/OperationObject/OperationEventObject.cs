using System;
using UnitySocket.Client;


namespace UnitySocket.OperationObject
{
    internal class OperationEventObject
    {
        private Action<IProtocol> operationEvent;

        public OperationEventObject(Action<IProtocol> e)
        {
            operationEvent = e;
        }

        public void Operation(IProtocol od)
        {
            if (operationEvent != null)
            {
                operationEvent(od);
            }
        }
    }
}
