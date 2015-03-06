using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace BaseEngine
{
    public class UpdateData
    {

        private float realtime;
        private bool isPause;
        public float lastTime;
        private UpdateData()
        {
            ResetTime();
        }


        private void ResetTime()
        {
            lastTime = Time.realtimeSinceStartup;
        }

        internal void UpdateTime()
        {
            realtime = Time.realtimeSinceStartup - lastTime;
            ResetTime();
        }

        /// <summary>
        /// 运行时间不受Time.timeScale影响
        /// </summary>
        public float RealTime
        {
            get
            {
                return realtime;
            }
        }

        /// <summary>
        /// 是否暂停
        /// </summary>
        public bool IsPause
        {
            get
            {
                return isPause;
            }
            internal set
            {
                isPause = value;
            }
        }

        internal static UpdateData Create()
        {
            return new UpdateData();
        }
    }
}
