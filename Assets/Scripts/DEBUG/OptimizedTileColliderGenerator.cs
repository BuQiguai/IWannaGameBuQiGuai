using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class OptimizedTileColliderGenerator : MonoBehaviour
{
    [Header("基础设置")]
    public Tilemap targetTilemap;         // 目标瓦片地图
    public GameObject fangzhigameObject; // 目标游戏对象
    public GameObject boxColliderPrefab; // 正方体碰撞体预制件
    public LayerMask collisionLayer;     // 碰撞体层级

    [Header("优化设置")]
    [Range(0.1f, 2f)] public float colliderHeight = 0.5f; // 碰撞体高度
    public bool mergeAdjacentColliders = true;           // 合并相邻碰撞体
    public float mergeThreshold = 0.1f;                 // 合并阈值

    [Header("调试")]
    public bool showDebugGizmos = true;
    public Color gizmoColor = Color.cyan;

    private List<GameObject> generatedColliders = new List<GameObject>();

    #if UNITY_EDITOR
    [ContextMenu("生成优化碰撞体")]
    public void GenerateOptimizedColliders()
    {
        if (targetTilemap == null || boxColliderPrefab == null)
        {
            Debug.LogError("目标瓦片地图或碰撞预制件未设置！");
            return;
        }

        ClearExistingColliders();

        BoundsInt bounds = targetTilemap.cellBounds;
        Dictionary<Vector3Int, bool> tileOccupied = new Dictionary<Vector3Int, bool>();

        // 标记所有有瓦片的位置
        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector3Int tilePos = new Vector3Int(x, y, 0);
                if (targetTilemap.GetTile(tilePos) != null)
                {
                    tileOccupied[tilePos] = true;
                }
            }
        }

        // 生成优化的碰撞体
        List<Vector3Int> positionsToProcess = new List<Vector3Int>(tileOccupied.Keys);

        foreach (var pos in positionsToProcess)
        {
            if (!tileOccupied.ContainsKey(pos) || !tileOccupied[pos]) continue;

            // 寻找可以合并的相邻瓦片
            Vector3Int startPos = pos;
            Vector3Int endPos = pos;

            // 水平方向合并
            if (mergeAdjacentColliders)
            {
                while (tileOccupied.ContainsKey(new Vector3Int(endPos.x + 1, endPos.y, 0)) && 
                    tileOccupied[new Vector3Int(endPos.x + 1, endPos.y, 0)])
                {
                    endPos.x++;
                    tileOccupied[endPos] = false; // 标记为已处理
                }
            }

            // 创建合并后的碰撞体
            CreateStretchedCollider(startPos, endPos);
            tileOccupied[pos] = false; // 标记为已处理
        }

        Debug.Log($"生成完成，共创建 {generatedColliders.Count} 个优化碰撞体");
    }
    #endif

    private void CreateStretchedCollider(Vector3Int startPos, Vector3Int endPos)
    {
        Vector3 startWorldPos = targetTilemap.CellToWorld(startPos);
        Vector3 endWorldPos = targetTilemap.CellToWorld(endPos);
        
        // 计算位置和大小
        Vector3 centerPos = new Vector3(startWorldPos.x , startWorldPos.y + 32, 0);
        float width = endPos.x - startPos.x + 1;
        float height = 1;

        // 实例化并配置碰撞体
        GameObject colliderObj = Instantiate(boxColliderPrefab, centerPos, Quaternion.identity, transform);
        colliderObj.name = $"TileCollider_{startPos.x}_{startPos.y}_to_{endPos.x}_{endPos.y}";
        colliderObj.layer = collisionLayer;
        
        // 调整碰撞体大小
        colliderObj.transform.localScale = new Vector3(width, height, colliderHeight);
        colliderObj.transform.parent = fangzhigameObject.transform;
        generatedColliders.Add(colliderObj);
    }

    #if UNITY_EDITOR
    [ContextMenu("清除所有碰撞体")]
    public void ClearExistingColliders()
    {
            for(int i = fangzhigameObject.transform.childCount - 1;i >= 0; i--)
            {
                var transforma = fangzhigameObject.transform.GetChild(i);
                GameObject.DestroyImmediate(transforma.gameObject);
            }
        generatedColliders.Clear();
        Debug.Log("已清除所有碰撞体");
    }

    void OnDrawGizmos()
    {
        if (!showDebugGizmos || targetTilemap == null) return;

        Gizmos.color = gizmoColor;
        foreach (GameObject collider in generatedColliders)
        {
            if (collider != null)
            {
                Gizmos.DrawWireCube(collider.transform.position + new Vector3(16 *collider.transform.localScale.x, -16 , 0), collider.transform.localScale * 32);
            }
        }
    }
    #endif
}

#if UNITY_EDITOR
[CustomEditor(typeof(OptimizedTileColliderGenerator))]
public class OptimizedTileColliderGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        OptimizedTileColliderGenerator generator = (OptimizedTileColliderGenerator)target;

        GUILayout.Space(15);
        if (GUILayout.Button("生成优化碰撞体", GUILayout.Height(30)))
        {
            generator.GenerateOptimizedColliders();
        }

        if (GUILayout.Button("清除所有碰撞体", GUILayout.Height(30)))
        {
            generator.ClearExistingColliders();
        }

        GUILayout.Space(10);
        EditorGUILayout.HelpBox("优化碰撞体生成说明：\n1. 自动合并相邻瓦片的碰撞体\n2. 减少总碰撞体数量\n3. 保持精确碰撞检测", MessageType.Info);
    }
}
#endif