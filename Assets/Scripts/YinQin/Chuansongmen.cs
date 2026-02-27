using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(PixelPerfectCollider))]
public class Chuansongmen : MonoBehaviour
{
    [Header("场景设置")]
    public string targetScene;
    
    [Header("过渡动画设置")]
    public float animationDuration = 1.0f;
    private AnimationCurve scaleCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public Sprite transitionSprite; // 过渡动画使用的精灵图
    private Color transitionColor = Color.black; // 过渡动画颜色

    private PixelPerfectCollider collider;
    private bool isTransitioning = false;
    private bool isTransitioning2 = true;
    private GameObject transitionObject; // 静态保存过渡对象

    Camera 场景主摄像机;
    void Awake()
    {

        collider = GetComponent<PixelPerfectCollider>();
        场景主摄像机 = Camera.main;
        // 如果已经有过渡对象存在（跨场景不销毁），则初始化它
        if (transitionObject != null)
        {
            transitionObject.SetActive(false);
        }
    }

    void Start()
    {
        isTransitioning = false;
        isTransitioning2 = true;
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
                StartCoroutine(StartTransitionAnimation(player.gameObject));
            }
        }

        // 如果是刚加载的场景且正在过渡中，执行结束动画
        if (!isTransitioning2 && SceneManager.GetActiveScene().name == targetScene)
        {
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

        // 显示并设置初始状态
        transitionObject.SetActive(true);
        transitionObject.transform.localScale = Vector3.zero;   

        // 缩放动画（放大）
        float timer = 0f;
        while (timer < animationDuration)
        {

            transitionObject.transform.position = new Vector3(场景主摄像机.transform.position.x, 场景主摄像机.transform.position.y, 0); ;
            timer += Time.deltaTime;
            float progress = timer / animationDuration;
            float scale = scaleCurve.Evaluate(progress);
            transitionObject.transform.localScale = new Vector3(1000, scale * 1000 / 32, 0);
            yield return null;
        }

        transitionObject.transform.localScale = Vector3.one * 1000;

        // 保存游戏状态
        if (World.instance != null)
        {
            World.instance.gameStarted = true;
            World.instance.autosave = true;
        }
        DontDestroyOnLoad(this.gameObject); // 跨场景不销毁
        this.gameObject.transform.position = new Vector3(999999, 999999, 0);
        // 加载新场景
        SceneManager.LoadScene(targetScene);
        

    }

    IEnumerator EndTransitionAnimation()
    {
        Debug.Log("结束动画");
        yield return new WaitForSeconds(0.4f);
        //移动到摄像机坐标
        transitionObject.transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, 0);
        transitionObject.SetActive(true);

        // 缩放动画（缩小）
        float timer = 0f;
        while (timer < animationDuration)
        {
            timer += Time.deltaTime;
            float progress = timer / animationDuration;
            float scale = scaleCurve.Evaluate(1-progress);
            transitionObject.transform.localScale = new Vector3(1000, scale * 1000 / 32, 0);

            if (场景主摄像机 == null)
            {
                场景主摄像机 = Camera.main;
            }
            if(场景主摄像机==null)
            {
                yield return null;
                continue;
            }
            transitionObject.transform.position = new Vector3(场景主摄像机.transform.position.x, 场景主摄像机.transform.position.y, 0); ;

            yield return null;
        }

        transitionObject.transform.localScale = Vector3.zero;
        transitionObject.SetActive(false);
        
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
}