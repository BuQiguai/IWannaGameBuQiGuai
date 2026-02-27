using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class 小耐久3 : DongZuoBase
{
    public GameObject danmuPrefab;
    public GameObject[] 消失的单位;
    public GameObject[] 出现的单位;
    public GameObject 存档;
    List<Danmu> danmuList = new List<Danmu>();
    List<Tween> 变换列表 = new List<Tween>();
    List<Danmu> 临时单位组1 = new List<Danmu>();
    public override void 动作()
    {
        this.存档.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
        变换列表.Add(this.存档.GetComponent<SpriteRenderer>().DOFade(0, 1.0f));
        计时器.执行(1.0f, () =>
        {
            this.存档.SetActive(false);
            计时器.执行(0.05f, () =>
            {
                发射弹幕(transform.position, Random.Range(0, 360), Random.Range(2f, 3f), 0, 0, Color.white);
            }, 200);
            计时器.执行(1f, () =>
            {
                // 向玩家方向发射弹幕
                var player = FindObjectOfType<Player>();
                if (player != null)
                {
                var 角度 = Mathf.Atan2(player.transform.position.y - transform.position.y, player.transform.position.x - transform.position.x) * Mathf.Rad2Deg;

                发射弹幕(transform.position, 角度, 5, 0, 0, Color.red);
                }


            }, 10);


            计时器.执行(12f, () =>
            {
                System.Array.ForEach(消失的单位, u => u.SetActive(false));
                System.Array.ForEach(出现的单位, u => u.SetActive(true));
                // 消失的单位.SetActive(false);
            }, 1);
        }, 1);

    }

    protected override void 重置()
    {
        Debug.Log("重置小耐久");
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
