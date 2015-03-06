using UnityEngine;
using System.Collections;
namespace BaseEngine.UI{


    public class NGUIJoystick : UIBaseItem
{
    public float radius;
    private bool ispress;
    private System.Action<Vector3> moveEvent;
    private Vector3 normalized;



    public Vector3 ReadNormalized()
    {
        return normalized;
    }

    public void BindMoveEvent(System.Action<Vector3> moveEvent)
    {
        this.moveEvent = moveEvent;
    }

    private void OnPress(bool isPressed)
    {
        if (isPressed)
        {

        }
        else
        {
            MyTF.localPosition = Vector3.zero;
            normalized = Vector3.zero;
            moveEvent(normalized);
        }
    }

    private void CustomUpdate(UpdateData realTime)
    {
        if (moveEvent != null)
        {
            if (normalized != Vector3.zero)
            {
                moveEvent(normalized);
            }
        }
    }

    private void OnDoubleClick()
    {
        normalized = (MyTF.localPosition - Vector3.zero) / radius;
        Debug.Log(normalized + "");
    }

    private void OnDrag(Vector2 delta)
    {
        MyTF.localPosition += new Vector3(delta.x * 1.5f, delta.y * 1.5f);
        if (Vector3.Distance(MyTF.localPosition, Vector3.zero) > radius)
        {
            MyTF.localPosition = Vector3.zero + (MyTF.localPosition - Vector3.zero).normalized * radius;
        }
        Vector3 temp = (MyTF.localPosition - Vector3.zero) / radius;
        normalized = new Vector3(temp.x, 0, temp.y);
    }

}

}