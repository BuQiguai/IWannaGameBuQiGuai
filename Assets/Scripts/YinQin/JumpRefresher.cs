using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpRefresher : MonoBehaviour
{
    public int refreshTime = 100;

    int timer = -1;

    void Start()
    {
        World.instance.OnPlayerR += 重置;
    }
    void 重置() {
        timer = -1;
        GetComponent<SpriteRenderer>().enabled = true;
    }

    void OnDestroy()
    {
        World.instance.OnPlayerR -= 重置;
    }
    private void FixedUpdate()
    {
        if (GetComponent<SpriteRenderer>().enabled)
        {
            var player = GetComponent<PixelPerfectCollider>().InstancePlace(transform.position.x, transform.position.y, "Player");
            if (player != null)
            {
                player.GetComponent<Player>().djump = true;
                GetComponent<SpriteRenderer>().enabled = false;
                timer = refreshTime;
            }
        }
        if (timer != -1)
        {
            if (timer-- == 0)
            {
                timer = -1;
                GetComponent<SpriteRenderer>().enabled = true;
            }
        }
    }
}
