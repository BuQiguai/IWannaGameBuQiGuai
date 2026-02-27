using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Danmu : MonoBehaviour
{
    public Vector2 speed = new Vector2(-4, 0);
    public float 加速度 = 0, 角速度 = 0;
    public bool 自动销毁 = true;
    public int f = 0;
    //每帧数执行动作
    public event System.Action<Danmu> onUpdate;

    Player player;

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<Player>();
    }

    // Update is called once per frame
    int timer = 0;
    void FixedUpdate()
    {
        timer++;
        if (player == null && timer % 2 == 0)
        {
            player = FindObjectOfType<Player>();
        }
        //应用加速度和角速度
        speed = speed.normalized * (speed.magnitude + 加速度);


        //角速度为旋转速度向量
        float angle = Mathf.Atan2(speed.y, speed.x) * Mathf.Rad2Deg;
        angle += 角速度;
        speed = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)) * speed.magnitude;
        //移动
        transform.position = new Vector3(transform.position.x + speed.x, transform.position.y + speed.y, transform.position.z);
        //距离摄像机太远就销毁
        if (自动销毁 && (Mathf.Abs(transform.position.x - Camera.main.transform.position.x) > 600 || Mathf.Abs(transform.position.y - Camera.main.transform.position.y) > 454))
        {
            Destroy(this.gameObject);
        }

        onUpdate?.Invoke(this);

        检测玩家碰撞伤害();
    }

    void 检测玩家碰撞伤害()
    {
        
        if (player!=null && 矩形与圆碰撞((Vector2)player.transform.position, 11, 21, (Vector2)transform.position + new Vector2(0,-2), 8,2))
        {
            player.Die();
        }

    }

static bool 矩形与圆碰撞(Vector2 rectCenter, float rectWidth, float rectHeight, 
                            Vector2 circlePos, float circleRadius, float yOffset = 0)
{
    // 计算实际矩形中心（考虑Y偏移）
    Vector2 adjustedCenter = new Vector2(rectCenter.x, rectCenter.y + yOffset);
    
    // 计算矩形半宽高
    float halfWidth = rectWidth * 0.5f;
    float halfHeight = rectHeight * 0.5f;
    
    // 计算矩形边界
    float rectLeft = adjustedCenter.x - halfWidth;
    float rectRight = adjustedCenter.x + halfWidth;
    float rectBottom = adjustedCenter.y - halfHeight;
    float rectTop = adjustedCenter.y + halfHeight;
    
    // 找出圆心到矩形最近的XY坐标
    float closestX = Mathf.Clamp(circlePos.x, rectLeft, rectRight);
    float closestY = Mathf.Clamp(circlePos.y, rectBottom, rectTop);
    
    // 计算最近点与圆心的距离平方（避免开方运算）
    float dx = circlePos.x - closestX;
    float dy = circlePos.y - closestY;
    float distanceSquared = dx * dx + dy * dy;
    
    // 比较距离平方与半径平方
    return distanceSquared <= (circleRadius * circleRadius);
}

}
