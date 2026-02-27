using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class s2小boss : DongZuoBase
{
    public GameObject danmuPrefab;
    public GameObject[] 消失的单位;
    public GameObject[] 出现的单位;
    public GameObject 传送门;
    public GameObject 存档;
        public GameObject 黑屏;
        public GameObject 贴图单位;
    List<Danmu> danmuList = new List<Danmu>();
    List<Tween> 变换列表 = new List<Tween>();
    List<Danmu> 临时单位组1 = new List<Danmu>();

    BOSS boss;

    protected override void 初始运行动作()
    {
        boss = GetComponent<BOSS>();

        boss.受到攻击 += 受到攻击;
        boss.死亡 += 死亡运行;

    }

    protected override void 销毁运行动作(){
        boss.受到攻击 -= 受到攻击;
        boss.死亡 -= 死亡运行;
    }


    void 受到攻击()
    {
        var jiaodu = Random.Range(0, 360);
        for (int i = 0; i < 10; i++)
        {
            发射弹幕(transform.position+new Vector3(50,-50), jiaodu+36*i, Random.Range(2f, 7f), 0, 0, Color.yellow);
        }
    }

    void 死亡运行()
    {
        传送门.SetActive(true);
        黑屏.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0);
        计时器.清除所有计时器();
    }
    public override void 动作()
    {
        System.Array.ForEach(出现的单位, u => u.SetActive(true));

        var 黑屏图片 = 黑屏.GetComponent<SpriteRenderer>();
        黑屏图片.color = new Color(0, 0, 0, 1);
        黑屏图片.DOFade(0.5f, 0.7f);
        for (int i = 0; i < 30; i++)
        {
            var danmu = 发射弹幕(transform.position+new Vector3(50,-50), 12*i, 3f, 0, 0, Color.black);
            danmu.加速度 = 0.3f;
        }
        this.存档.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
        this.gameObject.SetActive(true);
        this.贴图单位.GetComponent<SpriteRenderer>().DOFade(1, 1.0f);
        变换列表.Add(this.存档.GetComponent<SpriteRenderer>().DOFade(0, 1.0f));
        计时器.执行(1.0f, () =>
        {
            this.存档.SetActive(false);
            计时器.执行(0.8f, () =>
            {
                for (int i = 0; i < 20; i++)
                {
                    发射弹幕(transform.position+new Vector3(50,-50), 18*i, Random.Range(6f, 7f), 0, 0, Color.yellow);
                }
            }, 19);

            计时器.执行(5f,()=>{
                //boss移动
                变换列表.Add( this.transform.DOMoveX(763f+(Random.value >0.5? Random.Range(-200, -100):Random.Range(100, 200)),1f).SetEase(Ease.OutQuad).OnComplete(()=>{
                    
                    var player = FindObjectOfType<Player>();
                    float 角度 = Mathf.Atan2(player.transform.position.y - transform.position.y, player.transform.position.x - transform.position.x) * Mathf.Rad2Deg;
                    for (int i = 0; i < 5; i++)
                    {
                        
                        发射弹幕(transform.position + new Vector3(50, -50), 角度 - 12 + 6 * i, 6, 0, 0, Color.red);
                    }


                    变换列表.Add( this.transform.DOMoveX(763f, 1f).SetEase(Ease.OutQuad));
                })
                ); 
            },2);
            计时器.执行(17f, () =>
            {
                计时器.执行(0.3f, () =>
                {
                    for (int i = 0; i < 10; i++)
                    {
                        发射弹幕(transform.position + new Vector3(50, -50), 36 * i, 6, 0, 0, Color.red);
                    }
                },10);
            },1);
            计时器.执行(18f, () =>
            {
                //移动到地上 
                变换列表.Add(this.transform.DOMoveY(484, 2f).SetEase(Ease.InOutSine).OnComplete(() =>
                {
                计时器.执行(1f, () =>
                    {
                        for (int i = 0; i < 10; i++)
                        {
                            发射弹幕(transform.position + new Vector3(50, -50), 36 * i, Random.Range(6f, 7f), 0, 0, Color.red);
                        }
                    });
                }));


            }, 1);

        }, 1);

    }

    protected override void 重置()
    {
        Debug.Log("重置");
        this.gameObject.SetActive(false);
        传送门.SetActive(false);
        黑屏.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0);
        this.贴图单位.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
        存档.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
        存档.SetActive(true);
        System.Array.ForEach(消失的单位, u => u.SetActive(true));
        System.Array.ForEach(出现的单位, u => u.SetActive(false));
        danmuList.ForEach(d => { if (d != null && d.gameObject != null) Destroy(d.gameObject); });
        danmuList.Clear();
        临时单位组1.Clear();
        计时器.清除所有计时器();
        变换列表.ForEach(t =>
        {
            t?.Kill();
        });
        变换列表.Clear();
        this.transform.position = new Vector3(763.210083f,785.177856f,0);
    }

    // Update is called once per frame
    void Update()
    {

    }
    Danmu 发射弹幕(Vector3 position, float angle, float speed, float 加速度 = 0, float 角速度 = 0, Color color = new Color())
    {
        GameObject danmu;
        
        danmu = Instantiate(danmuPrefab, position, Quaternion.identity);
        

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

}
