using UnityEngine;
using System.Collections;

public class ButtonItemUI : View {
    private System.Action<bool> pressEvent;
    private System.Action<object[]> clickEvent;
    private bool isDrag;
    private object[] objList;
    public void BindPressEevent(System.Action<bool> pressEvent)
    {
        this.pressEvent = pressEvent;
    }

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
