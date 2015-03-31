using UnityEngine;
using System.Collections;
using BaseEngine;
/// <summary>
/// 动画控制器
/// </summary>
public sealed class AnimationTool : MetaScriptableHWQ
{
    /// <summary>
    /// 加载动画方式
    /// </summary>
    public static System.Func<string,string,AnimationClip> LoadAnimationClipEvent;
    public string loadAniamtionPath;
    private System.Collections.Generic.List<string> loadAnimationName = new System.Collections.Generic.List<string>();
    private System.Action<AnimationClip> animationFinishEvent;//动画播放完成时间
    private System.Action currentFinishEvent;
    private AnimationState curState; // 当前动画
    private AnimationClip lastFinish;
    private float animationTime; //
    private bool once; //
    private float animationSpeed;// 动画速度
    private string animationName;//动画名称
    private WrapMode warpMode;//循环模式
    private Animation _a;//动画组件
    private bool isPlay;//播放

    private AnimationTool()
    {

    }
    


    private void Awake()
    {

    }

    private void LoadAsset(string name)
    {
        if (LoadAnimationClipEvent != null)
        {
            if (_a[name] != null || loadAnimationName.Contains(name))
                return;
            loadAnimationName.Add(name);
            AnimationClip ac = LoadAnimationClipEvent(loadAniamtionPath, name);
            if (ac)
                _a.AddClip(ac, ac.name);
        }
    }

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="a"></param>
    public void Init(Animation a)
    {
        _a = a;
        _a.Stop();
    }

    /// <summary>
    /// 绑定动画结束事件
    /// </summary>
    /// <param name="e"></param>
    public void BindEvent(System.Action<AnimationClip> e)
    {
        animationFinishEvent = e;
    }

    private void PlayFinish(AnimationClip ac)
    {
        if (animationFinishEvent != null)
        {
            animationFinishEvent(ac);
        }
    }

    private void CurrentFinish()
    {
        if (currentFinishEvent != null)
        {
            currentFinishEvent();
        }
    }


    /// <summary>
    /// 验证时间
    /// </summary>
    /// <param name="percent">百分比</param>
    /// <returns></returns>
    public bool CheckAnimationTime(float percent)
    {
        if (curState != null)
        {
            return animationTime > curState.length * percent;
        }
        return false;
    }
    
    /// <summary>
    /// 判断动画时间
    /// </summary>
    /// <param name="time">时间</param>
    /// <returns></returns>
    public bool CheckAnimationByTime(float time)
    {
        if (curState != null)
        {
            return animationTime > time;
        }
        return false;
    }

    /// <summary>
    /// 改变动画
    /// </summary>
    /// <param name="name">动画名称</param>
    /// <param name="wm">循环模式</param>
    /// <param name="speed">播放速度</param>
    /// <param name="resetTime">重置时间</param>
    public void ChangeAnimation(string name, WrapMode wm, float speed, bool resetTime)
    {
        ChangePlaySpeed(speed);
        ChangeAnimation(name, wm, resetTime, null);
    }

    /// <summary>
    /// 改变动画
    /// </summary>
    /// <param name="name">动画名称</param>
    /// <param name="wm">循环模式</param>
    /// <param name="resetTime">重置时间</param>
    public void ChangeAnimation(string name, WrapMode wm,bool resetTime)
    {
        ChangeAnimation(name, wm, resetTime, null);
    }

    /// <summary>
    /// 改变动画
    /// </summary>
    /// <param name="name">动画名称</param>
    /// <param name="wm">循环模式</param>
    /// <param name="resetTime">重置动画</param>
    /// <param name="finishEvent">完成事件</param>
    public void ChangeAnimation(string name, WrapMode wm, bool resetTime, System.Action finishEvent)
    {
        if (_a == null)
            return;
        LoadAsset(name);
        once = false;
        warpMode = wm;
        if (resetTime || (!string.IsNullOrEmpty(name) && !string.Equals(animationName, name)))
            animationTime = 0;
        if (_a[name] == null)
        {
            return;
        }
        currentFinishEvent = finishEvent;
        animationName = name;
        _a.Play(animationName);
        _a.Stop();
        curState = _a[animationName];
  

    }

    /// <summary>
    /// 改变播放速度
    /// </summary>
    /// <param name="speed">速度</param>
    public void ChangePlaySpeed(float speed)
    {
        if (_a == null)
            return;
        animationSpeed = speed;
    }


    public Animation Animation()
    {
        return _a;
    }

    /// <summary>
    /// 播放动画
    /// </summary>
    public void RePlay()
    {
        isPlay = true;
    }

    /// <summary>
    /// 暂停动画
    /// </summary>
    public void Pause()
    {
        isPlay = false;
    }

    /// <summary>
    /// 运行动画
    /// </summary>
    public void RunAnimation()
    {
        if (_a == null || curState == null)
        {
            return;
        }

        curState.enabled = true;

        animationTime += (isPlay ? Time.deltaTime : 0) * animationSpeed;
        curState.time = animationTime;
        _a.Sample();
        curState.enabled = false;
        switch (warpMode)
        {
            case WrapMode.Loop:
                if (curState.length <= curState.time)
                {
                    PlayFinish(curState.clip);
                    CurrentFinish();
                    lastFinish = curState.clip;
                    animationTime = 0;
                }
                break;
            case WrapMode.Default:
            case WrapMode.Once:
                if (!once && curState.length <= curState.time)
                {
                    animationTime = curState.length;
                    once = true;
                    PlayFinish(curState.clip);
                    CurrentFinish();
                    lastFinish = curState.clip;
                }
                break;
            case WrapMode.ClampForever:
                if (curState.length <= curState.time)
                    animationTime = curState.length;
                break;
                
        }

    }

    /// <summary>
    /// 获得刚刚完成的动画
    /// </summary>
    /// <returns></returns>
    public AnimationClip GetLastFinish()
    {
        AnimationClip ac = lastFinish;
        lastFinish = null;
        return ac;
    }

    /// <summary>
    /// 获得当前动画长度
    /// </summary>
    /// <returns></returns>
    public float GetStateLength()
    {
        if (curState == null)
            return 0;
        return curState.length;
    }


    public static AnimationTool Create(Animation animation, System.Action<AnimationClip> e)
    {
        if (animation != null)
        {
            AnimationTool ao = Create();
            ao.Init(animation);
            ao.BindEvent(e);
            ao.RePlay();
            return ao;

        }
      
        return null;
    }

    public static AnimationTool Create()
    {
        return AnimationTool.CreateInstance<AnimationTool>();
    }
}
