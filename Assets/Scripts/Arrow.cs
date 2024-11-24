using UnityEngine;

public class Arrow : MonoBehaviour
{
    public float lifeTime = 3f; // 紆丁
    public float maxDistance = 200f; // 紆程禯瞒
    private Vector3 startPosition;

    HealthBar healthBar;

    void Start()
    {
        // 笆篈穓碝 HealthBar
        healthBar = FindObjectOfType<HealthBar>();

        startPosition = transform.position;
        Destroy(gameObject, lifeTime); // 丁笆綪反
    }

    void Update()
    {
        // 狦禬程禯瞒綪反紆
        if (Vector3.Distance(startPosition, transform.position) > maxDistance)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        
        if (other.tag == "Building" || other.tag == "Ground")
        {
            Destroy(gameObject); // 綪反紆
            Debug.Log("23");
        }
        else if (other.tag == "Player" )
        {
            Destroy(gameObject);
            healthBar.beenAttacked(10);   //㊣ネ㏑竲セ(端甡把计)
        }
    }
}
