using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Mathf;

public class Blood : MonoBehaviour
{


    new SpriteRenderer renderer;
    void Start()
    {
        renderer = GetComponent<SpriteRenderer>();
        World.instance.OnPlayerR += xiaohui;
    }
    void xiaohui()
    {
        GameObject.Destroy(gameObject);
    }
    void OnDestroy()
    {
        World.instance.OnPlayerR -= xiaohui;
    }
    void FixedUpdate()
    {
        transform.localScale += new Vector3(0.1f, 0.1f);
        renderer.color = new Color(1, 0, 0, renderer.color.a - 0.02f);

        transform.Rotate(new Vector3(0, 0, 10));
        if (renderer.color.a <= 0)
        {
            GameObject.Destroy(gameObject);
        }

    }
}
