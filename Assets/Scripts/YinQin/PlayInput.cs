using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.IO;


public class PlayInput : MonoBehaviour
{
    public Transform 虚拟按键;
    public Button[] 按键们;
    private Dictionary<string, bool> _当前帧按键状态 = new Dictionary<string, bool>();
    private Dictionary<string, bool> _帧按键状态 = new Dictionary<string, bool>();
    private Dictionary<string, bool> _上一帧按键状态 = new Dictionary<string, bool>();

    private Dictionary<KeyCode, bool> _上一帧所有按键 = new Dictionary<KeyCode, bool>();
    public static PlayInput _instance;
    private WaitForFixedUpdate _waitForFixedUpdate = new WaitForFixedUpdate();


    private Button 当前拖动的按键;
    private Vector2 拖动偏移量;
    private List<按键位置信息> 按键位置列表 = new List<按键位置信息>();
    private List<按键位置信息> 原始按键位置列表 = new List<按键位置信息>();
    private string 配置路径 => Application.persistentDataPath + "/按键位置配置.json";

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
        StartCoroutine(UpdatePhysicsState());
        if (按键们 == null || 按键们.Length == 0)
        {
            Debug.LogWarning("未启用虚拟按键");
            return;
        }


        // 初始化按键状态字典
        foreach (Button 按键 in 按键们)
        {
            string 按键名 = 按键.name;
            _当前帧按键状态[按键名] = false;
            _帧按键状态[按键名] = false;
            _上一帧按键状态[按键名] = false;

            // 添加事件监听
            var trigger = 按键.gameObject.AddComponent<EventTrigger>();

            // 按下事件
            var pointerDown = new EventTrigger.Entry();
            pointerDown.eventID = EventTriggerType.PointerEnter;
            pointerDown.callback.AddListener((data) => { OnButtonDown(按键, 按键名); });
            trigger.triggers.Add(pointerDown);

            var pointerUp = new EventTrigger.Entry();
            pointerUp.eventID = EventTriggerType.PointerUp;
            pointerUp.callback.AddListener((data) => { OnButtonUp(按键, 按键名); });
            trigger.triggers.Add(pointerUp);

            // 离开事件
            var pointerExit = new EventTrigger.Entry();
            pointerExit.eventID = EventTriggerType.PointerExit;
            pointerExit.callback.AddListener((data) => { OnButtonUp(按键, 按键名); });
            trigger.triggers.Add(pointerExit);

            // 添加按键位置数据
            按键位置信息 数据 = new 按键位置信息();
            数据.name = 按键名;
            RectTransform rt = 按键.GetComponent<RectTransform>();
            数据.position = rt.anchoredPosition;
            数据.size = rt.sizeDelta;
            原始按键位置列表.Add(数据);
        }

        // 读取按键位置配置
        if (File.Exists(配置路径))
        {
            Debug.Log("按键位置加载");
            string json数据 = File.ReadAllText(配置路径);
            按键位置列表 = JsonUtility.FromJson<按键位置列表包装>(json数据).positions;

            foreach (var 数据 in 按键位置列表)
            {
                
                Debug.Log("按键位置加载"+ 数据.name);
                Transform 按键变换 = 虚拟按键.Find(数据.name);
                if (按键变换 != null)
                {
                    Debug.Log("按键位置配置已加载" + 数据.name);
                    RectTransform rt = 按键变换.GetComponent<RectTransform>();
                    rt.anchoredPosition = 数据.position;
                    rt.sizeDelta = 数据.size;
                }
                else
                {
                    Debug.Log("按键位置配置加载失败" + 数据.name);
                }
            }

        }

