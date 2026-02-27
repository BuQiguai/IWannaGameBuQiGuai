using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using System.Xml;
using UnityEngine.SceneManagement;

public class DifficultyMenu : MonoBehaviour
{
    int select = 0;
    Vector3 cursorStart;

    Text[] difficultyText = new Text[3];
    Text[] deathsText = new Text[3];
    Text[] timeText = new Text[3];

    public string difficultySelect;

    private void Start()
    {
        var cursor = GameObject.Find("Cursor");
        cursorStart = cursor.transform.position;

        for (var i = 0; i < 3; i++)
        {
            difficultyText[i] = GameObject.Find($"Difficulty{i + 1}").GetComponent<Text>();
            deathsText[i] = GameObject.Find($"Deaths{i + 1}").GetComponent<Text>();
            timeText[i] = GameObject.Find($"Time{i + 1}").GetComponent<Text>();

            if (!File.Exists(Application.persistentDataPath + $"Data/save{i + 1}"))
            {
                difficultyText[i].text = "No Data";
                deathsText[i].text = $"Deaths: 0";
                timeText[i].text = $"Time: 0:00:00";
            }
            else
            {
                var text = File.ReadAllText(Application.persistentDataPath + $"Data/save{i + 1}");
                var saveFile = JsonUtility.FromJson<SaveFile>(text);

                difficultyText[i].text = saveFile.difficulty.ToString();
                deathsText[i].text = $"Deaths: {saveFile.death}";
                timeText[i].text = $"Time: {saveFile.time / 3600}:{saveFile.time / 60 % 60}:{saveFile.time % 60}";
            }
        }
    }

    void FixedUpdate()
    {
        if (PlayInput.获取按键按下状态(KeyCode.D))
        {
            select++;
            if (select == 3)
                select = 0;
        }
        if (PlayInput.获取按键按下状态(KeyCode.A))
        {
            select--;
            if (select == -1)
                select = 2;
        }
        if (PlayInput.获取按键按下状态(KeyCode.J) || PlayInput.获取按键按下状态(KeyCode.J))
        {
            World.instance.gameStarted = true;
            World.instance.savenum = select + 1;
            SceneManager.LoadScene(difficultySelect);
        }
        var cursor = GameObject.Find("Cursor");
        cursor.transform.position = cursorStart + new Vector3(240f * select, 0);
    }


    public void 当按下按键(int 下标)
    {
        if (select == 下标)
        {
            //开始游戏
            World.instance.gameStarted = true;
            World.instance.savenum = select + 1;
            SceneManager.LoadScene(difficultySelect);
        }
        else
        {
            select = 下标;
        }
    }
}
