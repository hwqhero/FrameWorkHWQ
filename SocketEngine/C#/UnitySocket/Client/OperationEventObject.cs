using System;


namespace UnitySocket.Client
{
    internal class OperationEventObject
    {
        private Action<OperationProtocol> operationEvent;
        private bool isUnityMainThread;

        internal bool IsUnityMainThread
        {
            get { return isUnityMainThread; }
            set { isUnityMainThread = value; }
        }


        public OperationEventObject(Action<OperationProtocol> e)
        {
            operationEvent = e;
        }

        public void Operation(OperationProtocol od)
        {
            if (operationEvent != null)
            {
                operationEvent(od);
            }
        }
    }
}
