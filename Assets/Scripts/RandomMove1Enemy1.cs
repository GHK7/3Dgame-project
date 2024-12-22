using UnityEngine;
using UnityEngine.AI; //important

//if you use this code you are contractually obligated to like the YT video
public class RandomMoveEnemy1 : MonoBehaviour //don't forget to change the script name if you haven't
{
    [Header("Patrol Settings")]
    public float range; //radius of sphere
    public Transform centrePoint; //centre of the area the agent wants to move around in
    public float stopTime = 2f;
    public float rotationSpeed = 2;
    //instead of centrePoint you can set it as the transform of the agent if you don't care about a specific area

    [Header("Chase Settings")]
    public GameObject player;
    public float attackRange = 5f;  // 攻擊範圍
    public float attackCooldown = 2f;     // 攻击冷却时间
    public int damage = 15;
    public LayerMask obstacleLayer;  // 障礙物層級

    [Header("Movement Settings")]
    public float viewAngle = 45f;     // 視野角度
    public float alertRadius = 10f;  // 警戒半徑
    public float patrolSpeed = 15f;   // 巡邏速度
    public float chaseSpeed = 50f;  // 追擊速度
    

    private NavMeshAgent agent;
    private bool isChasing = false;  // 當前模式狀態
    private bool isAlert = false;
    public HealthBar healthBar;            //生命值
    private float lastAttackTime;

    private Animator animator;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = patrolSpeed;

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
            //Debug.Log("Patrol");
        }
    }
    private void PatrolMode()
    {
        // 判斷是否發現玩家
        Vector3 directionToPlayer = player.transform.position - transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;

        if (distanceToPlayer <= alertRadius)
        {
            AlertMode();

            if (Mathf.Abs(Vector3.Angle(transform.forward, directionToPlayer)) < viewAngle &&
                !Physics.Raycast(transform.position, directionToPlayer.normalized, distanceToPlayer, obstacleLayer))
            {
                // 使用 Raycast 檢查是否有障礙物遮擋
                
                // 發現玩家，切換到追擊模式
                StartChasing();
                return;
                
            }                      
        }
        else if (distanceToPlayer > alertRadius)
        {
            isAlert = false;
        }

        if (agent.remainingDistance <= agent.stoppingDistance) //done with path
        {
            RandomMove();
        }
    }
    public void RandomMove()
    {
        Vector3 point;
        if (RandomPoint(centrePoint.position, range, out point)) //pass in our centre point and radius of area
        {
            Debug.DrawRay(point, Vector3.up, Color.blue, 1.0f); //so you can see with gizmos
            agent.SetDestination(point);
        }
    }
    private void AlertMode()
    {
        if(!isAlert)
        {
            isAlert = true;
            // 停止移動一段時間
            agent.isStopped = true;
            Vector3 directionToPlayer = player.transform.position - transform.position;

            // 等速轉向面向玩家
            Vector3 lookDirection = directionToPlayer.normalized;
            Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            Invoke(nameof(ResumePatrol), stopTime); // 停止後自動恢復巡邏
            Debug.Log("3333");
        }
        
    }
    private void ResumePatrol()
    {
        agent.isStopped = false;
        RandomMove();
        Debug.Log("4444");
    }
        private void ChaseMode()
    {
        Vector3 directionToPlayer = player.transform.position - transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;

        // 更新追擊目標
        agent.SetDestination(player.transform.position);

        if (distanceToPlayer <= attackRange)
        {
            AttackPlayer();
        }

        // 如果玩家離開警戒範圍，切回巡邏模式
        if (distanceToPlayer > alertRadius)
        {
            StopChasing();
        }
    }
    void AttackPlayer()
    {
        if (Time.time - lastAttackTime >= attackCooldown)
        {
            lastAttackTime = Time.time;  //記錄攻擊時間
            healthBar.beenAttacked(damage);   //呼叫生命值腳本(傷害參數)
        }

    }
    private void StartChasing()
    {
        isChasing = true;
        agent.speed = chaseSpeed; // 切換到追擊速度

    }

    private void StopChasing()
    {
        isChasing = false;
        agent.speed = patrolSpeed; // 切換到巡邏速度


    }
    bool RandomPoint(Vector3 center, float range, out Vector3 result)
    {

        Vector3 randomPoint = center + Random.insideUnitSphere * range; //random point in a sphere 
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas)) //documentation: https://docs.unity3d.com/ScriptReference/AI.NavMesh.SamplePosition.html
        {
            //the 1.0f is the max distance from the random point to a point on the navmesh, might want to increase if range is big
            //or add a for loop like in the documentation
            result = hit.position;
            return true;
        }

        result = Vector3.zero;
        return false;
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
