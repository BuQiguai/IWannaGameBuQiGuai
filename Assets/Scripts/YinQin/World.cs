using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.SceneManagement;

// World singleton helps us manage the game
[DefaultExecutionOrder(-100)] // 优先级越小，越早执行
public class World : Singleton<World>
{
    static World instance;
    public int savenum = 1;
    public Difficulty difficulty = Difficulty.Medium;
    public enum Difficulty
    {
        Medium = 0,
        Hard = 1,
        VeryHard = 2,
        Impossible = 3,
    }
    public int death = 0;
    public float time = 0;
    public int grav = 1;

    public bool gameStarted = false;
    public bool autosave = false;
    public string startScene = "Stage01";

    public string saveScene;
    public float savePlayerX;
    public float savePlayerY;
    public int saveGrav;

    public Player playerPrefab;

    // May move these to separate class
    public Dictionary<Texture2D, MaskData> maskDataManager = new Dictionary<Texture2D, MaskData>();
    public Dictionary<string, List<PixelPerfectCollider>> colliders = new Dictionary<string, List<PixelPerfectCollider>>();

    //死亡重置事件
    public event Action OnPlayerR;
    SaveFile saveFile;

    void Start()
    {
        instance = this;
        // Initialize game
        //默认播放bgm
        声音.Play音乐(声音.MusicTrack.默认BGM);

        //如果是编辑器内 开启垂直同步
#if UNITY_EDITOR
        QualitySettings.vSyncCount = 1;
        Application.targetFrameRate = 120;
#else
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 120;
#endif
        // QualitySettings.vSyncCount = 0;
        // Application.targetFrameRate = 120;
        // Application.targetFrameRate = 30;

    }

    void FixedUpdate()
    {

        if (gameStarted)
        {
            time += 0.02f;
            // Restart game
            if (PlayInput.获取按键按下状态(KeyCode.R))
            {
                // World.instance.KillPlayer();
                SaveGame(false);
                // LoadGame(false);

                fuhuoPlayer();

                Debug.Log("Restart");
                OnPlayerR?.Invoke();
            }

        }
    }

    public void fuhuoPlayer()
    {

        // Debug.Log("Fuhuo");
        foreach (var p in GameObject.FindObjectsOfType<Player>())
            GameObject.Destroy(p.gameObject);

        var player = GameObject.Instantiate<Player>(playerPrefab);
        player.gameObject.transform.position = new Vector3(savePlayerX, savePlayerY);
        grav = saveGrav;
    }

    /// <summary>
    /// 加载游戏方法，根据参数决定是否从文件加载游戏数据
    /// </summary>
    /// <param name="loadFile">是否从文件加载游戏数据的布尔值</param>
    public void LoadGame(bool loadFile)
    {
        if (loadFile)
        {
            var saveJson = File.ReadAllText(Application.persistentDataPath + $"Data/save{savenum}");
            var saveFile = JsonUtility.FromJson<SaveFile>(saveJson);

            instance.death = saveFile.death;
            time = saveFile.time;

            difficulty = saveFile.difficulty;
            saveScene = saveFile.scene;

            savePlayerX = saveFile.playerX;
            savePlayerY = saveFile.playerY;
            saveGrav = saveFile.playerGrav;
        }
        gameStarted = true;
        autosave = false;
        grav = saveGrav;

        foreach (var p in GameObject.FindObjectsOfType<Player>())
            GameObject.Destroy(p.gameObject);

        var player = GameObject.Instantiate<Player>(playerPrefab);
        player.gameObject.transform.position = new Vector3(savePlayerX, savePlayerY);
        SceneManager.LoadScene(saveScene);
    }

    public void SaveGame(bool savePosition)
    {
        if (savePosition)
        {
            var player = GameObject.FindObjectOfType<Player>();
            if (player != null)
            {
                saveScene = SceneManager.GetActiveScene().name;
                savePlayerX = player.transform.position.x;
                savePlayerY = player.transform.position.y;
                saveGrav = grav;
            }
        }

        this.saveFile = new SaveFile()
        {
            death = death,
            time = (int)time,
            difficulty = difficulty,
            scene = saveScene,
            playerX = savePlayerX,
            playerY = savePlayerY,
            playerGrav = saveGrav,
        };

        var saveJson = JsonUtility.ToJson(saveFile);
        if (!Directory.Exists(Application.persistentDataPath + "Data"))
            Directory.CreateDirectory(Application.persistentDataPath + "Data");

        File.WriteAllText(Application.persistentDataPath + $"Data/save{savenum}", saveJson);

    }
    public void KillPlayer()
    {

        声音.Play音效(声音.SoundEffect.sndDeath);
        death++;


    }
}



public class MaskData
{
    public int left;
    public int right;
    public int top;
    public int bottom;

    public int width;
    public int height;

    public bool[] boolData;
}