using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BOSS : MonoBehaviour
{
    private PixelPerfectCollider collider;
    private SpriteRenderer spriteRenderer;
    public int health = 10;
    public int 无敌时间 = 20;
    private int timer = 0;

    public int maxhealth = 0;

    public event System.Action 受到攻击;
    public event System.Action 死亡;
     
    public GameObject 血条;
    public GameObject boss贴图;
    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if(boss贴图!=null)
        {
            spriteRenderer = boss贴图.GetComponent<SpriteRenderer>();
        }
        collider = GetComponent<PixelPerfectCollider>();
        timer = 无敌时间;
                //当玩家死亡  重置
        World.instance.OnPlayerR += 重置血量;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        血条.transform.localScale = new Vector3(health * 1.0f / maxhealth, 1, 1);
        if (collider.PlaceMeeting(transform.position.x, transform.position.y, "Bullet"))
        {
            if (timer <= 0)
            {
                timer = 无敌时间;
                health--;
                Debug.Log("BOSS Health: " + health);
                if (health > 0)
                {
                    受到攻击?.Invoke();
                    // Destroy(this.gameObject);
                }
            }
        }
        if (timer > 0)
        {
            timer--;
            spriteRenderer.color = new Color(1, 1, 1, 0.5f);
        }
        else
        {
            spriteRenderer.color = new Color(1, 1, 1, 1f);
        }

        if (health <= 0)
        {
            死亡?.Invoke();
            // GameObject.Destroy(gameObject);
            gameObject.SetActive(false);
            
            血条.SetActive(false);
        }
    }
    void OnDestroy()
    {
        World.instance.OnPlayerR -= 重置血量;
    }
    void 重置血量()
    {
        health = maxhealth;
        timer = 无敌时间;
    }
}
