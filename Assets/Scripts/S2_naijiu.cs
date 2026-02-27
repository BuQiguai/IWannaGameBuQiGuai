using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class S2_naijiu : MonoBehaviour
{
    //弹幕预制件
    private Player player;
    public GameObject danmuPrefab;
    public GameObject danmuPrefab拖尾;
    public AudioClip bgmClip;
    AudioClip oldClip;

    public GameObject 传送门;
    //血条显示颜色
    public GameObject 耐久条;
    public SpriteRenderer 黑屏对象;
    // Start is called before the first frame update
    void Start()
    {
        World.instance.OnPlayerR += Chongzi;
    }

    void 播放bgm()
    {
        oldClip = 声音.getBGM();
        声音.Play音乐(bgmClip);
    }
    List<Danmu> danmuList = new List<Danmu>();

    List<Danmu> 临时单位组1 = new List<Danmu>();
    List<Danmu> 临时单位组2 = new List<Danmu>();
    List<Tween> 变换列表 = new List<Tween>();
    int timer = 0;
    void FixedUpdate()
    {
        float bgm长度 = bgmClip.length;
        timer++;
        float 当前进度 = (float)timer / 50 / bgm长度;

        当前进度 = 当前进度 > 1 ? 1 : 当前进度;

        Debug.Log("timer = " + timer);
        if (player == null)
        {
            player = FindObjectOfType<Player>();
            if (player == null)
            {
                return;
            }
        }
        耐久条.transform.localScale = new Vector3(1 - 当前进度, 0.72f, 1);
        if (timer / 50 >= bgm长度)
        {
            this.gameObject.SetActive(false);
            死亡运行();
        }
        帧率绑定音乐();
        if (timer == 10)
        {
            播放bgm();
            // 转跳耐久进度(5750);
            // 黑屏对象.DOFade(0.5f, 1);
        }
        if (timer == 50)
        {
            for (int i = 0; i < 20; i++)
            {
                var 弹幕 = 发射弹幕(new Vector3(0, 0, 0), Random.Range(0, 360), 0, 0, 0, Color.black);
                变换列表.Add(弹幕.transform.DOMove(new Vector3(i * 20, 0, 0), 1f));
                var 弹幕2 = 发射弹幕(new Vector3(0, 0, 0), Random.Range(0, 360), 0, 0, 0, Color.black);
                变换列表.Add(弹幕2.transform.DOMove(new Vector3(i * -20, 0, 0), 1f));
                临时单位组1.Add(弹幕);
                临时单位组2.Add(弹幕2);
            }
        }

        if (timer == 130)
        {
            var i = 0;
            计时器.执行(0.08f, () =>
            {
                发射弹幕(临时单位组1[i].transform.position, Random.Range(0, 360), 5, 0, 0, Color.black);
                发射弹幕(临时单位组2[i].transform.position, Random.Range(0, 360), 5, 0, 0, Color.black);
                发射弹幕(临时单位组1[i].transform.position, Random.Range(0, 360), 5, 0, 0, Color.black);
                发射弹幕(临时单位组2[i].transform.position, Random.Range(0, 360), 5, 0, 0, Color.black);
                i += 1;
            }, 19);
        }
        if (timer == 270)
        {
            for (int i = 0; i <= 10; i++)
            {
                发射弹幕(new Vector3(400 - 32, 0, 0), i * 18f + 90, 8, 0, 0, Color.black);
            }
            for (int i = 0; i <= 10; i++)
            {
                发射弹幕(new Vector3(-400 + 32, 0, 0), i * 18f + 270, 8, 0, 0, Color.black);
            }
        }
        if (timer == 350)
        {
            临时单位组1.ForEach(danmu =>
            {
                danmu.speed = new Vector2(0, 1);
                danmu.加速度 = 0.1f;
            });
            临时单位组2.ForEach(danmu =>
            {
                danmu.speed = new Vector2(0, -1);
                danmu.加速度 = 0.1f;
            });
            临时单位组1.Clear();
            临时单位组2.Clear();
        }

        if (timer == 450)
        {
            for (int i = 0; i < 20; i++)
            {
                var 弹幕 = 发射弹幕(new Vector3(0, 0, 0), i * 18, 8, 0, 0, Color.black);
                // 变换列表.Add(弹幕.transform.DOMove(极坐标转为坐标(i * 18, 100), 3f));
            }
        }
        if (timer == 550)
        {
            for (int i = 0; i < 20; i++)
            {
                var 弹幕 = 发射弹幕(new Vector3(0, 0, 0), i * 18, 6, 0, 0, Color.black);
            }
        }

        if (timer == 650)
        {
            for (int i = 0; i < 20; i++)
            {
                var 弹幕 = 发射弹幕(new Vector3(0, 0, 0), i * 18, 4, 0, 0, Color.black);
            }
        }
        if (timer == 750)
        {
            临时单位组2.Clear();
            变换列表.Add(黑屏对象.DOFade(1, 2f));
        }

        if (timer == 860)
        {
            黑屏对象.color = new Color(0.8f, 0.8f, 0.8f, 1);
            变换列表.Add(黑屏对象.DOFade(0, 0.5f));
            计时器.执行(0.2f, () =>
            {
                临时单位组2.Add(发射弹幕(new Vector3(400, Random.Range(-300, 300), 0), 180 + Random.Range(45, -45), 5, 0, 0, Color.black));
                临时单位组2.Add(发射弹幕(new Vector3(-400, Random.Range(-300, 300), 0), Random.Range(45, -45), 5, 0, 0, Color.black));
            }, 14);
        }
        if (timer == 1080)
        {
            临时单位组2.ForEach(d =>
            {
                if (d == null) return;
                //朝向玩家移动
                //不在屏幕内的不移动
                if (d.transform.position.x < -400 || d.transform.position.x > 400 || d.transform.position.y < -300 || d.transform.position.y > 300) return;
                d.speed = (player.transform.position - d.transform.position).normalized * 0.1f;
                d.加速度 = 0.05f;
            });
        }
        if (timer == 1250)
        {
            计时器.执行(0.04f, () =>
            {
                临时单位组1.Add(发射弹幕(new Vector3(Random.Range(-400, 400), -300, 0), 90, 1.5f, 0, 0, Color.black));
            }, 60);
        }
        if (timer == 1400)
        {
            临时单位组1.ForEach(d =>
            {
                //随机扩散
                d.speed = 极坐标转为坐标(Random.Range(0, 360), 5 * Random.Range(0.4f, 1));
            });
        }
        // if (timer == 1550)
        // {
        //     for (int i = 0; i < 20; i++)
        //     {
        //         临时单位组1.Add(发射弹幕(new Vector3(284,112,0), i * 18, 5, 0, 0, Color.black));
        //     }
        //     for (int i = 0; i < 10; i++)
        //     {
        //         临时单位组1.Add(发射弹幕(new Vector3(284,112,0), i * 36+9, 8, 0, 0, Color.black));
        //     }
        // }
        if (timer == 1600)
        {
            var i = 0;
            计时器.执行(0.02f, () =>
            {
                临时单位组1.Add(发射弹幕(new Vector3(324, -56, 0), i++ * 186, 6f, 0, i % 2 == 0 ? -0.5f : 0.5f, Color.black));
            }, 240);
        }
        if (timer == 2000)
        {
            for (int j = 0; j < 4; j++)
            {
                for (int i = 0; i < 20; i++)
                {
                    临时单位组1.Add(发射弹幕(极坐标转为坐标(j * 90, 100), i * 18, 1f, 0, 0, Color.black));
                }
            }

        }
        if (timer == 2170)
        {
            临时单位组1.ForEach(d =>
            {
                if (d == null) return;
                //朝向屏幕中心移动
                d.speed = d.transform.position.normalized * -0.1f;
                d.加速度 = 0.05f;
            });

        }
        if (timer == 2400)
        {
            for (int i = 0; i < 20; i++)
            {
                int 临时 = i;
                发射弹幕(new Vector3(0, 0), i * 18, 1f, 0, 0, Color.black).onUpdate += (d) =>
                {
                    d.f++;
                    if (d.f == 120 + 临时 * 2)
                    {
                        for (int i = -2; i < 3; i++)
                        {
                            //中心朝向玩家
                            var 与玩家角度 = Mathf.Atan2(player.transform.position.y - d.transform.position.y, player.transform.position.x - d.transform.position.x) * Mathf.Rad2Deg;
                            发射弹幕(d.transform.position, i * 72 + 与玩家角度, 8f, 0, 0, Color.black);
                        }
                        Destroy(d.gameObject);
                    }
                };
            }
        }
        if (timer == 2650)
        {
            黑屏对象.color = new Color(0f, 0f, 0f, 0);
            变换列表.Add(黑屏对象.DOFade(0.6f, 1f));
            var ii = 0;
            计时器.执行(1f, () =>
            {
                ii++;
                var 随机位置 = new Vector3(Random.Range(-400, 400), 200, 0);
                for (int i = 0; i < 20; i++)
                {
                    var 弹幕 = 发射弹幕(随机位置, i * 18, 8f, 0.07f, ii % 2 == 0 ? -1.5f : 1.5f, Color.white, true);
                    //修改图层为最上层
                    //修改排序图层
                    弹幕.GetComponent<SpriteRenderer>().sortingOrder = 100;
                    弹幕.GetComponent<SpriteRenderer>().sortingLayerName = "Top"; // 或者你想要的具体图层名称
                }
            }, 10);
        }
        if (timer == 3200)
        {
            var 速度 = 2f;
            计时器.执行(0.04f, () =>
            {
                速度 += 0.1f;
                var 弹幕 = 发射弹幕(new Vector3(0, 0, 0), Random.Range(0, 360), 速度, 0, 0, Color.white);
                弹幕.GetComponent<SpriteRenderer>().sortingOrder = 100;
                弹幕.GetComponent<SpriteRenderer>().sortingLayerName = "Top"; // 或者你想要的具体图层名称
            }, 100);
        }
        if (timer == 3400)
        {
            计时器.执行(1f, () =>
            {
                var 随机位置 = new Vector3(400, Random.Range(-300, 300), 0);
                var 随机位置2 = new Vector3(400, Random.Range(-300, 300), 0);
                var 弹幕 = 发射弹幕(随机位置, 180, 8f, 0, 0, Color.red);
                弹幕.onUpdate += (d) =>
                {
                    d.f++;
                    if (d.f == 25)
                    {
                        for (int i = 0; i < 18; i++)
                        {
                            //中心朝向玩家
                            var 与玩家角度 = Mathf.Atan2(player.transform.position.y - d.transform.position.y, player.transform.position.x - d.transform.position.x) * Mathf.Rad2Deg;
                            var 弹幕 = 发射弹幕(d.transform.position, i * 20 + 与玩家角度, 8f, 0, 0, Color.red, true);
                            弹幕.GetComponent<SpriteRenderer>().sortingOrder = 100;
                            弹幕.GetComponent<SpriteRenderer>().sortingLayerName = "Top"; // 或者你想要的具体图层名称
                        }
                        Destroy(d.gameObject);
                    }
                };
                var 弹幕2 = 发射弹幕(随机位置2, 180, 8f, 0, 0, new Color(0.3f, 0.3f, 1, 1));
                弹幕2.onUpdate += (d) =>
                {
                    d.f++;
                    if (d.f == 25)
                    {
                        for (int i = 0; i < 18; i++)
                        {
                            //中心朝向玩家
                            var 与玩家角度 = Mathf.Atan2(player.transform.position.y - d.transform.position.y, player.transform.position.x - d.transform.position.x) * Mathf.Rad2Deg;
                            var 弹幕 = 发射弹幕(d.transform.position, i * 20 + 与玩家角度, 8f, 0, 0, new Color(0.3f, 0.3f, 1, 1), true);
                            弹幕.GetComponent<SpriteRenderer>().sortingOrder = 100;
                            弹幕.GetComponent<SpriteRenderer>().sortingLayerName = "Top"; // 或者你想要的具体图层名称
                        }
                        Destroy(d.gameObject);
                    }
                };
                //修改图层为最上层
                //修改排序图层
                弹幕.GetComponent<SpriteRenderer>().sortingOrder = 100;
                弹幕.GetComponent<SpriteRenderer>().sortingLayerName = "Top"; // 或者你想要的具体图层名称
                弹幕2.GetComponent<SpriteRenderer>().sortingOrder = 100;
                弹幕2.GetComponent<SpriteRenderer>().sortingLayerName = "Top"; // 或者你想要的具体图层名称
            }, 7);
        }
        if (timer == 3850)
        {
            var 速度 = 3f;
            计时器.执行(0.05f, () =>
            {
                速度 += 0.1f;
                if (速度 > 10)
                {
                    速度 = 10;
                }
                var 弹幕 = 发射弹幕(new Vector3(324, -64, -1.40934908f), Random.Range(0, 360), 速度, 0, 0, Color.red);
                弹幕.GetComponent<SpriteRenderer>().sortingOrder = 100;
                弹幕.GetComponent<SpriteRenderer>().sortingLayerName = "Top"; // 或者你想要的具体图层名称
                var 弹幕2 = 发射弹幕(new Vector3(204, -52, -1.40934908f), Random.Range(0, 360), 速度, 0, 0, new Color(0.3f, 0.3f, 1, 1));
                弹幕2.GetComponent<SpriteRenderer>().sortingOrder = 100;
                弹幕2.GetComponent<SpriteRenderer>().sortingLayerName = "Top"; // 或者你想要的具体图层名称
            }, 100);
        }

        if (timer == 4200)
        {
            var 角度 = 0;
            计时器.执行(0.04f, () =>
            {
                角度 += 94;
                var 缓存角度 = 角度;
                var 弹幕 = 发射弹幕(new Vector3(0, 0, 0), 角度, 8f, 0, 0, Color.yellow);
                弹幕.onUpdate += (d) =>
                {
                    d.f++;
                    if (d.f == 25)
                    {
                        d.speed = 极坐标转为坐标(缓存角度 + 120, 1f);
                        d.加速度 = 0.1f;
                    }
                };
                弹幕.GetComponent<SpriteRenderer>().sortingOrder = 100;
                弹幕.GetComponent<SpriteRenderer>().sortingLayerName = "Top"; // 或者你想要的具体图层名称
            }, 90);
            计时器.执行(5.5f, () =>
            {
                for (int i = 0; i < 360; i += 15)
                {
                    var 弹幕 = 发射弹幕(极坐标转为坐标(i + 0, 20f * 32), i, 8f, 0, 0, Color.white);
                    弹幕.自动销毁 = false;
                    弹幕.speed = 极坐标转为坐标(i + -180 + 25, 10f);
                    弹幕.GetComponent<SpriteRenderer>().sortingOrder = 100;
                    弹幕.GetComponent<SpriteRenderer>().sortingLayerName = "Top"; // 或者你想要的具体图层名称
                    Destroy(弹幕.gameObject, 5f);
                }
            }, 1);
        }

        if (timer == 4500)
        {
            var 角度1 = 0f;
            计时器.执行(0.04f, () =>
            {
                var 角度 = 角度1 += 60f;
                var 弹幕 = 发射弹幕(new Vector3(324, -64, -1.40934908f) + 极坐标转为坐标(角度 + 0, 30f * 32), 角度, 8f, 0, 0, Color.yellow);
                弹幕.自动销毁 = false;
                弹幕.speed = 极坐标转为坐标(角度 + -180, 6f);
                弹幕.加速度 = 0.1f;
                弹幕.GetComponent<SpriteRenderer>().sortingOrder = 100;
                弹幕.GetComponent<SpriteRenderer>().sortingLayerName = "Top";
                弹幕.onUpdate += (d) =>
                {
                    if (d.f == 0 && Vector2.Distance(d.transform.position, new Vector3(324, -64, -1.40934908f)) < 128f)
                    {
                        d.speed = 极坐标转为坐标(Random.Range(0, 360), 1f);
                        d.f = 1;
                        Destroy(d, 4f);
                    }
                };
            }, 150);
        }
        if (timer == 4950)
        {
            黑屏对象.DOFade(0f, 1);
        }
        if (timer == 5000)
        {

            计时器.执行(0.15f, () =>
            {
                var 颜色 = new Color[] { Color.red, Color.green, Color.blue, Color.yellow, Color.gray, Color.white };
                {
                    var 随机颜色 = 颜色[Random.Range(0, 颜色.Length)];
                    发射弹幕(new Vector3(400, Random.Range(-300, 300), 0), Random.Range(90, 270), 5f, 0, 0, 随机颜色);
                }
                {
                    var 随机颜色 = 颜色[Random.Range(0, 颜色.Length)];
                    发射弹幕(new Vector3(-400, Random.Range(-300, 300), 0), Random.Range(270, 360 + 90), 5f, 0, 0, 随机颜色);
                }
                {
                    var 随机颜色 = 颜色[Random.Range(0, 颜色.Length)];
                    发射弹幕(new Vector3(Random.Range(-400, 400), 300, 0), Random.Range(180, 360), 5f, 0, 0, 随机颜色);
                }
            }, 35);
        }

        if (timer == 5370)
        {
            //五角星形状扩散弹幕
            for (int i = 0; i < 360; i += 72)
            {
                for (int j = 0; j < 320; j += 18)
                {
                    var 五角星坐标 = 极坐标转为坐标(i + 90, 5 * 32f) + 极坐标转为坐标(i + 180 - 18 + 90, j);
                    var 弹幕 = 发射弹幕(new Vector3(0, 0, 0), 0, 0, 0, 0, Color.red);
                    弹幕.speed = 五角星坐标 * 0.01f;
                    临时单位组1.Add(弹幕);
                }
            }
        }
        if (timer == 5570)
        {
            临时单位组1.ForEach(d =>
            {
                d.speed *= -4;
            });
        }

        if (timer == 5750)
        {
            var i = 0.1f;
            var j = 1f;
            计时器.执行(0.08f, () =>
            {
                i += 2*j;
                for (int j = 0; j < 3; j++)
                {
                    var 位置 = 极坐标转为坐标(-i * 9 + j * 120, 4 * 32f);
                    var 弹幕 = 发射弹幕(位置, 0, 0, 0, 0, Color.yellow);
                    弹幕.speed = -弹幕.transform.position.normalized * 5;
                }
            }, 140);

            计时器.执行(6f, () =>
            {
                j *= -0.5f;
            },1);
        }
        if (timer == 6430)
        {
            var 角度 = 20;
            计时器.执行(0.4f, () =>
            {
                角度 -= 2;
                for (int i = 0; i < 360; i += 角度)
                {
                    var 弹幕 = 发射弹幕(new Vector3(0, 0, 0), i,7f+Random.Range(0,4), 0, 0, Color.red);
                }
            }, 4);
        }
    }

    void 帧率绑定音乐()
    {
        var a = 声音.getBGMY();
        if (a != null)
        {
            //获取当前播放进度
            var 进度 = a.time / a.clip.length;
            float bgm长度 = bgmClip.length;
            //设置bgm同步进度
            float 当前进度 = (float)timer / 50 / bgm长度;
            if (进度 - 当前进度 > 0.01f)
            {
                当前进度 = 当前进度 > 1 ? 1 : 当前进度;
                声音.设置音乐进度(当前进度);
            }

        }

    }

    void 转跳耐久进度(int 帧数)
    {
        timer = 帧数;
        float bgm长度 = bgmClip.length;
        //设置bgm同步进度
        float 当前进度 = (float)timer / 50 / bgm长度;

        当前进度 = 当前进度 > 1 ? 1 : 当前进度;
        声音.设置音乐进度(当前进度);

    }

    Vector3 极坐标转为坐标(float angle, float radius)
    {
        return new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad) * radius, Mathf.Sin(angle * Mathf.Deg2Rad) * radius, 0);
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

        传送门.SetActive(true);
    }

    Color 随机颜色()
    {
        return new Color(Random.Range(0, 1f), Random.Range(0, 1f), Random.Range(0, 1f));
    }

    Danmu 发射弹幕(Vector3 position, float angle, float speed, float 加速度 = 0, float 角速度 = 0, Color color = new Color(), bool is拖尾 = false)
    {
        GameObject danmu;
        if (is拖尾)
        {
            danmu = Instantiate(danmuPrefab拖尾, position, Quaternion.identity);
        }
        else
        {
            danmu = Instantiate(danmuPrefab, position, Quaternion.identity);
        }

        //随机角度随机速度向量
        Vector2 speed2 = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)) * speed;
        var danmu1 = danmu.GetComponent<Danmu>();
        danmu1.speed = speed2;
        danmuList.Add(danmu1);
        danmu1.加速度 = 加速度;
        danmu1.角速度 = 角速度;
        danmu1.GetComponent<SpriteRenderer>().color = color;
        return danmu1;
    }

    void OnDestroy()
    {
        World.instance.OnPlayerR -= Chongzi;//当玩家死亡  重置
    }

    public void Chongzi()
    {
        传送门.SetActive(false);
        声音.Play音乐(oldClip);
        黑屏对象.color = new Color(0, 0, 0, 0);
        timer = 0;
        foreach (var danmu in danmuList)
        {
            if (danmu != null)
                GameObject.Destroy(danmu.gameObject);
        }
        临时单位组1.Clear();
        临时单位组2.Clear();
        foreach (var 变换 in 变换列表)
        {
            变换.Kill();
        }
    }
}
