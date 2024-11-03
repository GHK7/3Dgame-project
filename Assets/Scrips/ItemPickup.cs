using UnityEngine;
using UnityEngine.UI;

public class ItemPickup : MonoBehaviour
{
    private bool isPlayerInRange = false;
    public GameObject pickupText; // UI 提示對象
    public Light playerLight; // 玩家頭上的光源
    public float lightIntensityIncrement = 0.5f; // 每次撿取物品增加的光強度

    void Start()
    {
        if (pickupText != null)
        {
            pickupText.SetActive(false); // 初始化時隱藏撿取提示
        }

        if (playerLight == null)
        {
            // 嘗試在玩家身上自動尋找光源，如果沒有，則需要手動設置
            playerLight = GameObject.FindWithTag("Player").GetComponentInChildren<Light>();
        }
    }

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
        // 顯示撿取提示訊息
        if (pickupText != null)
        {
            pickupText.SetActive(true);
        }
    }

    void HidePickupMessage()
    {
        // 隱藏撿取提示訊息
        if (pickupText != null)
        {
            pickupText.SetActive(false);
        }
    }

    void PickupItem()
    {
        // 撿取物品的邏輯，例如增加物品到玩家的背包
        //Debug.Log("Item picked up!");

        // 增加玩家頭上的光強度
        if (playerLight != null)
        {
            playerLight.intensity += lightIntensityIncrement;
        }

        // 隱藏提示並刪除物品
        HidePickupMessage();
        Destroy(gameObject);
    }
}
