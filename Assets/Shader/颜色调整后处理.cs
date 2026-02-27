using UnityEngine;

[RequireComponent(typeof(Camera))]
[ExecuteInEditMode] // 允许在编辑模式下预览效果
public class 颜色调整后处理 : MonoBehaviour
{
    [Header("Shader设置")]
    [SerializeField] private Shader _shader; // 通过Inspector赋值
    [SerializeField, Range(1f, 3f)] private float _redMultiplier = 1.6f;
    
    private Material _material;
    public bool _isSupported = true;

    void OnEnable()
    {
        // 检查平台支持
        _isSupported = CheckSupport();
        if (!_isSupported)
        {
            Debug.LogWarning("当前设备不支持后处理效果");
            enabled = false;
            return;
        }
    }

    bool CheckSupport()
    {

        // Shader检查
        if (!_shader || !_shader.isSupported)
        {
            Debug.LogError("Shader不可用或不支持当前平台");
            return false;
        }

        return true;
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {

        // 确保材质存在
        if (_material == null)
        {
            _material = CreateMaterial();
        }
        if (!_isSupported || _material == null)
        {
            Graphics.Blit(source, destination);
            return;
        }

        // 设置参数
        _material.SetFloat("_RedMultiplier", _redMultiplier);

        // 安卓平台特别处理
        #if UNITY_ANDROID

    // 1. 获取设备支持的最大尺寸
    int maxSize = SystemInfo.maxTextureSize;
    int targetWidth = Mathf.Min(source.width, maxSize);
    int targetHeight = Mathf.Min(source.height, maxSize);

    // 2. 创建降采样RT（使用R8G8B8A8格式更兼容移动端）
    RenderTexture downSampled = RenderTexture.GetTemporary(
        targetWidth, 
        targetHeight, 
        0, 
        RenderTextureFormat.ARGB32
    );

    // 3. 高质量降采样（使用双线性或自定义Shader）
    Graphics.Blit(source, downSampled, _material);

    // 4. 执行后处理
    RenderTexture processed = RenderTexture.GetTemporary(targetWidth, targetHeight, 0);
    Graphics.Blit(downSampled, processed, _material);

    // 5. 升采样回屏幕分辨率（可选锐化）
    Graphics.Blit(processed, destination, _material);

    // 6. 释放临时RT
    RenderTexture.ReleaseTemporary(downSampled);
    RenderTexture.ReleaseTemporary(processed);

        #else
        // 其他平台直接处理
        Graphics.Blit(source, destination, _material);
        #endif
    }

    Material CreateMaterial()
    {
        var mat = new Material(_shader)
        {
            hideFlags = HideFlags.DontSave
        };
        return mat;
    }

    void OnDisable()
    {
        // 清理材质
        if (_material != null)
        {
            DestroyImmediate(_material);
        }
    }
}