using UnityEngine;
using UnityEngine.UI;

public class EnergyBar : MonoBehaviour
{
    public GameObject Player;
    public Slider slider; // 滑动条
    public int maxEnergy = 100; // 最大能量值
    public float drainRate = 20f; // 每秒消耗的能量值
    public float recoverRate = 10f; // 每秒恢复的能量值

    private bool isUsingEnergy = false; // 是否正在消耗能量
    private PlayerMovement playerMovement;

    private void Start()
    {
        SetMaxEnergy(maxEnergy);
        playerMovement = Player.GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        // 检测是否按下 Dash 键且能量值大于 0
        if (Input.GetKey(KeyCode.LeftShift) && slider.value > 0)
        {
            isUsingEnergy = true;
            UseEnergy(drainRate * Time.deltaTime);

            // 启动 Dash
            if (!playerMovement.isDashing)
            {
                playerMovement.isDashing = true;
            }
        }
        else
        {
            isUsingEnergy = false;
            RecoverEnergy(recoverRate * Time.deltaTime);

            // 停止 Dash
            if (playerMovement.isDashing)
            {
                playerMovement.isDashing = false;
            }
        }

        // 当能量归零时强制停止 Dash
        if (slider.value <= 0 && playerMovement.isDashing)
        {
            playerMovement.isDashing = false;
        }
    }

    // 设置最大能量值
    public void SetMaxEnergy(int energy)
    {
        slider.maxValue = energy;
        slider.value = energy;
    }

    // 消耗能量
    public void UseEnergy(float amount)
    {
        slider.value = Mathf.Max(0, slider.value - amount); // 确保能量不低于 0
    }

    // 恢复能量
    public void RecoverEnergy(float amount)
    {
        if (!isUsingEnergy)
        {
            slider.value = Mathf.Min(maxEnergy, slider.value + amount); // 确保能量不超过最大值
        }
    }
}
