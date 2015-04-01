using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace BaseEngine
{
    /// <summary>
    /// 数据控制
    /// </summary>
    public sealed class DataCenter : MetaHWQ
    {
        /// <summary>
        /// 数据
        /// </summary>
        private Dictionary<string, object> objectDic = new Dictionary<string, object>();

        private DataCenter()
        {

        }

        private new void Awake()
        {
            BaseSystem.instance = this;
        }

        private void Update()
        {
            if (Input.GetKey(KeyCode.Escape))
            {
                Application.Quit();
            }
        }

        private void LateUpdate()
        {
            DataCenterObject.Update();
        }


        /// <summary>
        /// 读取数据
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="key">键</param>
        /// <returns></returns>
        public T Read<T>(string key) where T : class
        {
            if (objectDic.ContainsKey(key))
            {
                return objectDic[key] as T;
            }
            return default(T);
        }

        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="obj">值</param>
        /// <returns>写入成功</returns>
        public bool Write(string key,object obj)
        {
            if (objectDic.ContainsKey(key))
            {
                HWQEngine.Log("重复的键");
                return false;
            }
            objectDic.Add(key, obj);
            return true;
        }
    }
}
