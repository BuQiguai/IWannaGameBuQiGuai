using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class UI玩家 : MonoBehaviour
{
    public GameObject 设置按钮;
    public GameObject 设置界面;
    public GameObject 调按键界面;
    public GameObject 帮助信息;
    public GameObject 虚拟按键;
    public GameObject 统计信息;
    public Toggle 音乐;
    public Toggle 音效;

    public Text 玩家死亡次数;
    public Text 玩家总时间;
    public static bool 音乐开关 = true;
    public static bool 音效开关 = true;

    public static bool 是否正在调按键 = false;

    void FixedUpdate()
    {
        if (World.instance != null)
        {
            玩家死亡次数.text = " " + World.instance.death;
            // 玩家总时间.text = ":" + World.instance.time;
            //时间显示为时间格式  time为浮点数 转换为00:00:00格式
            玩家总时间.text = " " + System.TimeSpan.FromSeconds(World.instance.time).ToString(@"hh\:mm\:ss");
            if (World.instance.gameStarted)
            {
                统计信息.SetActive(true);
                虚拟按键.SetActive(false);
                设置按钮.SetActive(true);
            }
            else
            {
                统计信息.SetActive(false);
                虚拟按键.SetActive(false);
                设置按钮.SetActive(false);
            }
        }


    }
    public void onclick设置按键()
    {
        设置按钮.SetActive(false);
        虚拟按键.SetActive(false);
        设置界面.SetActive(true);
        统计信息.SetActive(false);
    }
    public void onclick帮助信息()
    {
        设置按钮.SetActive(false);
        虚拟按键.SetActive(false);
        设置界面.SetActive(false);
        帮助信息.SetActive(true);
                统计信息.SetActive(false);
    }
    public void onclick调按键()
    {
        设置按钮.SetActive(false);
        虚拟按键.SetActive(false);
        设置界面.SetActive(false);
        调按键界面.SetActive(true);
        虚拟按键.SetActive(true);
                统计信息.SetActive(false);
        是否正在调按键 = true;
    }
    public void onclick返回()
    {
        设置按钮.SetActive(true);
        虚拟按键.SetActive(true);
        设置界面.SetActive(false);
        调按键界面.SetActive(false);
        帮助信息.SetActive(false);
        统计信息.SetActive(true);
        是否正在调按键 = false;
    }

    public void onclick音乐()
    {
        音乐开关 = 音乐.isOn;

        //Debug.Log(音乐开关);
        if (音乐开关)
        {
            声音.设置音乐音量(0.5f);
        }
        else
        {
            声音.设置音乐音量(0);
        }
    }
    public void onclick音效()
    {
        音效开关 = 音效.isOn;
        //Debug.Log(音效开关);
        if (音效.isOn)
        {
            声音.设置音效音量(1);
        }
        else
        {
            声音.设置音效音量(0);
        }
    }
    public void onclick重置()
    {
        PlayInput.重置按键位置();
        Debug.Log("重置布局!");

    }

    public void onclick保存按键布局()
    {
        PlayInput.保存按键位置();
        Debug.Log("保存布局!");
        onclick返回();
        //Debug.Log(音效开关);
    }
}
