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
        if(!controller)
        controller = GetComponent<CharacterController>();
        if (!controller)
        {
            enabled = false;
            DestroyImmediate(this);
            return;
        }
        ResetGravity();
    }

    /// <summary>
    /// 判断是否在地上
    /// </summary>
    /// <returns></returns>
    public bool CheckGrounded()
    {
        if (controller)
            return controller.isGrounded;
        return false;
    }

    /// <summary>
    /// 改变移动速度
    /// </summary>
    /// <param name="moveSpeed"></param>
    public void ChangeSpeed(float moveSpeed)
    {
        curMoveSpeed = moveSpeed;
    }

    /// <summary>
    /// 改变移动方向
    /// </summary>
    /// <param name="moveDir"></param>
    public void ChangeDir(Vector3 moveDir)
    {
        moveDirection = moveDir;
        
    }

    /// <summary>
    /// 应用重力
    /// </summary>
    /// <param name="appleGravity"></param>
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

    /// <summary>
    /// 重置重力
    /// </summary>
    public void ResetGravity()
    {
        m_gravity = GRAVITY;
    }

    /// <summary>
    /// 移动
    /// </summary>
    /// <param name="realTime"></param>
    public void Movement(float realTime)
    {
        Vector3 movement = moveDirection * curMoveSpeed;
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

    /// <summary>
    /// 移动
    /// </summary>
    /// <param name="dir"></param>
    public void MovementVector3(Vector3 dir)
    {
        if (controller)
        {
            controller.Move(dir);
        }
    }

    /// <summary>
    /// 获得当前速度
    /// </summary>
    /// <returns></returns>
    public float GetCurrentSpeed()
    {
        return controller.velocity.magnitude;
    }

}
