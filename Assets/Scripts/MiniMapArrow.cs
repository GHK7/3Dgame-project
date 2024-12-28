using UnityEngine;
using System.Collections.Generic;

public class MiniMapArrow : MonoBehaviour
{
    // 小地圖相關
    public RectTransform miniMapRect;  // 小地圖的 RectTransform
    public GameObject arrowPrefab;     // 箭頭的預製體
    public Transform player;           // 玩家物件
    public Transform[] items;          // 場景中的物品列表

    private List<RectTransform> arrows = new List<RectTransform>();   // 箭頭列表
    private List<GameObject> arrowsObjects = new List<GameObject>(); // 用來記錄箭頭物件
    private List<bool> itemsPicked = new List<bool>();  // 用來跟蹤哪些物品已經被撿起
    private float miniMapScale = 1.0f;                    // 小地圖縮放比例
    private int maxArrows = 5;                             // 限制箭頭數量為5

    void Start()
    {
        SetupMiniMapArrows();  // 創建小地圖箭頭
    }

    void Update()
    {
        UpdateMiniMapArrows();  // 更新箭頭位置與方向
    }

    // 初始化小地圖箭頭
    void SetupMiniMapArrows()
    {
        // 清空現有箭頭，並且確保最多只有5個箭頭
        foreach (var arrow in arrows)
        {
            Destroy(arrow.gameObject);
        }
        arrows.Clear();
        arrowsObjects.Clear();
        itemsPicked.Clear();

        // 確保不會超過 5 個箭頭
        int arrowCount = Mathf.Min(items.Length, maxArrows);

        // 為每個物品創建一個箭頭 (最多 5 個)
        for (int i = 0; i < arrowCount; i++)
        {
            GameObject arrow = Instantiate(arrowPrefab, miniMapRect);
            arrow.SetActive(false);  // 開始時隱藏箭頭
            arrowsObjects.Add(arrow);  // 記錄箭頭物件
            arrows.Add(arrow.GetComponent<RectTransform>());
            itemsPicked.Add(false);  // 記錄每個物品是否被撿起
        }
    }

    // 更新小地圖箭頭
    void UpdateMiniMapArrows()
    {
        for (int i = 0; i < arrows.Count; i++)  // 使用箭頭數量進行循環
        {
            // 檢查物品是否有效
            if (items[i] == null)
            {
                // 如果物品被銷毀或設定為 null，則跳過這個物品
                continue;
            }

            // 計算物品與玩家的相對位置
            Vector3 relativePosition = items[i].position - player.position;

            // 檢查物品是否在可撿取範圍內
            if (Vector3.Distance(player.position, items[i].position) < 2.0f && !itemsPicked[i])  // 2.0f 是撿取範圍的閾值
            {
                // 假設物品已撿起，將箭頭隱藏
                arrows[i].gameObject.SetActive(false);
                itemsPicked[i] = true;  // 更新物品已被撿起

                // 隱藏物品，而不銷毀物品
                if (items[i] != null)
                {
                    items[i].gameObject.SetActive(false); // 隱藏物品
                }

                // 或者可以將物品設為 null
                // items[i] = null; // 這樣會保留物品的位置，但不再使用
            }
            else if (!itemsPicked[i])  // 物品尚未被撿起
            {
                // 計算物品方向與玩家的相對方向
                Vector3 direction = relativePosition.normalized;
                float angle = Mathf.Atan2(direction.z, direction.x) * Mathf.Rad2Deg;

                // 更新箭頭方向：這裡的角度基於物品的位置
                arrows[i].rotation = Quaternion.Euler(0, 0, -angle); // 固定箭頭的角度，與玩家視角無關

                // 計算箭頭的位置 (小地圖範圍內)
                Vector2 miniMapSize = miniMapRect.sizeDelta;
                Vector2 targetPosition = new Vector2(relativePosition.x, relativePosition.z);
                Vector2 clampedPosition = Vector2.ClampMagnitude(targetPosition * miniMapScale, miniMapSize.x * 0.5f);

                // 更新箭頭位置
                arrows[i].anchoredPosition = clampedPosition;

                // 顯示箭頭
                arrows[i].gameObject.SetActive(true);
            }
        }
    }
}
