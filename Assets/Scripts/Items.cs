using UnityEngine;

public class HorizontalRotation : MonoBehaviour
{
    public float rotationSpeed = 100f; // 控制旋轉速度

    void Update()
    {
        // 計算旋轉角度
        float rotationAmount = rotationSpeed * Time.deltaTime;

        // 讓物件繞著 Y 軸旋轉
        transform.Rotate(0f, rotationAmount, 0f);
    }
}
