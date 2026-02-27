using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static 声音;

public enum YunXingType
{
    手枪射击,
    碰撞运行
}
public class YunXing : MonoBehaviour
{
    new PixelPerfectCollider collider;
    public YunXingType type = YunXingType.碰撞运行;

    public DongZuoBase 目标单位;

    public SoundEffect 音效 = SoundEffect.空;
    //是否已经运行过
    bool isyunxing = false;
    // Start is called before the first frame update
    void Start()
    {
        World.instance.OnPlayerR += 重置;
        collider = GetComponent<PixelPerfectCollider>();
    }
    void 重置()
    {
        isyunxing = false;
    }

    void OnDestroy()
    {
        World.instance.OnPlayerR -= 重置;
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if (isyunxing) return;

        if (type == YunXingType.手枪射击)
        {
            if (collider.PlaceMeeting(transform.position.x, transform.position.y, "Bullet"))
            {
                isyunxing = true;
                                声音.Play音效(音效);
                目标单位?.动作();
                return;
            }
        }
        else if (type == YunXingType.碰撞运行)
        {
            if (collider.PlaceMeeting(transform.position.x, transform.position.y, "Player"))
            {
                isyunxing = true;
                                声音.Play音效(音效);
                目标单位?.动作();
            }
        }
    }
}
