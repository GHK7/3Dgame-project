using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    private bool isPlayerInRange = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))  // 確保玩家進入觸發範圍
        {
            isPlayerInRange = true;
            ShowPickupMessage();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))  // 確保玩家離開觸發範圍
        {
            isPlayerInRange = false;
            HidePickupMessage();
        }
    }

    void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            PickupItem();
        }
    }

    void ShowPickupMessage()
    {
        // 顯示撿取提示訊息，例如顯示UI文字
        Debug.Log("Press E to pick up the item");
    }

    void HidePickupMessage()
    {
        // 隱藏撿取提示訊息
        Debug.Log("Out of range");
    }

    void PickupItem()
    {
        // 撿取物品的邏輯，例如增加物品到玩家的背包
        Debug.Log("Item picked up!");

        // 刪除或隱藏物品
        Destroy(gameObject);
    }
}
