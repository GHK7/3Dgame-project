using UnityEngine;

public class Arrow : MonoBehaviour
{
    public float lifeTime = 5f; // 子彈的存活時間
    public float maxDistance = 50f; // 子彈的最大距離
    private Vector3 startPosition;

    public HealthBar healthBar;

    void Start()
    {
        startPosition = transform.position;
        Destroy(gameObject, lifeTime); // 在存活時間後自動銷毀
    }

    void Update()
    {
        // 如果超出最大距離，銷毀子彈
        if (Vector3.Distance(startPosition, transform.position) > maxDistance)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // 如果進入的對象標籤是 "Player"
        if (other.tag == "Player")
        {
            Destroy(gameObject); // 銷毀子彈
            healthBar.beenAttacked(10);   //呼叫生命值腳本(傷害參數)
        }
        else if (other.tag == "Building")
        {
            Destroy(gameObject);
        }
    }
}
