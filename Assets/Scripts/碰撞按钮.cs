using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Collider2D))]
public class 碰撞按钮 : MonoBehaviour
{
    [System.Serializable]
    public class ButtonEvent : UnityEvent { }

    [Header("按钮设置")]
    [Tooltip("是否可交互")]
    public bool interactable = true;

    [Header("点击事件")]
    [Tooltip("点击时触发的事件")]
    public ButtonEvent onClick = new ButtonEvent();

    [Header("视觉效果")]
    [Tooltip("正常状态颜色")]
    public Color normalColor = Color.white;
    [Tooltip("悬停状态颜色")]
    public Color highlightedColor = new Color(0.95f, 0.95f, 0.95f, 1f);
    [Tooltip("按下状态颜色")]
    public Color pressedColor = new Color(0.8f, 0.8f, 0.8f, 1f);
    [Tooltip("禁用状态颜色")]
    public Color disabledColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);

    private SpriteRenderer spriteRenderer;
    private bool isPointerOver = false;

    private void Awake()
    {
        // 获取或添加必要的组件
        spriteRenderer = GetComponent<SpriteRenderer>();
        Collider2D collider = GetComponent<Collider2D>();
        collider.isTrigger = true;

        // 如果没有EventSystem，自动创建一个
        if (FindObjectOfType<EventSystem>() == null)
        {
            GameObject eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<EventSystem>();
            eventSystem.AddComponent<StandaloneInputModule>();
        }

        UpdateVisualState();
    }

    private void Update()
    {
        // 检测鼠标/触摸是否在碰撞体上
        if (interactable)
        {
            bool newPointerOver = IsPointerOverCollider();
            if (newPointerOver != isPointerOver)
            {
                isPointerOver = newPointerOver;
                UpdateVisualState();
            }

            if (isPointerOver && Input.GetMouseButtonDown(0))
            {
                OnPointerClick();
            }
        }
    }

    private bool IsPointerOverCollider()
    {
        // 鼠标/触摸位置检测
        Vector2 pointerPosition = Vector2.zero;
        
        #if UNITY_EDITOR || UNITY_STANDALONE
        pointerPosition = Input.mousePosition;
        #elif UNITY_IOS || UNITY_ANDROID
        if (Input.touchCount > 0)
            pointerPosition = Input.GetTouch(0).position;
        #endif

        Ray ray = Camera.main.ScreenPointToRay(pointerPosition);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
        
        return hit.collider != null && hit.collider.gameObject == gameObject;
    }

    private void UpdateVisualState()
    {
        if (spriteRenderer == null) return;

        if (!interactable)
        {
            spriteRenderer.color = disabledColor;
        }
        else if (Input.GetMouseButton(0) && isPointerOver)
        {
            spriteRenderer.color = pressedColor;
        }
        else if (isPointerOver)
        {
            spriteRenderer.color = highlightedColor;
        }
        else
        {
            spriteRenderer.color = normalColor;
        }
    }

    public void OnPointerClick()
    {
        if (!interactable) return;
        Debug.Log("Button Clicked");
        onClick.Invoke();
    }

    #if UNITY_EDITOR
    private void Reset()
    {
        // 自动配置默认组件
        if (GetComponent<Collider2D>() == null)
        {
            gameObject.AddComponent<BoxCollider2D>();
        }
        
        if (GetComponent<SpriteRenderer>() == null)
        {
            SpriteRenderer renderer = gameObject.AddComponent<SpriteRenderer>();
            renderer.sprite = UnityEditor.AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
        }
    }

    [UnityEditor.MenuItem("GameObject/2D Object/Collider Button", false, 20)]
    public static void CreateColliderButton()
    {
        GameObject go = new GameObject("ColliderButton");
        go.AddComponent<SpriteRenderer>();
        go.AddComponent<BoxCollider2D>();
        go.AddComponent<碰撞按钮>();
        UnityEditor.Selection.activeGameObject = go;
    }
    #endif
}