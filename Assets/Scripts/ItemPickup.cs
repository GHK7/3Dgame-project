using UnityEngine;
using UnityEngine.UI;

public class ItemPickup : MonoBehaviour
{
    public int itemCount = 0; // 拾取數量
    public Text itemCountText; // 直接使用Text類型
    public GameObject ending;
    //public float rotationSpeed = 100f; // 控制旋轉速度
    public GameObject pickupText; // UI 提示對象
    public Light playerLight; // 玩家頭上的光源
    public float lightIntensityIncrement = 0.3f; // 每次拾取物品增加的光強度

    private bool isPlayerInRange = false;

    void Start()
    {
        ending = GameObject.Find("Ending");
        if(ending != null)
        {
            ending.SetActive(false);
        }
        

        if (pickupText != null)
        {
            pickupText.SetActive(false); // 初始化時隱藏撿取提示
        }

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
        GameObject[] numberItems = GameObject.FindGameObjectsWithTag("Items");
        foreach (GameObject Items in numberItems)
        {
            itemCount++; // 增加拾取數量

            if (itemCountText != null)
            {
                itemCountText.text = $"{itemCount}";

            }

            if (playerLight != null)
            {
                playerLight.intensity += lightIntensityIncrement;
            }
            Destroy(gameObject);
        }
        

        

        HidePickupMessage();
        
    }
}

