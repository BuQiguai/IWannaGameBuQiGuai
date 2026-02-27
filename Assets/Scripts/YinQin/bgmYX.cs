using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[DefaultExecutionOrder(100)] // 优先级越小，越早执行
public class bgmYX : MonoBehaviour
{
    public AudioClip bgm;


    //开始延迟一帧运行
    void Start()
    {
        声音.Play音乐(bgm);

    }

    // IEnumerator PlayBGM()
    // {
    //     yield return new WaitForEndOfFrame();
    //     if (World.instance.bgm.clip != bgm)
    //     {
        // World.instance.bgm.Stop();
        // World.instance.bgm.clip = bgm;
        // World.instance.bgm.Play();
    //     }

    // }

    // Update is called once per frame
    void Update()
    {
        
    }
}
