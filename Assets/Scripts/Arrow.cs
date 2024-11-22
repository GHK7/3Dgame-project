using UnityEngine;

public class Arrow : MonoBehaviour
{
    public float lifeTime = 5f; // �l�u���s���ɶ�
    public float maxDistance = 50f; // �l�u���̤j�Z��
    private Vector3 startPosition;

    public HealthBar healthBar;

    void Start()
    {
        startPosition = transform.position;
        Destroy(gameObject, lifeTime); // �b�s���ɶ���۰ʾP��
    }

    void Update()
    {
        // �p�G�W�X�̤j�Z���A�P���l�u
        if (Vector3.Distance(startPosition, transform.position) > maxDistance)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // �p�G�i�J����H���ҬO "Player"
        if (other.tag == "Player")
        {
            Destroy(gameObject); // �P���l�u
            healthBar.beenAttacked(10);   //�I�s�ͩR�ȸ}��(�ˮ`�Ѽ�)
        }
        else if (other.tag == "Building")
        {
            Destroy(gameObject);
        }
    }
}
