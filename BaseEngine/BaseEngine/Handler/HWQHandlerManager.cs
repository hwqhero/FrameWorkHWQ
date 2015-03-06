using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using BaseEngine;

internal sealed class HWQHandlerManager : MetaHWQ
{

    private HWQHandlerManager() { }

    public static event Action<UpdateData> UpdateHandler, LateHandler, FixedHandler;
    private UpdateData updateTime, lateTime, fixedTime;
    private void Awake()
    {
        updateTime = UpdateData.Create();
        lateTime = UpdateData.Create();
        fixedTime = UpdateData.Create();

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
        if (UpdateHandler != null)
        {
            updateTime.UpdateTime();
            UpdateHandler(updateTime);
            
        }
    }

    private void LateUpdate()
    {
        if (LateHandler != null)
        {
            Delegate d = (Delegate)LateHandler;
            
            lateTime.UpdateTime();
            LateHandler(lateTime);
        }
    }

    private void FixedUpdate()
    {
        if (FixedHandler != null)
        {
            fixedTime.UpdateTime();
            FixedHandler(fixedTime);
        }
    }


}

