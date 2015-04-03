using UnityEngine;
using System.Collections;
using System.Collections.Generic;
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
    /// <summary>
    /// 加载动画的路劲
    /// </summary>
    public string loadAniamtionPath;
    private List<string> loadAnimationName = new List<string>();
    private Dictionary<int, List<AnimationEventHWQ>> animationEventDic = new Dictionary<int, List<AnimationEventHWQ>>();
    private List<int> eventIdList = new List<int>();
    private System.Action<AnimationClip> animationFinishEvent;//动画播放完成时间
    private System.Action currentFinishEvent;
    private AnimationState curState; // 当前动画
    private AnimationClip lastFinish;//
    private float animationTime; //动画时间
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
    /// 获得当前动画时间
    /// </summary>
    /// <returns></returns>
    public float GetCurrentTime()
    {
        return animationTime;
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
        eventIdList.Clear();
    }

    private int GetEventCountState(AnimationState state)
    {
        if (state != null)
        {
            int hc = state.GetHashCode();
            if (animationEventDic.ContainsKey(hc))
            {
                return animationEventDic[hc].Count;
            }
        }
        return 0;
    }

    /// <summary>
    /// 获得当前动画绑定事件数量
    /// </summary>
    /// <returns>数量</returns>
    public int GetEventCountState()
    {
        return GetEventCountState(curState);
    }

    /// <summary>
    /// 获得动画绑定事件数量
    /// </summary>
    /// <param name="name">动画名称</param>
    /// <returns>数量</returns>
    public int GetEventCountState(string name)
    {
        if (_a != null)
        {
            AnimationState astate = _a[animationName];
            if (astate)
            {
                return GetEventCountState(astate);
            }
        }
        return 0;
    }



    /// <summary>
    /// 绑定当前动画事件
    /// </summary>
    /// <param name="time">动画时间</param>
    /// <param name="event">事件</param>
    /// <param name="objList">参数列表</param>
    public void AddEventState(float time, System.Action<object[]> @event, params object[] objList)
    {
        AddEState(curState, time, @event, objList);
    }


    /// <summary>
    /// 绑定动画事件
    /// </summary>
    /// <param name="aniamtionName">动画名称</param>
    /// <param name="time">触发时间</param>
    /// <param name="event">事件</param>
    /// <param name="objList">参数</param>
    public void AddEventStateByName(string aniamtionName, float time, System.Action<object[]> @event, params object[] objList)
    {
        if (_a != null)
        {
            AnimationState astate = _a[animationName];
            if (astate)
            {
                AddEState(astate, time, @event, objList);
            }
        }
    }

    /// <summary>
    /// 删除所有事件
    /// </summary>
    public void ClearAllAnimationEvent()
    {
        foreach (KeyValuePair<int, List<AnimationEventHWQ>> keyValue in animationEventDic)
        {
            foreach (AnimationEventHWQ aehwq in keyValue.Value)
            {
                DestroyImmediate(aehwq, true);
            }
        }
        animationEventDic.Clear();
    }


    

    private void AddEState(AnimationState cc, float time, System.Action<object[]> @event, object[] objList)
    {
        if (cc)
        {
            int hc = cc.GetHashCode();
            if (!animationEventDic.ContainsKey(hc))
            {
                animationEventDic.Add(hc, new List<AnimationEventHWQ>());

            }
            AnimationEventHWQ aehwq = AnimationEventHWQ.CreateInstance<AnimationEventHWQ>();
            aehwq.@event = @event;
            aehwq.parameters = objList;
            aehwq.time = time;
            animationEventDic[hc].Add(aehwq);
        }
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

    /// <summary>
    /// 获得动画组件
    /// </summary>
    /// <returns></returns>
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
    /// 触发事件
    /// </summary>
    private void TriggerEvent()
    {
        int hc = curState.GetHashCode();
        if (animationEventDic.ContainsKey(hc))
        {
            foreach (AnimationEventHWQ aehwq in animationEventDic[hc])
            {
                if (!eventIdList.Contains(aehwq.GetHashCode()) && aehwq.time < curState.time)
                {
                    aehwq.Execute();
                    eventIdList.Add(aehwq.GetHashCode());
                }
            }
        }
    }

    /// <summary>
    /// 运行动画
    /// </summary>
    public void RunAnimation()
    {
        if (_a == null || curState == null || !isPlay)
        {
            return;
        }

        curState.enabled = true;
        animationTime += Time.deltaTime * animationSpeed;
        curState.time = animationTime;
        TriggerEvent();
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
                    eventIdList.Clear();
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

    /// <summary>
    /// 创建一个 动画工具
    /// </summary>
    /// <param name="animation">Unity动画组件 必需</param>
    /// <param name="e">所有动画结束事件</param>
    /// <returns></returns>
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

    private sealed class AnimationEventHWQ : MetaScriptableHWQ
    {

        public System.Action<object[]> @event;
        public object[] parameters;
        public float time;

        public void Execute()
        {
            if (@event != null)
            {
                @event(parameters);
            }
        }

        private AnimationEventHWQ()
        {

        }
    }
}
