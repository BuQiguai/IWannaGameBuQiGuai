using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static 声音;

public class YunXingMore : MonoBehaviour
{
    PixelPerfectCollider collider;
    public YunXingType type = YunXingType.手枪射击;

    public DongZuoBase[] 目标单位;
    public SoundEffect 音效 = SoundEffect.空;
    //是否已经运行过
    bool isyunxing = false;
    Color color;
    // Start is called before the first frame update
    void Start()
    {
        color = this.GetComponent<SpriteRenderer>().color;
        World.instance.OnPlayerR += 重置;
        collider = GetComponent<PixelPerfectCollider>();
    }
    void 重置()
    {
        this.GetComponent<SpriteRenderer>().color = color;
        isyunxing = false;
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
                this.GetComponent<SpriteRenderer>().color = new Color(0, 1, 0, 0.5f);;
                foreach (var item in 目标单位)
                {
                    item?.动作();
                }

                return;
            }
        }
        else if (type == YunXingType.碰撞运行)
        {
            if (collider.PlaceMeeting(transform.position.x, transform.position.y, "Player"))
            {
                isyunxing = true;
                声音.Play音效(音效);
                this.GetComponent<SpriteRenderer>().color = new Color(0, 1, 0, 0.5f);;
                foreach (var item in 目标单位)
                {
                    item?.动作();
                }
            }
        }
    }
}
