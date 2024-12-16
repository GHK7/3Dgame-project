using UnityEngine;

public class Arrow : MonoBehaviour
{
    public float lifeTime = 3f; // �l�u���s���ɶ�
    public float maxDistance = 200f; // �l�u���̤j�Z��
    private Vector3 startPosition;

    HealthBar healthBar;

    void Start()
    {
        // �ʺA�j�M HealthBar
        healthBar = FindObjectOfType<HealthBar>();

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
        
        if (other.tag == "Building" || other.tag == "Ground")
        {
            Destroy(gameObject); 
            //Debug.Log("23");
        }
        else if (other.tag == "Player" )
        {
            Destroy(gameObject);
            healthBar.beenAttacked(10);  
        }
    }
}
