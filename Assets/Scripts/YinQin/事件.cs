using System;
using System.Collections.Generic;

/// <summary>
/// 静态事件管理器（自动清理空引用）
/// </summary>
public static class 事件
{
    // 事件字典（使用弱引用避免内存泄漏）
    private static readonly Dictionary<string, List<WeakReference<Action>>> _eventActions = new Dictionary<string, List<WeakReference<Action>>>();
    private static readonly Dictionary<string, List<WeakReference<Action<object>>>> _eventActionsWithData = new Dictionary<string, List<WeakReference<Action<object>>>>();

    /// <summary>
    /// 注册无参数事件
    /// </summary>
    public static void on(string eventName, Action callback)
    {
        if (!_eventActions.ContainsKey(eventName))
        {
            _eventActions[eventName] = new List<WeakReference<Action>>();
        }

        _eventActions[eventName].Add(new WeakReference<Action>(callback));
    }

    /// <summary>
    /// 注册带参数事件
    /// </summary>
    public static void on(string eventName, Action<object> callback)
    {
        if (!_eventActionsWithData.ContainsKey(eventName))
        {
            _eventActionsWithData[eventName] = new List<WeakReference<Action<object>>>();
        }

        _eventActionsWithData[eventName].Add(new WeakReference<Action<object>>(callback));
    }

    /// <summary>
    /// 触发无参数事件
    /// </summary>
    public static void emit(string eventName)
    {
        if (_eventActions.TryGetValue(eventName, out var callbacks))
        {
            // 清理空引用
            callbacks.RemoveAll(wr => !wr.TryGetTarget(out _));

            foreach (var weakRef in callbacks.ToArray()) // 使用ToArray避免迭代时修改集合
            {
                if (weakRef.TryGetTarget(out var callback))
                {
                    callback?.Invoke();
                }
            }

            // 如果列表为空，移除该事件
            if (callbacks.Count == 0)
            {
                _eventActions.Remove(eventName);
            }
        }
    }

    /// <summary>
    /// 触发带参数事件
    /// </summary>
    public static void emit(string eventName, object eventData)
    {
        if (_eventActionsWithData.TryGetValue(eventName, out var callbacks))
        {
            // 清理空引用
            callbacks.RemoveAll(wr => !wr.TryGetTarget(out _));

            foreach (var weakRef in callbacks.ToArray())
            {
                if (weakRef.TryGetTarget(out var callback))
                {
                    callback?.Invoke(eventData);
                }
            }

            // 如果列表为空，移除该事件
            if (callbacks.Count == 0)
            {
                _eventActionsWithData.Remove(eventName);
            }
        }
    }

    /// <summary>
    /// 移除指定事件的所有监听
    /// </summary>
    public static void RemoveEvent(string eventName)
    {
        _eventActions.Remove(eventName);
        _eventActionsWithData.Remove(eventName);
    }

    /// <summary>
    /// 清理所有空引用
    /// </summary>
    public static void Cleanup()
    {
        // 清理无参数事件
        foreach (var kvp in _eventActions)
        {
            kvp.Value.RemoveAll(wr => !wr.TryGetTarget(out _));
            if (kvp.Value.Count == 0)
            {
                _eventActions.Remove(kvp.Key);
            }
        }

        // 清理带参数事件
        foreach (var kvp in _eventActionsWithData)
        {
            kvp.Value.RemoveAll(wr => !wr.TryGetTarget(out _));
            if (kvp.Value.Count == 0)
            {
                _eventActionsWithData.Remove(kvp.Key);
            }
        }
    }
}