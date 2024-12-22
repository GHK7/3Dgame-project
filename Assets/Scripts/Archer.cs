using UnityEngine;

public class detect : MonoBehaviour
{
    bool detected;
    GameObject player; // 玩家物件
    public Transform enemy; // 敵人自身的 Transform

    public GameObject arrow; // 箭物件
    public Transform shootPoint; // 發射點

    public float shootspeed = 10f; // 箭速
    public float timetoShoot = 1.3f; // 發射間隔
    float originaltime;

    public float detectRange = 10f; // 偵測範圍

    private Animator animator;

    void Start()
    {
        originaltime = timetoShoot;
        animator = GetComponent<Animator>();

        // 自動尋找玩家
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        // 檢查與玩家的距離是否小於偵測範圍
        if (player != null)
        {
            float distance = Vector3.Distance(enemy.position, player.transform.position);

            if (distance <= detectRange)
            {
                detected = true;
                enemy.LookAt(player.transform); // 面向玩家
                animator.SetBool("IsShooting", true);
            }
            else
            {
                detected = false;
                animator.SetBool("IsShooting", false);
            }
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

    private void ShootPlayer()
    {
        GameObject currentArrow = Instantiate(arrow, shootPoint.position, shootPoint.rotation);
        Rigidbody rigidbody = currentArrow.GetComponent<Rigidbody>();

        rigidbody.AddForce(transform.forward * shootspeed, ForceMode.VelocityChange);
    }

    private void OnDrawGizmosSelected()
    {
        // 畫出警戒範圍
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectRange);
    }
}

