using UnityEngine;
using UnityEngine.AI;

public class PatrolEnemy : MonoBehaviour
{
    [Header("Patrol Settings")]
    public Transform[] patrolPoints; // 巡邏點
    private int currentPatrolIndex = 0;

    [Header("Chase Settings")]
    public GameObject player;
    public float attackRange = 5f;  // 攻擊範圍
    public float attackCooldown = 2f;     // 攻击冷却时间
    public int damage = 15;
    public LayerMask obstacleLayer;  // 障礙物層級
    //public float rotationSpeed = 5f; // 追擊旋轉速度

    [Header("Movement Settings")]
    public float viewAngle = 45f;     // 視野角度
    public float alertRadius = 10f;  // 警戒半徑
    public float patrolSpeed = 2f;   // 巡邏速度
    public float chaseSpeed = 3.5f;  // 追擊速度

    private NavMeshAgent agent;
    private bool isChasing = false;  // 當前模式狀態
    public HealthBar healthBar;            //生命值
    private float lastAttackTime;

    private Animator animator;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = patrolSpeed;
        currentPatrolIndex = 0;
        PatrolToNextPoint();

        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (isChasing)
        {
            ChaseMode();
            Debug.Log("Chase");
        }
        if (!isChasing)
        {
            PatrolMode();
            Debug.Log("Patrol");
        }
    }

    private void PatrolMode()
    {
        // 判斷是否發現玩家
        Vector3 directionToPlayer = player.transform.position - transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;

        if (distanceToPlayer <= alertRadius && Mathf.Abs(Vector3.Angle(transform.forward, directionToPlayer)) < viewAngle)
        {
            // 使用 Raycast 檢查是否有障礙物遮擋
            if (!Physics.Raycast(transform.position, directionToPlayer.normalized, distanceToPlayer, obstacleLayer))
            {
                // 發現玩家，切換到追擊模式
                StartChasing();
                return;
            }
        }

        // 巡邏點邏輯
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            PatrolToNextPoint();
        }
    }

    private void ChaseMode()
    {
        Vector3 directionToPlayer = player.transform.position - transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;

        // 更新追擊目標
        agent.SetDestination(player.transform.position);

        if(distanceToPlayer <= attackRange)
        {
            AttackPlayer();
        }
        else
        {
            animator.SetBool("IsAttacking", false);
            animator.SetBool("IsChasing", true);
        }
        // 如果玩家離開警戒範圍，切回巡邏模式
        if (distanceToPlayer > alertRadius)
        {
            StopChasing();
        }
    }
    void AttackPlayer()
    {
        if(Time.time - lastAttackTime >= attackCooldown)
        {
            lastAttackTime = Time.time;  //記錄攻擊時間
            healthBar.beenAttacked(damage);   //呼叫生命值腳本(傷害參數)
        }
        animator.SetBool("IsAttacking", true);
        animator.SetBool("IsChasing", false);
    }

    /*private void ChaseMode()
    {
        Vector3 directionToPlayer = (player.transform.position - transform.position).normalized;

        // 面向玩家並移動
        MoveAndRotate(directionToPlayer, chaseSpeed);

       
        // 如果玩家離開警戒半徑，切換回巡邏模式
        if (Vector3.Distance(transform.position, player.transform.position) > alertRadius)
        {
            StopChasing();
        }
    }*/

    /*private void MoveAndRotate(Vector3 direction, float speed)
    {
        // 旋轉邏輯
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        // 移動邏輯
        transform.position += transform.forward * speed * Time.deltaTime;
    }*/

    private void StartChasing()
    {
        isChasing = true;
        agent.speed = chaseSpeed; // 切換到追擊速度

        animator.SetBool("IsChasing", true);
    }

    private void StopChasing()
    {
        isChasing = false;
        agent.speed = patrolSpeed; // 切換到巡邏速度
        PatrolToNextPoint(); // 繼續巡邏

        animator.SetBool("IsChasing", false);
    }

    private void PatrolToNextPoint()
    {
        if (patrolPoints.Length == 0) return;

        // 設置目標為下一個巡邏點
        agent.SetDestination(patrolPoints[currentPatrolIndex].position);
        currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length; // 循環切換巡邏點
    }

    private void OnDrawGizmosSelected()
    {
        // 畫出警戒範圍
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, alertRadius);

        // 畫出視野範圍
        Vector3 leftBoundary = Quaternion.Euler(0, -viewAngle, 0) * transform.forward * alertRadius;
        Vector3 rightBoundary = Quaternion.Euler(0, viewAngle, 0) * transform.forward * alertRadius;

        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, leftBoundary);
        Gizmos.DrawRay(transform.position, rightBoundary);

        // 畫出攻擊範圍
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
