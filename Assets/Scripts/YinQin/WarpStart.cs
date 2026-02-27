using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WarpStart : MonoBehaviour
{
    public Difficulty difficulty;
    private PixelPerfectCollider collider;

    public enum Difficulty
    {
        Medium = 0,
        Hard = 1,
        VeryHard = 2,
        Impossible = 3,
        LoadGame = 4,
    }

    [Header("过渡动画设置")]
    public float animationDuration = 1.0f;
    private AnimationCurve scaleCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public Sprite transitionSprite; // 过渡动画使用的精灵图
    private Color transitionColor = Color.black; // 过渡动画颜色

    private bool isTransitioning = false;
    private bool isTransitioning2 = true;
    private GameObject transitionObject; // 静态保存过渡对象

    void Awake()
    {
        isTransitioning = false;
        isTransitioning2 = true;
        collider = GetComponent<PixelPerfectCollider>();

    }

    void FixedUpdate()
    {
        if (!isTransitioning && collider.PlaceMeeting(transform.position.x, transform.position.y, "Player"))
        {
            isTransitioning = true;
            var player = FindObjectOfType<Player>();
            if (player != null)
            {
                isTransitioning2 = false;
                this.transform.parent = null;
                StartCoroutine(StartTransitionAnimation(player.gameObject));
            }
        }

        // 如果是刚加载的场景且正在过渡中，执行结束动画
        // Debug.Log(SceneManager.GetActiveScene().name);
        // Debug.Log(isTransitioning2);
        if (!isTransitioning2 && SceneManager.GetActiveScene().name != "DifficultySelect")
        {
            Debug.Log("Start End Transition Animation");
            isTransitioning2 = true;
            StartCoroutine(EndTransitionAnimation());
        }
    }

    IEnumerator StartTransitionAnimation(GameObject player)
    {
        isTransitioning = true;

        // 销毁玩家
        Destroy(player);

        // 创建过渡对象（如果还不存在）
        if (transitionObject == null)
        {
            CreateTransitionObject();
        }


        // 缩放动画（放大）
        float timer = 0f;
        while (timer < animationDuration)
        {
            transitionObject.transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, 0);

            timer += Time.deltaTime;
            float progress = timer / animationDuration;
            float scale = scaleCurve.Evaluate(progress);
            transitionObject.transform.localScale = new Vector3(1000, scale * 1000 / 32, 0);
            yield return null;
        }

        transitionObject.transform.localScale = Vector3.one * 100000;

        // 保存游戏状态
        if (World.instance != null)
        {
            World.instance.gameStarted = true;
            World.instance.autosave = true;
        }
        
        DontDestroyOnLoad(this.gameObject); // 跨场景不销毁
        this.gameObject.transform.position = new Vector3(999999, 999999, 0);

        开始游戏();
        

    }

    IEnumerator EndTransitionAnimation()
    {
        Debug.Log("End Transition Animation");


        yield return null;
        yield return null;
        yield return null;

        
        //移动到摄像机坐标
        transitionObject.transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, 0);

        // 缩放动画（缩小）
        float timer = 0f;
        while (timer < animationDuration)
        {
            transitionObject.transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, 0);

            timer += Time.deltaTime;
            float progress = timer / animationDuration;
            float scale = scaleCurve.Evaluate(1-progress);
            transitionObject.transform.localScale = new Vector3(1000, scale * 1000 / 32, 0);
            yield return null;
        }

        transitionObject.transform.localScale = Vector3.zero;
        
        Destroy(this.gameObject); // 销毁当前对象
    }

    void CreateTransitionObject()
    {
        // 创建一个新游戏对象用于过渡
        transitionObject = new GameObject("SceneTransition");

        DontDestroyOnLoad(transitionObject); // 跨场景不销毁
        
        // 添加SpriteRenderer组件
        SpriteRenderer sr = transitionObject.AddComponent<SpriteRenderer>();
        sr.sprite = transitionSprite;
        sr.color = transitionColor;
        //排序图层
        sr.sortingLayerName = "UI";
        sr.sortingOrder = 9999; // 确保在最上层
        
        // 设置位置和缩放
        transitionObject.transform.position = this.transform.position;
        transitionObject.transform.localScale = Vector3.zero;

    }


    void 开始游戏()
    {
        if (difficulty == Difficulty.LoadGame)
            {
                if (File.Exists(Application.persistentDataPath + $"Data/save{World.instance.savenum}"))
                {
                    // Load exists game
                    World.instance.LoadGame(true);
                }
                else
                {
                    // Restart scene
                    SceneManager.LoadScene(SceneManager.GetActiveScene().name);

                }
            }
            else
            {
                // Start new game
                World.instance.gameStarted = true;
                World.instance.autosave = true;

                World.instance.difficulty = (World.Difficulty)difficulty;

                if (File.Exists(Application.persistentDataPath + $"Data/save{World.instance.savenum}"))
                    File.Delete(Application.persistentDataPath + $"Data/save{World.instance.savenum}");

                SceneManager.LoadScene(World.instance.startScene);
            }
    }
}
