using UnityEngine;

public class RandomSpawner : MonoBehaviour
{
    // 場景中已存在的物品
    public GameObject[] originalItems;

    // 地形對象
    public Terrain terrain;

    // 生成範圍 (世界座標範圍)
    public Vector3 areaMin; // 最小範圍 (x, z)
    public Vector3 areaMax; // 最大範圍 (x, z)

    void Start()
    {
        RandomizePositions();
    }

    void RandomizePositions()
    {
        if (terrain == null || originalItems.Length == 0)
        {
            Debug.LogError("請確保已設置地形和原始物品列表！");
            return;
        }

        foreach (GameObject item in originalItems)
        {
            // 隨機生成 X 和 Z 坐標，基於限定範圍
            float randomX = Random.Range(areaMin.x, areaMax.x);
            float randomZ = Random.Range(areaMin.z, areaMax.z);

            // 獲取地形上的高度
            float terrainHeight = terrain.SampleHeight(new Vector3(randomX, 0, randomZ));

            // 設定物品的新位置
            Vector3 newPosition = new Vector3(randomX, terrainHeight, randomZ);

            // 檢查位置是否有效，不在 "Building" 區域內
            if (IsPositionValid(newPosition, 2.0f))
            {
                item.transform.position = newPosition;
            }
            else
            {
                // 如果生成位置無效，則重新隨機生成位置
                RandomizePositions();
            }
        }
    }

    bool IsPositionValid(Vector3 position, float minDistance)
    {
        // 檢查位置是否與原有物品重疊
        foreach (GameObject item in originalItems)
        {
            if (Vector3.Distance(position, item.transform.position) < minDistance)
            {
                return false;
            }
        }

        // 获取所有带有 "Building" 标签的物体
        GameObject[] buildings = GameObject.FindGameObjectsWithTag("Building");

        // 遍历这些物体，检查生成位置是否与它们的区域重叠
        foreach (GameObject building in buildings)
        {
            // 获取建筑物的碰撞器（假设建筑物有碰撞器组件）
            Collider buildingCollider = building.GetComponent<Collider>();
            if (buildingCollider != null)
            {
                // 检查生成的位置是否与建筑物碰撞
                if (buildingCollider.bounds.Contains(position))
                {
                    return false; // 如果位置与建筑物重叠，则无效
                }
            }
        }

        return true;  // 如果位置有效，返回 true
    }
}
