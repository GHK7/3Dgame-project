using UnityEngine;

public class HorizontalRotation : MonoBehaviour
{
    public float rotationSpeed = 100f; // �������t��

    void Update()
    {
        // �p����ਤ��
        float rotationAmount = rotationSpeed * Time.deltaTime;

        // ������¶�� Y �b����
        transform.Rotate(0f, rotationAmount, 0f);
    }
}
