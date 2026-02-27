using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class S1_boss_ai : MonoBehaviour
{
    //弹幕预制件
    private Player player;
    public GameObject danmuPrefab;

    public AudioClip bgmClip;

    BOSS boss;
    AudioClip oldClip;

    public GameObject 传送门;
    public AudioSource 死亡音效;
    public AudioSource 受伤音效;
    //血条显示颜色
    public SpriteRenderer 血条;
    // Start is called before the first frame update
    void Start()
    {
        boss = GetComponent<BOSS>();

        boss.受到攻击 += 受到攻击;
        boss.死亡 += 死亡运行;

        World.instance.OnPlayerR += Chongzi;

    }

    void 播放bgm()
    {
        oldClip = 声音.getBGM();
        声音.Play音乐(bgmClip);
    }
    List<Danmu> danmuList = new List<Danmu>();
    List<Tween> 变换列表 = new List<Tween>();
    int timer = 0;
    void FixedUpdate()
    {

        timer++;
        if (player == null)
        {
            player = FindObjectOfType<Player>();
            return;
        }

        if (timer < 100)
        {
            if (timer == 10)
            {
                播放bgm();
                血条.DOColor(Color.red, 0.5f);
                变换列表.Add(transform.DOMoveY(0, 3f));
            }
            //开场动画
            return;
        }
        var 阶段 = (timer - 100) / 650;
        var 阶段内计时 = (timer - 100) % 650;
        if (阶段 % 3 == 0)
        {
            if (阶段内计时 == 0)
            {
                Debug.Log("timer = " + timer);
                变换列表.Add(transform.DOMoveY(0, 1f));
            }
            if (阶段内计时 == 100)
            {
                计时器.执行(0.06f, () =>
                {
                    普通螺旋弹幕();
                }, 140);
            }


        }
        else if (阶段 % 3 == 1)
        {
            if (阶段内计时 == 0)
            {
                变换列表.Add(transform.DOMoveY(160, 1f));
            }
            if (阶段内计时 == 50 && timer < 1900)
            {
                var a = 1;
                计时器.执行(0.06f, () =>
                {
                    随机弹幕(a++);
                }, 160);
            }
            if (阶段内计时 == 50 && timer > 1900)
            {
                var a = 1;
                计时器.执行(0.08f, () =>
                {
                    发射弹幕(new Vector3(400*Random.Range(-1f, 1f), 290, 0), 270+60*Random.Range(-1f, 1f), 8f*Random.Range(0.4f, 1));
                }, 130);
            }
        }
        else if (阶段 % 3 == 2)
        {
            //战吼

            if (阶段内计时 == 0)
            {
                var i = 0;
                计时器.执行(0.02f, () =>
                {
                    i++;
                    发射弹幕(transform.position, i * 187, 8f).onUpdate += (d) =>
                    {
                        d.f++;
                        if (d.f == 30)
                        {
                            d.加速度 = 0.1f;
                            //随机一个角度
                            d.speed = new Vector2(Mathf.Cos(Random.Range(0, 360) * Mathf.Deg2Rad), Mathf.Sin(Random.Range(0, 360) * Mathf.Deg2Rad)) * 0.1f;
                        }
                    };

                }, 50);
            }

            if (阶段内计时 % 50 == 0 && 阶段内计时 > 180)
            {
                战吼();
            }

        }
    }

    void 受到攻击()
    {
        受伤音效.Play();
    }
    void 死亡运行()
    {
        计时器.清除所有计时器();
        foreach (var 变换 in 变换列表)
        {
            变换.Kill();
        }

        //播放死亡音效

        声音.StopMusic();
        死亡音效.Play();
        传送门.SetActive(true);
    }


    void 战吼()
    {
        //扩散弹幕
        float angle1 = Random.Range(0, 360);
        水波着色器.尝试调用冲击波特效(transform.position, 0.2f, 0.2f, 6f);
        for (int i = 0; i < 360; i += 36)
        {
            danmuList.ForEach(d =>
            {
                if (d != null)
                {
                    //使其速度变为固定
                    d.speed = Vector2.ClampMagnitude(d.speed, 14);
                }
            });

            GameObject danmu = Instantiate(danmuPrefab, transform.position, Quaternion.identity);
            //随机角度随机速度向量
            float angle = i + angle1;
            Vector2 speed = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)) * 10;
            danmu.GetComponent<Danmu>().speed = speed;
            danmuList.Add(danmu.GetComponent<Danmu>());

            //销毁
        }
        for (int i = 0; i < 360; i += 18)
        {
            danmuList.ForEach(d =>
            {
                if (d != null)
                {
                    //使其速度变为固定
                    d.speed = Vector2.ClampMagnitude(d.speed, 14);
                }
            });

            GameObject danmu = Instantiate(danmuPrefab, transform.position, Quaternion.identity);
            //随机角度随机速度向量
            float angle = i + angle1 + 9;
            Vector2 speed = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)) * 7;
            danmu.GetComponent<Danmu>().speed = speed;
            danmuList.Add(danmu.GetComponent<Danmu>());

            //销毁
        }

    }

    Danmu 发射弹幕(Vector3 position, float angle, float speed, float 加速度 = 0, float 角速度 = 0)
    {
        GameObject danmu = Instantiate(danmuPrefab, position, Quaternion.identity);
        //随机角度随机速度向量
        Vector2 speed2 = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)) * speed;
        var danmu1 = danmu.GetComponent<Danmu>();
        danmu1.speed = speed2;
        danmuList.Add(danmu1);
        danmu1.加速度 = 加速度;
        danmu1.角速度 = 角速度;
        return danmu1;
    }
    void 随机弹幕(int a)
    {
        GameObject danmu = Instantiate(danmuPrefab, transform.position, Quaternion.identity);
        //随机角度随机速度向量
        float angle = a * 66f % 360;
        Vector2 speed = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)) * 6.5f;
        // danmu.GetComponent<Danmu>().speed = speed * Random.Range(0.5f, 1);
        danmu.GetComponent<Danmu>().speed = speed * 1;
        danmuList.Add(danmu.GetComponent<Danmu>());

        danmu.GetComponent<Danmu>().onUpdate += (d) =>
        {
            if (player == null)
            {
                return;
            }
            //距离玩家太近  改变一次方向  根据玩家与弹幕夹角决定方向最多改变60角度
            if (d.f == 0 && Vector2.Distance(d.transform.position, player.transform.position) < 210)
            {

                d.f++;
                // Vector2 dir = (player.transform.position - d.transform.position).normalized;
                // d.speed = dir * d.speed.magnitude;

                Vector2 toPlayer = (player.transform.position - d.transform.position).normalized;
                float currentAngle = Mathf.Atan2(d.speed.y, d.speed.x) * Mathf.Rad2Deg;
                float targetAngle = Mathf.Atan2(toPlayer.y, toPlayer.x) * Mathf.Rad2Deg;
                float angleDiff = Mathf.DeltaAngle(currentAngle, targetAngle);
                angleDiff = Mathf.Clamp(angleDiff, -60, 60);
                float newAngle = currentAngle + angleDiff;
                d.speed = new Vector2(Mathf.Cos(newAngle * Mathf.Deg2Rad), Mathf.Sin(newAngle * Mathf.Deg2Rad)) * 1f;
                danmu.GetComponent<Danmu>().加速度 = 0.1f;

            }
        };
        //销毁
    }

    void 普通螺旋弹幕()
    {
        {
            GameObject danmu = Instantiate(danmuPrefab, transform.position, Quaternion.identity);
            //随机角度随机速度向量
            float angle = timer / 47f * 360 + 90;
            Vector2 speed = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad))
                * 6;
            danmu.GetComponent<Danmu>().speed = speed;
            danmuList.Add(danmu.GetComponent<Danmu>());

            // danmu.GetComponent<Danmu>().角速度 = -1;
            // danmu.GetComponent<Danmu>().加速度 = 0.02f;
            //销毁
        }
        {
            GameObject danmu = Instantiate(danmuPrefab, transform.position, Quaternion.identity);
            //随机角度随机速度向量
            float angle = -timer / 47f * 360 + 90;
            Vector2 speed = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad))
                            * 6;
            danmu.GetComponent<Danmu>().speed = speed;
            danmuList.Add(danmu.GetComponent<Danmu>());


            // danmu.GetComponent<Danmu>().角速度 = 1;
            // danmu.GetComponent<Danmu>().加速度 = 0.05f;
            //销毁
        }
    }
    void OnDestroy()
    {
        World.instance.OnPlayerR -= Chongzi;//当玩家死亡  重置
    }

    public void Chongzi()
    {
        传送门.SetActive(false);

        声音.Play音乐(oldClip);

        timer = 0;
        foreach (var danmu in danmuList)
        {
            if (danmu != null)
                GameObject.Destroy(danmu.gameObject);
        }
        foreach (var 变换 in 变换列表)
        {
            变换.Kill();
        }
    }
}
