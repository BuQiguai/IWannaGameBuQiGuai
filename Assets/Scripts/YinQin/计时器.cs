using UnityEngine;
using System;
using System.Collections.Generic;

public class 计时器 : MonoBehaviour
{
    private static 计时器 _instance;
    private Dictionary<string, TimerData> _timers = new Dictionary<string, TimerData>();
    private Dictionary<string, TimerData> _pendingAddTimers = new Dictionary<string, TimerData>();
    private List<string> _pendingRemoveTimers = new List<string>();
    private bool _isInFixedUpdate = false;

    void Start()
    {
        World.instance.OnPlayerR += 重置;
    }

    void OnDestroy()
    {
        World.instance.OnPlayerR -= 重置;
    }

    void 重置()
    {
        清除所有计时器();
    }

    // 外部调用接口 (静态方法)
    public static string 延时执行(float delay, Action callback)
    {
        string id = Guid.NewGuid().ToString();
        Instance._SetTimer(id, delay, false, false, callback, -1);
        return id;
    }

    public static string 执行(float interval, Action callback, int executeCount = -1)
    {
        string id = Guid.NewGuid().ToString();
        Instance._SetTimer(id, interval, true, false, callback, executeCount);
        return id;
    }

    public static void SetCountdown(string id, float duration, Action callback, int executeCount = -1)
    {
        Instance._SetTimer(id, duration, false, true, callback, executeCount);
    }

    public static void ClearTimer(string id)
    {
        Instance._RemoveTimer(id);
    }

    public static void 清除所有计时器()
    {
        Instance._timers.Clear();
    }

    // 内部实现
    private static 计时器 Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("全局计时器");
                _instance = go.AddComponent<计时器>();
                DontDestroyOnLoad(go);
            }
            return _instance;
        }
    }

    private void _SetTimer(string id, float duration, bool isInterval, bool isCountdown, Action callback, int executeCount)
    {
        var timer = new TimerData
        {
            duration = duration,
            remaining = isCountdown ? duration : 0,
            isInterval = isInterval,
            isCountdown = isCountdown,
            callback = callback,
            executeCount = executeCount,
            executedCount = 0
        };

        if (_isInFixedUpdate)
        {
            // 如果在 FixedUpdate 中调用，先暂存到待添加列表
            _pendingAddTimers[id] = timer;
        }
        else
        {
            // 否则直接添加
            _timers[id] = timer;
        }
    }

    private void _RemoveTimer(string id)
    {
        if (_isInFixedUpdate)
        {
            // 如果在 FixedUpdate 中调用，先暂存到待删除列表
            _pendingRemoveTimers.Add(id);
        }
        else
        {
            // 否则直接删除
            _timers.Remove(id);
        }
    }

    private void FixedUpdate()
    {
        _isInFixedUpdate = true;

        // 1. 处理当前计时器
        List<string> timersToRemove = new List<string>();

        foreach (var pair in _timers)
        {
            var timer = pair.Value;
            
            if (timer.isCountdown)
            {
                timer.remaining -= Time.fixedDeltaTime;
                if (timer.remaining <= 0)
                {
                    timer.callback?.Invoke();
                    timer.executedCount++;
                    
                    if (timer.isInterval && (timer.executeCount < 0 || timer.executedCount < timer.executeCount))
                    {
                        timer.remaining = timer.duration;
                    }
                    else
                    {
                        timersToRemove.Add(pair.Key);
                    }
                }
            }
            else
            {
                timer.remaining += Time.fixedDeltaTime;
                if (timer.remaining >= timer.duration)
                {
                    timer.callback?.Invoke();
                    timer.executedCount++;
                    
                    if (timer.isInterval && (timer.executeCount < 0 || timer.executedCount < timer.executeCount))
                    {
                        timer.remaining = 0;
                    }
                    else
                    {
                        timersToRemove.Add(pair.Key);
                    }
                }
            }
        }

        // 2. 删除已完成的计时器
        foreach (var id in timersToRemove)
        {
            _timers.Remove(id);
        }

        // 3. 处理在回调期间新增的计时器
        foreach (var pair in _pendingAddTimers)
        {
            _timers[pair.Key] = pair.Value;
        }
        _pendingAddTimers.Clear();

        // 4. 处理在回调期间请求删除的计时器
        foreach (var id in _pendingRemoveTimers)
        {
            _timers.Remove(id);
        }
        _pendingRemoveTimers.Clear();

        _isInFixedUpdate = false;
    }

    private class TimerData
    {
        public float duration;        // 时间间隔/倒计时总时长
        public float remaining;      // 剩余时间
        public bool isInterval;      // 是否是重复执行
        public bool isCountdown;     // 是否是倒计时模式
        public Action callback;      // 回调函数
        public int executeCount;     // 执行次数 (-1表示无限)
        public int executedCount;    // 已执行次数
    }
}