using UnityEngine;

public class detect : MonoBehaviour
{
    bool detected;
    GameObject target;
    public Transform enemy;

    public GameObject arrow;
    public Transform shootPoint;

    public float shootspeed = 10f;
    public float timetoShoot = 1.3f;
    float originaltime;

    void Start()
    {
        originaltime = timetoShoot;
    }

    // Update is called once per frame
    void Update()
    {
        if (detected)
        {
            Debug.Log("®g½b");
            enemy.LookAt(target.transform);
        }
    }
    private void FixedUpdate()
    {
        if (detected)
        {
            timetoShoot -= Time.deltaTime;
            if (timetoShoot < 0)
            {
                ShootPlayer();
                timetoShoot = originaltime;
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            detected = true;
            target = other.gameObject;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            detected = false;
        }
    }
    private void ShootPlayer()
    {
        GameObject currenArrow = Instantiate(arrow, shootPoint.position, shootPoint.rotation);
        Rigidbody rigidbody = currenArrow.GetComponent<Rigidbody>();

        rigidbody.AddForce(transform.forward * shootspeed, ForceMode.VelocityChange);
    }
}
