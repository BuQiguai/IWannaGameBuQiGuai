using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChuFa_Boss : DongZuoBase
{
    public GameObject Boss;
    public GameObject Boss血条;
    public GameObject Boss血;
    public override void 动作()
    {
        var player = GameObject.FindObjectOfType<Player>();
        if (player != null)
        {
            World.instance.SaveGame(true);
        }
        Debug.Log("触发了boss");
        Boss血.GetComponent<SpriteRenderer>().color = new Color(0.6f, 0, 0, 0);
        Boss.SetActive(true);
        Boss血条.SetActive(true);
        gameObject.SetActive(false);
    }

    protected override void 重置()
    {
        Boss.transform.position = new Vector3(-2, 376, 0);
        Boss.SetActive(false);
        Boss血条.SetActive(false);
        gameObject.SetActive(true);
        Boss血.GetComponent<SpriteRenderer>().color = new Color(0.6f, 0, 0, 0);

    }
}
