using UnityEngine;
using System.Collections.Generic;

public class OnScreenDebugger : MonoBehaviour
{
    private static OnScreenDebugger _instance;
    public static OnScreenDebugger Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<OnScreenDebugger>();
                if (_instance == null)
                {
                    GameObject obj = new GameObject("OnScreenDebugger");
                    _instance = obj.AddComponent<OnScreenDebugger>();
                    DontDestroyOnLoad(obj);
                }
            }
            return _instance;
        }
    }

    public float ziti_daxiao = 0.01f;

    // 调试日志相关
    private List<string> logMessages = new List<string>();
    private Vector2 logScrollPosition = Vector2.zero;
    private const int MAX_LOG_COUNT = 100;
    private bool showDebugLog = false;

    // FPS相关
    private float updateInterval = 0.5f;
    private float accum = 0.0f;
    private int frames = 0;
    private float timeleft;
    private float fps = 0.0f;

    // GUI样式
    private GUIStyle debugTextStyle;
    private GUIStyle buttonStyle;

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }

        timeleft = updateInterval;

        // 初始化GUI样式
        debugTextStyle = new GUIStyle();
        debugTextStyle.normal.textColor = Color.red;
        debugTextStyle.fontSize = Mathf.RoundToInt(Screen.width * ziti_daxiao);

        // 监听Unity的Debug.Log输出
        Application.logMessageReceived += HandleLog;
    }

    void OnDestroy()
    {
        // 移除监听，防止内存泄漏
        Application.logMessageReceived -= HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        // 只显示普通日志、警告和错误
        if (type == LogType.Log || type == LogType.Warning || type == LogType.Error)
        {
            logMessages.Add(logString);

            // 限制日志数量
            if (logMessages.Count > MAX_LOG_COUNT)
            {
                logMessages.RemoveAt(0);
            }

            // 自动滚动到底部
            logScrollPosition.y = logMessages.Count * 20;
        }
    }

    void Update()
    {
        // 计算FPS
        timeleft -= Time.deltaTime;
        accum += Time.timeScale / Time.deltaTime;
        frames++;

        if (timeleft <= 0.0f)
        {
            fps = accum / frames;
            timeleft = updateInterval;
            accum = 0.0f;
            frames = 0;
        }
    }

    void OnGUI()
    {
        
        buttonStyle = new GUIStyle(GUI.skin.button);
        buttonStyle.fontSize = Mathf.RoundToInt(Screen.width * ziti_daxiao);
        if (debugTextStyle == null)
        {
            debugTextStyle = new GUIStyle();
            debugTextStyle.normal.textColor = Color.red;
            debugTextStyle.fontSize = Mathf.RoundToInt(Screen.width * ziti_daxiao);
        }
        // 调试日志区域在右上角
        float logWidth = Screen.width * 0.6f;
        float logHeight = Screen.height * 0.3f;
        float logX = Screen.width - logWidth - 10;
        float logY = 10;
        debugTextStyle.fontSize = 20;
        // 显示FPS在左上角
        GUI.Label(new Rect(10, 10, 200, 30), string.Format("FPS: {0:F2}", fps), debugTextStyle);
        debugTextStyle.fontSize = Mathf.RoundToInt(Screen.width * ziti_daxiao);

        if (GUI.Button(new Rect(Screen.width - logWidth - 200, logY + 10, 180, 50), "显示/隐藏", buttonStyle))
        {
            qiehuan();
        }
        if (GUI.Button(new Rect( 300, logY + 10, 180, 50), "无敌", buttonStyle))
        {
            wudi();
        }
        if (GUI.Button(new Rect( 300, logY + 70, 180, 50), "存档", buttonStyle))
        {
            cundang();
        }
        if (!showDebugLog)
        {
            //只显示最新信息
            if (logMessages.Count > 0)
            {
                GUI.Label(new Rect(logX, logY + 10, logWidth, 30), logMessages[logMessages.Count - 1], debugTextStyle);
            }
            return;
        }



        // 日志背景
        GUI.Box(new Rect(logX, logY, logWidth, logHeight), "");

        // 日志标题和清除按钮
        GUI.Label(new Rect(logX + 10, logY + 10, logWidth - 60, 30), "Debug Log", debugTextStyle);
        if (GUI.Button(new Rect(logX + logWidth - 130, logY + 10, 120, 30), "Clear", buttonStyle))
        {
            logMessages.Clear();
        }


        // 日志内容区域
        float contentY = logY + 50;
        float contentHeight = logHeight - 60;

        // 滚动视图
        logScrollPosition = GUI.BeginScrollView(
            new Rect(logX, contentY, logWidth, contentHeight),
            logScrollPosition,
            new Rect(0, 0, logWidth - 20, logMessages.Count * 20 + 10)
        );

        for (int i = 0; i < logMessages.Count; i++)
        {
            GUI.Label(new Rect(10, i * 20 * ziti_daxiao / 0.01f, logWidth - 30, 20), logMessages[i], debugTextStyle);
        }

        GUI.EndScrollView();
    }
    void qiehuan()
    {
        showDebugLog = !showDebugLog;
    }

    void wudi()
    {

        Player.invincible = !Player.invincible;
        Debug.Log("玩家无敌:"+Player.invincible);

    }

    void cundang()
    {
        World.instance.SaveGame(true);
        Debug.Log("游戏已存档!");
    }
    // 用于切换日志显示
    public static void ToggleDebugLog()
    {
        if (Instance == null) return;
        Instance.showDebugLog = !Instance.showDebugLog;
    }
}