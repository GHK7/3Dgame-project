using UnityEngine;
using UnityEngine.UI;

public class ItemPickup : MonoBehaviour
{
    public int itemCount = 0; // 拾取數量
    public Text itemCountText; // 直接使用Text類型
    public GameObject ending;
    public GameObject pickupText; // UI 提示對象
    public GameObject image , text, gate;
    public GameObject patrol1, patrol2;
    public Light playerLight; // 玩家頭上的光源
    public float lightIntensityIncrement = 0.3f; // 每次拾取物品增加的光強度

    private bool isPlayerInRange = false;
    private GameObject currentItem; // 儲存當前進入範圍的物品

    void Start()
    {
        
        ending.SetActive(false);             
        pickupText.SetActive(false); // 初始化時隱藏撿取提示
        image.SetActive(true);
        text.SetActive(true);
        gate.SetActive(true);
        patrol1.SetActive(false);
        patrol2.SetActive(false);


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
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            PickupItem();
        }
        if(itemCount == 1)
        {
            patrol1.SetActive(true);
        }
        if (itemCount == 3)
        {
            patrol2.SetActive(true);
        }
        if (itemCount == 5)
        {
            ending.SetActive(true);
            image.SetActive(false);
            text.SetActive(false);
            gate.SetActive(false);
            itemCountText.text = "逃離村莊";
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Items")) // 確保玩家進入觸發範圍
        {
            isPlayerInRange = true;
            currentItem = other.gameObject; // 儲存觸發的物品
            ShowPickupMessage();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Items")) // 確保玩家離開觸發範圍
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

            if (itemCountText != null)
            {
                itemCountText.text = $"{itemCount}";
            }

            if (playerLight != null)
            {
                playerLight.intensity += lightIntensityIncrement;
            }

            Destroy(currentItem); // 銷毀拾取的物品
            HidePickupMessage();
        }
    }
}


