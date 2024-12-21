using UnityEngine;
using UnityEngine.UI;

public class EnergyBar : MonoBehaviour
{
    public Slider slider; // 連接Slider
    public int maxEnergy = 100; // 最大體力值
    public float drainRate = 20f; // 每秒減少的體力值
    public float recoverRate = 10f; // 每秒恢復的體力值

    private bool isUsingEnergy = false; // 是否正在消耗體力

    PlayerMovement playerMovement;

    private void Start()
    {
        SetMaxEnergy(maxEnergy);
    }

    private void Update()
    {
        // 按住F鍵消耗體力
        if (Input.GetKey(KeyCode.F))
        {
            isUsingEnergy = true;
            UseEnergy(drainRate * Time.deltaTime);

            playerMovement.Dash();
        }
        else
        {
            isUsingEnergy = false;
            RecoverEnergy(recoverRate * Time.deltaTime);
        }
    }

    // 設定最大體力值
    public void SetMaxEnergy(int energy)
    {
        slider.maxValue = energy;
        slider.value = energy;
    }

    // 消耗體力
    public void UseEnergy(float amount)
    {
        slider.value = Mathf.Max(0, slider.value - amount); // 確保體力不低於0
    }

    // 恢復體力
    public void RecoverEnergy(float amount)
    {
        if (!isUsingEnergy)
        {
            slider.value = Mathf.Min(maxEnergy, slider.value + amount); // 確保體力不超過最大值
        }
    }
}
