using UnityEngine;
using System.Collections;
using BaseEngine.UI;

public class ButtonItemUI : UIBaseItem
{
    private System.Action<bool> pressEvent;
    private System.Action<object[]> clickEvent;
    private bool isDrag;
    private object[] objList;

    /// <summary>
    /// 绑定按下事件
    /// </summary>
    /// <param name="pressEvent"></param>
    public void BindPressEevent(System.Action<bool> pressEvent)
    {
        this.pressEvent = pressEvent;
    }

    /// <summary>
    /// 绑定单击事件
    /// </summary>
    /// <param name="clickEvent"></param>
    /// <param name="objList"></param>
    public void BindClickEvent(System.Action<object[]> clickEvent,params object[] objList)
    {
        this.clickEvent = clickEvent;
        this.objList = objList;
    }

    void OnDrag(Vector2 delta)
    {
        isDrag = true;
    }

    protected virtual void OnPress(bool isPressed)
    {
        if (isPressed)
        {
            isDrag = false;
        }
        if (pressEvent != null)
        {
            pressEvent(isPressed);
        }
    }

    protected virtual void OnClick()
    {
        if (!isDrag)
        {
            if (clickEvent != null)
            {
                clickEvent(objList);
            }
        }
   
    }

}
