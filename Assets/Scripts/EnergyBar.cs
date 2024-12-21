using UnityEngine;
using UnityEngine.UI;

public class EnergyBar : MonoBehaviour
{
    public Slider slider; // �s��Slider
    public int maxEnergy = 100; // �̤j��O��
    public float drainRate = 20f; // �C���֪���O��
    public float recoverRate = 10f; // �C���_����O��

    private bool isUsingEnergy = false; // �O�_���b������O

    PlayerMovement playerMovement;

    private void Start()
    {
        SetMaxEnergy(maxEnergy);
    }

    private void Update()
    {
        // ����F�������O
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

    // �]�w�̤j��O��
    public void SetMaxEnergy(int energy)
    {
        slider.maxValue = energy;
        slider.value = energy;
    }

    // ������O
    public void UseEnergy(float amount)
    {
        slider.value = Mathf.Max(0, slider.value - amount); // �T�O��O���C��0
    }

    // ��_��O
    public void RecoverEnergy(float amount)
    {
        if (!isUsingEnergy)
        {
            slider.value = Mathf.Min(maxEnergy, slider.value + amount); // �T�O��O���W�L�̤j��
        }
    }
}