        foreach (Button 按键 in 按键们)
        {
            // 在原有事件监听代码后添加：
            var dragTrigger = 按键.gameObject.AddComponent<EventTrigger>();
            
            var beginDrag = new EventTrigger.Entry();
            beginDrag.eventID = EventTriggerType.BeginDrag;
            beginDrag.callback.AddListener((data) => { 开始拖动按键(按键); });
            dragTrigger.triggers.Add(beginDrag);
        }
    }

    IEnumerator UpdatePhysicsState()
    {
        while (true)
        {


            // 在物理帧之后更新状态
            foreach (var kvp in _帧按键状态)
            {
                _上一帧按键状态[kvp.Key] = kvp.Value;
            }

            // 将当前帧状态同步到物理帧状态
            foreach (var kvp in _当前帧按键状态)
            {
                _帧按键状态[kvp.Key] = kvp.Value;
            }

            // 更新物理按键状态
            foreach (KeyCode key in System.Enum.GetValues(typeof(KeyCode)))
            {
                _上一帧所有按键[key] = Input.GetKey(key);
            }

            // 等待物理帧结束
            yield return _waitForFixedUpdate;
        }
    }

    void Update()
    {
        // 实时更新当前帧状态（不需要同步到物理帧状态）
        if (UI玩家.是否正在调按键)
        {
            if (Input.GetMouseButton(0) && 当前拖动的按键 != null)
            {
                Vector2 鼠标位置;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    当前拖动的按键.transform.parent as RectTransform,
                    Input.mousePosition,
                    Camera.main,
                    out 鼠标位置);

                当前拖动的按键.GetComponent<RectTransform>().anchoredPosition = 鼠标位置 - 拖动偏移量;
            }
            else
            {
                当前拖动的按键 = null;
            }
        }
    }
    public void 开始拖动按键(Button 按键)
    {
        if (!UI玩家.是否正在调按键) return;
        
        当前拖动的按键 = 按键;
        
        Vector2 鼠标位置;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            按键.transform.parent as RectTransform,
            Input.mousePosition,
            Camera.main,
            out 鼠标位置);
        
        拖动偏移量 = 鼠标位置 - 按键.GetComponent<RectTransform>().anchoredPosition;
    }
    private void OnButtonDown(Button 按键, string buttonName)
    {
        if (UI玩家.是否正在调按键)
        {
            return;
        }
        _当前帧按键状态[buttonName] = true;
    }

    private void OnButtonUp(Button 按键, string buttonName)
    {
        if (UI玩家.是否正在调按键)
        {
            return;
        }
        _当前帧按键状态[buttonName] = false;
    }

    // ========== 公共接口 ==========

    public static bool 获取按键状态(KeyCode 按键)
    {
        if (_instance == null) return Input.GetKey(按键);
        string 按键名称 = 按键.ToString();

        return _instance._帧按键状态.TryGetValue(按键名称, out bool 状态)
            ? 状态 || Input.GetKey(按键)
            : Input.GetKey(按键);
    }

    public static bool 获取按键按下状态(KeyCode 按键)
    {
        if (_instance == null) return 物理按键按下(按键);
        string 按键名称 = 按键.ToString();

        if (_instance._帧按键状态.TryGetValue(按键名称, out bool 当前状态) &&
            _instance._上一帧按键状态.TryGetValue(按键名称, out bool 上一帧状态))
        {
            return (当前状态 && !上一帧状态) || 物理按键按下(按键);
        }
        return 物理按键按下(按键);
    }

    public static bool 获取按键抬起状态(KeyCode 按键)
    {
        if (_instance == null) return 物理按键抬起(按键);
        string 按键名称 = 按键.ToString();

        if (_instance._帧按键状态.TryGetValue(按键名称, out bool 当前状态) &&
            _instance._上一帧按键状态.TryGetValue(按键名称, out bool 上一帧状态))
        {
            return (!当前状态 && 上一帧状态) || 物理按键抬起(按键);
        }
        return 物理按键抬起(按键);
    }


    public static bool 物理按键按下(KeyCode 按键)
    {
        return Input.GetKey(按键) && !_instance._上一帧所有按键[按键];
    }

    public static bool 物理按键抬起(KeyCode 按键)
    {
        return !Input.GetKey(按键) && _instance._上一帧所有按键[按键];
    }

    public static void 保存按键位置()
    {
        if (_instance == null) return;
        _instance.按键位置列表.Clear();

        foreach (Button 按键 in _instance.按键们)
        {
            Debug.Log("记录按键名：" + 按键.name);
            RectTransform rt = 按键.GetComponent<RectTransform>();
            _instance.按键位置列表.Add(new 按键位置信息
            {
                name = 按键.name,
                position = rt.anchoredPosition,
                size = rt.sizeDelta
            });
        }
        var 包装 = new 按键位置列表包装();
        包装.positions = _instance.按键位置列表;

        string json数据 = JsonUtility.ToJson(包装, true);
        Debug.Log(json数据);
        File.WriteAllText(_instance.配置路径, json数据);
        Debug.Log("按键位置已保存到：" + _instance.配置路径);
    }
    
    public static void 重置按键位置()
    {
        if (_instance == null) return;
        foreach (按键位置信息 位置 in _instance.原始按键位置列表)
        {
            foreach (Button 按键 in _instance.按键们)
            {
                if (按键.name == 位置.name)
                {
                    RectTransform rt = 按键.GetComponent<RectTransform>();
                    rt.anchoredPosition = 位置.position;
                    rt.sizeDelta = 位置.size;
                }
            }
        }
        
    }

}


[System.Serializable]
public class 按键位置信息
{
    public string name;
    public Vector2 position;
    public Vector2 size;
}

[System.Serializable]
public class 按键位置列表包装
{
    public List<按键位置信息> positions = new List<按键位置信息>();
}
