
using System.Collections.Generic;
using UnityEngine;

public class 水波着色器 : MonoBehaviour
{

    Material material;
    [Header("水波参数")]
    float 比值;
    List<Vector4> 渲染队列_UV = new List<Vector4>();
    List<float> 时间队列 = new List<float>();
    List<float> 波速队列 = new List<float>();
    public Shader shader;
    public static 水波着色器 instance;
    // Start is called before the first frame update
    void Start()
    {
        material = new Material(shader);
        instance = this;
        // 获取当前屏幕的分辨率  
        int width = Screen.width;
        int height = Screen.height;

        // 计算并存储宽高比值  
        比值 = (float)width / height;
        Debug.Log("分辨率  : " + 比值);
        material.SetFloat("_Zhi", 比值);
    }

    // Update is called once per frame
    void Update()
    {
        // if (Input.GetMouseButtonDown(0)) // 0 代表鼠标左键  
        // {
        //     // 获取鼠标在屏幕上的位置（像素单位）  
        //     Vector3 mousePosition = Input.mousePosition;

        //     // 将屏幕坐标转换为UV坐标（0-1 范围）  
        //     Vector2 uvCoordinates = new Vector2(
        //         mousePosition.x / Screen.width,
        //         mousePosition.y / Screen.height
        //     );
        //     Vector2 p = new Vector2(0.5f + (uvCoordinates.x - 0.5f) * 比值, uvCoordinates.y);

        //     Vector2 lw = new Vector2(p.x * 2 - 1, p.y * 2 - 1);

        //     渲染队列_UV.Add( new Vector4(lw.x , lw.y , 振幅, 波长));
        //     时间队列.Add(0);
        // }


        for (int i = 0; i < 时间队列.Count; i++)
        {
            if (时间队列[i] < 2)
            {
                时间队列[i] += Time.deltaTime * 波速队列[i];
            }
            else
            {
                时间队列.RemoveAt(i);
                渲染队列_UV.RemoveAt(i);
                波速队列.RemoveAt(i);
            }
        }
    }

    public void 冲击波特效(Vector3 世界位置, float 振幅, float 波长, float 波速)
    {

        Vector3 pingmu = Camera.main.WorldToScreenPoint(世界位置);

        // 将屏幕坐标转换为UV坐标（0-1 范围）  
        Vector2 uvCoordinates = new Vector2(
            pingmu.x / Screen.width,
            pingmu.y / Screen.height
        );
        Vector2 p = new Vector2(0.5f + (uvCoordinates.x - 0.5f) * 比值, uvCoordinates.y);

        Vector2 lw = new Vector2(p.x * 2 - 1, p.y * 2 - 1);

        渲染队列_UV.Add(new Vector4(lw.x, lw.y, 振幅, 波长));
        波速队列.Add(波速);
        时间队列.Add(0);

    }

    public static void 尝试调用冲击波特效(Vector3 世界位置, float 振幅, float 波长, float 波速)
    {
        if (instance != null)
        {
            instance.冲击波特效(世界位置, 振幅, 波长, 波速);
        }
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {

        //         // 安卓平台特别处理
// #if UNITY_ANDROID
    
    // 1. 计算合适的降采样比例（至少降采样到设备支持的大小）
    int maxSize = SystemInfo.maxTextureSize;
    int downSampleFactor = Mathf.Max(1, Mathf.CeilToInt((float)source.width / maxSize));
    
    // 2. 创建降采样纹理（使用ARGB32格式兼容性更好）
    int reducedWidth = source.width / downSampleFactor;
    int reducedHeight = source.height / downSampleFactor;
    
    RenderTexture reduced = RenderTexture.GetTemporary(
        reducedWidth, 
        reducedHeight, 
        0, 
        RenderTextureFormat.ARGB32);
    
    // 3. 高质量降采样（使用双线性滤波）
    Graphics.Blit(source, reduced);
    
    // 4. 创建处理用的临时纹理
    RenderTexture tempTex1 = RenderTexture.GetTemporary(
        reducedWidth, 
        reducedHeight, 
        0, 
        RenderTextureFormat.ARGB32);
    
    RenderTexture tempTex2 = RenderTexture.GetTemporary(
        reducedWidth, 
        reducedHeight, 
        0, 
        RenderTextureFormat.ARGB32);
    
    // 5. 执行迭代处理（与原逻辑一致但使用降采样后的尺寸）
    Graphics.Blit(reduced, tempTex1);
    
    for (int i = 0; i < 渲染队列_UV.Count; i++)
    {
        material.SetVector("_zhong", 渲染队列_UV[i]);
        material.SetFloat("_time1", 时间队列[i]);
        
        // 交替使用两个临时纹理
        if (i % 2 == 0)
        {
            Graphics.Blit(tempTex1, tempTex2, material);
            Graphics.Blit(tempTex2, tempTex1);
        }
        else
        {
            Graphics.Blit(tempTex2, tempTex1, material);
            Graphics.Blit(tempTex1, tempTex2);
        }
    }
    
    // 6. 最终升采样回目标分辨率（使用点过滤保持锐利）
    RenderTexture finalResult = RenderTexture.GetTemporary(
        source.width, 
        source.height, 
        0, 
        source.format);
    
    // 创建点过滤材质（临时）
    Material pointFilterMat = new Material(shader);
    Graphics.Blit((渲染队列_UV.Count % 2 == 0) ? tempTex1 : tempTex2, finalResult, pointFilterMat);
    Destroy(pointFilterMat);
    
    // 7. 输出到目标并释放资源
    Graphics.Blit(finalResult, destination);
    
    RenderTexture.ReleaseTemporary(reduced);
    RenderTexture.ReleaseTemporary(tempTex1);
    RenderTexture.ReleaseTemporary(tempTex2);
    RenderTexture.ReleaseTemporary(finalResult);

// #else

//             RenderTexture 源图像克隆 = RenderTexture.GetTemporary(source.width, source.height, source.depth, source.format);

//             Graphics.Blit(source, 源图像克隆);
//             RenderTexture 临时纹理 = RenderTexture.GetTemporary(source.width, source.height, source.depth, source.format);

//             // 开始迭代渲染队列  
//             for (int i = 0; i < 渲染队列_UV.Count; i++)
//             {
//                 // 设置材质参数  
//                 material.SetVector("_zhong", 渲染队列_UV[i]);
//                 material.SetFloat("_time1", 时间队列[i]);

//                 Graphics.Blit(源图像克隆, 临时纹理, material);   
//                 Graphics.Blit(临时纹理, 源图像克隆);  

//             }
//             Graphics.Blit(临时纹理, destination); 

//             RenderTexture.ReleaseTemporary(源图像克隆);
//             RenderTexture.ReleaseTemporary(临时纹理); 

// #endif
    }
}

