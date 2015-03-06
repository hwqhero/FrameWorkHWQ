using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace BaseEngine
{
    /// <summary>
    /// 数据对象
    /// </summary>
    public class DataCenterObject : MetaScriptableHWQ
    {

        /// <summary>
        /// 修改过的对象
        /// </summary>
        private static List<DataCenterObject> changeObject = new List<DataCenterObject>();
        /// <summary>
        /// 所有对象
        /// </summary>
        private static List<DataCenterObject> centerObjectList = new List<DataCenterObject>();

        /// <summary>
        /// 更新事件
        /// </summary>
        private System.Action<DataCenterObject> m_ModifyEvent;
        /// <summary>
        /// 销毁事件
        /// </summary>
        private System.Action<DataCenterObject> m_DestroyEvent;

        protected DataCenterObject()
        {
            centerObjectList.Add(this);
        }

        /// <summary>
        /// 绑定事件
        /// </summary>
        /// <param name="modifyEvent">更新事件</param>
        /// <param name="destroyEvent">销毁事件</param>
        public void BindEvent(System.Action<DataCenterObject> modifyEvent, System.Action<DataCenterObject> destroyEvent)
        {
            m_ModifyEvent += modifyEvent;
            m_DestroyEvent += destroyEvent;
        }

        /// <summary>
        /// 移除绑定
        /// </summary>
        /// <param name="modifyEvent">更新事件</param>
        /// <param name="destroyEvent">销毁事件</param>
        public void RemoveEvent(System.Action<DataCenterObject> modifyEvent, System.Action<DataCenterObject> destroyEvent)
        {
            m_ModifyEvent -= modifyEvent;
            m_DestroyEvent -= destroyEvent;
        }

        /// <summary>
        /// 提交修改
        /// </summary>
        public void CommitChange()
        {
            if (!changeObject.Contains(this))
            {
                changeObject.Add(this);
            }
        }

        /// <summary>
        /// 删除对象
        /// </summary>
        public void Delete()
        {
            DestroyImmediate(this, true);
        }

        /// <summary>
        /// 销毁
        /// </summary>
        private void OnDestroy()
        {
            changeObject.Remove(this);
            centerObjectList.Remove(this);
            if (m_DestroyEvent != null)
            {
                m_DestroyEvent(this);
            }
        }

        internal static void Update()
        {
            while (changeObject.Count > 0)
            {
                try
                {
                    changeObject[0].Change();
                }
                catch (System.Exception ex)
                {
                    Debug.Log("Update error --->" + ex.Message);
                }
                finally
                {
                    changeObject.RemoveAt(0);
                }
            }
        }

        private void Change()
        {
            if (m_ModifyEvent != null)
            {
                m_ModifyEvent(this);
            }
        }
    }
}
