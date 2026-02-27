using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleMenu : MonoBehaviour
{
    public string menu;

    void FixedUpdate()
    {
        if (PlayInput.获取按键按下状态(KeyCode.J) || PlayInput.获取按键按下状态(KeyCode.J))
        {
            开始游戏();
        }
    }

    public void 开始游戏()
    {
        SceneManager.LoadScene(menu);
    }
}
