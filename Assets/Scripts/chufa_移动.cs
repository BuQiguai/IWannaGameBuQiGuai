using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;

public class chufa_移动 : DongZuoBase
{
    public Vector2 速度;
    public bool isMove = false;
    public float 最大移动距离 = 9999;
    float _移动距离 = 9999;
    Vector3 startPos;
    void Start()
    {
        //调用父类的初始化方法
        base.Start();

        Debug.Log("初始化" + gameObject.name);
        startPos = gameObject.transform.position;
        _移动距离 = 最大移动距离;
    }
    public override void 动作()
    {
        //移动
        isMove = true;
        startPos = gameObject.transform.position;
    }
    void FixedUpdate()
    {
        if (isMove)
        {
            _移动距离 -= 速度.magnitude;
            if (_移动距离 > 0)
            {
                gameObject.transform.Translate(速度);
            }
            else
            {
                //固定在最终位置
                gameObject.transform.position = new Vector2(startPos.x, startPos.y) + (速度.normalized * 最大移动距离);
            }
        }

    }
    protected override void 重置()
    {
        Debug.Log("重置" + gameObject.name);
        isMove = false;
        gameObject.transform.position = startPos;
        _移动距离 =  最大移动距离;
    }
}
