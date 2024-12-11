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

    private Animator animator;

    void Start()
    {
        originaltime = timetoShoot;
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (detected)
        {
            animator.SetBool("IsShooting",true);
            Debug.Log("射箭");
            enemy.LookAt(target.transform);
        }
        else
        {
            animator.SetBool("IsShooting", false);
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
