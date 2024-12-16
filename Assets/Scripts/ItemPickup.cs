using UnityEngine;
using UnityEngine.UI;

public class ItemPickup : MonoBehaviour
{
    public int itemCount = 0;    // 拾取數量
    public Text itemCountText;   // 直接使用Text類型
    public GameObject ending;
    //public float rotationSpeed = 100f; // 控制旋轉速度
    public GameObject pickupText; // UI 提示對象
    public Light playerLight;     // 玩家頭上的光源
    public float lightIntensityIncrement = 0.3f; // 每次拾取物品增加的光強度

    private bool isPlayerInRange = false;
    private GameObject currentItem; // 儲存當前進入範圍的物品

    void Start()
    {
        ending = GameObject.Find("Ending");
        ending.SetActive(false);
        pickupText.SetActive(false); 
        
        if (playerLight == null)
        {
            // 嘗試在玩家身上自動尋找光源
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                playerLight = player.GetComponentInChildren<Light>();
            }
        }
    }

    void Update()
    {
        /*// 計算旋轉角度
        float rotationAmount = rotationSpeed * Time.deltaTime;
        // 讓物件繞著 Y 軸旋轉
        transform.Rotate(0f, rotationAmount, 0f);*/

        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            PickupItem();
        }

        if(itemCount == 5)
        {
            ending.SetActive(true);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Items"))  // 確保玩家進入觸發範圍
        {
            isPlayerInRange = true;
            currentItem = other.gameObject; // 儲存觸發的物品
            ShowPickupMessage();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Items"))  // 確保玩家離開觸發範圍
        {
            isPlayerInRange = false;
            currentItem = null; // 清空物品參考
            HidePickupMessage();
        }
    }

    void ShowPickupMessage()
    {
        if (pickupText != null)
        {
            pickupText.SetActive(true);
        }
    }

    void HidePickupMessage()
    {
        if (pickupText != null)
        {
            pickupText.SetActive(false);
        }
    }

    void PickupItem()
    {
        if (currentItem != null) // 確保當前物品存在
        {
            itemCount++; // 增加拾取數量
            itemCountText.text = $"{itemCount}";

            if (playerLight != null)
            {
                playerLight.intensity += lightIntensityIncrement;
            }

            Destroy(currentItem); // 銷毀拾取的物品
            HidePickupMessage();
        }
    }
}

