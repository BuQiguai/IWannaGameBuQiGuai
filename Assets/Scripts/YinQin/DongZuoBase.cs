using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 动作基类  通过yunxing选定来运行动作
/// </summary>
[DefaultExecutionOrder(100)] // 执行慢
public abstract class DongZuoBase : MonoBehaviour
{
    //是否已经运行过
    bool isyunxing = false;
    // Start is called before the first frame update
    protected void Start()
    {
        World.instance.OnPlayerR += 重置;
        World.instance.OnPlayerR += 重置运行过;
        初始运行动作();
    }

    protected virtual void 初始运行动作()
    {
        
    }
        protected virtual void 销毁运行动作()
    {
        
    }
    protected void 重置运行过()
    {
        isyunxing = false;
    }
    protected abstract void 重置();


    public abstract void 动作();
    void OnDestroy()
    {
        World.instance.OnPlayerR -= 重置;
        World.instance.OnPlayerR -= 重置运行过;
        销毁运行动作();
    }
    // Update is called once per frame


}
