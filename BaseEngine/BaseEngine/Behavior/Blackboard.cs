using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BaseEngine
{
    public class Blackboard
    {
        public static readonly Blackboard GlobalDatas = new Blackboard();
        private Dictionary<string, object> datasStore = new Dictionary<string, object>();
        public event System.Action<Blackboard, BlackboardUpdateType> UpdateEvent;

        private void OnUpdateEvent(BlackboardUpdateType updateType)
        {
            if (updateType != null)
            {
                UpdateEvent(this, updateType);
            }
        }

        public bool HaveDatas
        {
            get
            {
                return datasStore.Keys.Count > 0;
            }
        }

        public object GetData(string name)
        {
            return datasStore[name];
        }

        public T GetData<T>(string name)
        {
            if (!datasStore.ContainsKey(name))
            {
                return default(T);
            }
            return (T)datasStore[name];
        }

        public void AddData(string name, object data)
        {
            var srcHad = datasStore.ContainsKey(name);
            datasStore[name] = data;
            OnUpdateEvent(srcHad ? BlackboardUpdateType.Modify : BlackboardUpdateType.Add);
        }

        public object RemoveData(string name)
        {
            if (!datasStore.ContainsKey(name))
                return null;
            object result = datasStore[name];
            datasStore.Remove(name);
            OnUpdateEvent(BlackboardUpdateType.Remove);
            return result;
        }

        public void UpdateData(string name)
        {
            if (!datasStore.ContainsKey(name))
                return;
            OnUpdateEvent(BlackboardUpdateType.Modify);
        }

    }
}
