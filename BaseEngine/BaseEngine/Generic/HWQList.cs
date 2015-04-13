using System;
using System.Collections.Generic;
using System.Collections;

namespace BaseEngine.Generic
{
    public sealed class HWQList<T, T1> : IEnumerable<T1>
    {
        private List<T1> allNode = new List<T1>();
        private Dictionary<T, T1> nodeDic = new Dictionary<T, T1>();
        private Dictionary<int, T> collect = new Dictionary<int, T>();


        /// <summary>
        /// 添加一个元素
        /// </summary>
        /// <param name="t1"></param>
        /// <param name="t2"></param>
        public void Add(T t1, T1 t2)
        {
            if (t2 != null)
            {
                if (allNode.Contains(t2))
                {
                    allNode.Add(t2);
                }
                collect[t2.GetHashCode()] = t1;
                nodeDic[t1] = t2;
            }
        }

        /// <summary>
        /// 删除一个键
        /// </summary>
        /// <param name="t1"></param>
        public void RemoveKey(T t1)
        {
            if (nodeDic.ContainsKey(t1))
            {
                RemoveObject(nodeDic[t1]);
            }
        }

        /// <summary>
        /// 删除下标
        /// </summary>
        /// <param name="index">下标</param>
        public void RemoveIndex(int index)
        {
            if (allNode.Count > index && index >= 0)
            {
                T1 t = allNode[index];
                RemoveObject(t);
            }
        }

        /// <summary>
        /// 删除一个对象
        /// </summary>
        /// <param name="t"></param>
        public void RemoveObject(T1 t)
        {
            int hc = t.GetHashCode();
            if (collect.ContainsKey(hc))
            {
                T tt = collect[hc];
                nodeDic.Remove(tt);
                collect.Remove(hc);
                allNode.Remove(t);
            }
        }

        /// <summary>
        /// 删除所有元素
        /// </summary>
        public void DeleteAll()
        {
            while (allNode.Count > 0)
            {
                RemoveObject(allNode[0]);
            }
        }


        IEnumerator<T1> IEnumerable<T1>.GetEnumerator()
        {
            return allNode.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return allNode.GetEnumerator();
        }
    }
}
