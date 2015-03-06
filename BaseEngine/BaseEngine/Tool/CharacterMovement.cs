using UnityEngine;
using System.Collections;

/// <summary>
/// 角色运动
/// </summary>
public class CharacterMovement : MonoBehaviour
{
    /// <summary>
    /// 角色控制器
    /// </summary>
    public CharacterController controller;

    private const float GRAVITY = -9.8f;//默认重力
    private const float AIRRESISTANCE = 2.3f;//空气阻力
    private float m_gravity;
    private float curMoveSpeed;//当前移动速度

    private Vector3 moveDirection;//移动方向 

    private float vSpeed;//下落速度

    private bool apply;//应用重力

    private float floatingnum;
    private float stiffTime;
    private float flyPower;
    private Vector3 flyDir;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        if (!controller)
        {
            enabled = false;
            DestroyImmediate(this);
            return;
        }
        ResetGravity();
    }

    public bool CheckGrounded()
    {
        if (controller)
            return controller.isGrounded;
        return false;
    }

    public void ChangeSpeed(float moveSpeed)
    {
        curMoveSpeed = moveSpeed;
    }

    public void ChangeDir(Vector3 moveDir)
    {
        moveDirection = moveDir;
        
    }

    public void ApplyGravity(bool appleGravity)
    {
        apply = appleGravity;
    }

    public void Fly(float height, float g, float flyPower, Vector3 dir)
    {
        vSpeed = height;
        m_gravity = GRAVITY * g;
        this.flyPower = flyPower;
        flyDir = dir;
    }

    /// <summary>
    /// 获得垂直速度
    /// </summary>
    /// <returns></returns>
    public float GetVerticalSpeed()
    {
        return vSpeed;
    }

    public void ResetGravity()
    {
        m_gravity = GRAVITY;
    }

    public void Movement(float realTime)
    {
        Vector3 movement = moveDirection;
        vSpeed += m_gravity * Time.deltaTime;
        vSpeed = Mathf.Max(-80, vSpeed);
        if (!apply)
        {
            movement += Vector3.up * vSpeed;
        }
        if (flyPower > 0)
        {
            movement += flyDir * flyPower;
            flyPower -= Time.deltaTime * AIRRESISTANCE;
        }
        movement *= Time.deltaTime;
        MovementVector3(movement);
    }

    public void MovementVector3(Vector3 dir)
    {
        if (controller)
        {
            controller.Move(dir);
        }
    }

}
