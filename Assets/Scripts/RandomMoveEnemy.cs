using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class RandomMoveEnemy : MonoBehaviour
{
    [Header("Patrol Settings")]
    public float range; // 巡邏範圍半徑
    public Transform centrePoint; // 巡邏中心點

    [Header("Alert Settings")]
    public float alertRadius = 10f;  // 警戒半徑
    public float viewAngle = 45f;    // 視野角度
    public float rotationSpeed = 2f; // 面向玩家的轉向速度
    public float stopTime = 2f;      // 停止時間
    public LayerMask obstacleLayer;  // 障礙物層級

    [Header("Chase Settings")]
    public GameObject player;
    public float chaseSpeed = 50f;  // 追擊速度
    public float attackRange = 5f; // 攻擊範圍
    public float attackCooldown = 2f; // 攻擊冷卻時間
    public int damage = 15;

    [Header("Movement Settings")]
    public float patrolSpeed = 15f; // 巡邏速度

    private NavMeshAgent agent;
    private bool isChasing = false;
    private bool isAlert = false;
    private float lastAttackTime;
    private Animator animator;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = patrolSpeed;
        PatrolMode();
    }

    private void Update()
    {
        if (isChasing)
        {
            ChaseMode();
        }
        else if (isAlert)
        {
            AlertMode();
        }
        else
        {
            PatrolMode();
        }
    }

    private void PatrolMode()
    {
        // 判斷是否玩家進入警戒範圍
        if (PlayerInAlertRadius())
        {
            StartAlertMode();
        }

        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            Vector3 point;
            if (RandomPoint(centrePoint.position, range, out point))
            {
                Debug.DrawRay(point, Vector3.up, Color.blue, 1.0f);
                agent.SetDestination(point);
            }
        }
    
    }

    private void AlertMode()
    {
        agent.isStopped = true; // 停止移動

        Vector3 directionToPlayer = player.transform.position - transform.position;
        StartCoroutine(SmoothLookAt(directionToPlayer));

        if (PlayerInSight())
        {
            StartChaseMode();
        }
        else if (!PlayerInAlertRadius())
        {
            EndAlertMode();
        }
    }

    private void ChaseMode()
    {
        agent.isStopped = false;
        agent.SetDestination(player.transform.position);

        // 確保角色面朝向玩家
        Vector3 directionToPlayer = player.transform.position - transform.position;
        if (directionToPlayer != Vector3.zero) // 防止向量為零導致錯誤
        {
            Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        // 判斷是否進入攻擊範圍
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
        if (distanceToPlayer <= attackRange)
        {
            AttackPlayer();
        }

        // 如果玩家離開警戒範圍，返回巡邏模式
        if (!PlayerInAlertRadius())
        {
            EndChaseMode();
        }
    }

    private void StartAlertMode()
    {
        isAlert = true;
        agent.isStopped = true;
        Debug.Log("進入 AlertMode");
        Invoke(nameof(EndAlertMode), stopTime); // 停止後返回巡邏模式
    }

    private void EndAlertMode()
    {
        isAlert = false;
        agent.isStopped = false;
        Debug.Log("結束 AlertMode");
    }

    private void StartChaseMode()
    {
        isChasing = true;
        agent.speed = chaseSpeed;
        Debug.Log("進入 ChaseMode");
    }

    private void EndChaseMode()
    {
        isChasing = false;
        agent.speed = patrolSpeed;
        Debug.Log("結束 ChaseMode");
    }

    private bool PlayerInAlertRadius()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
        return distanceToPlayer <= alertRadius;
    }

    private bool PlayerInSight()
    {
        Vector3 directionToPlayer = player.transform.position - transform.position;
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);
        if (angleToPlayer < viewAngle / 2)
        {
            if (!Physics.Raycast(transform.position, directionToPlayer.normalized, directionToPlayer.magnitude, obstacleLayer))
            {
                return true; // 玩家在視野內且無障礙物
            }
        }
        return false;
    }

    private void AttackPlayer()
    {
        if (Time.time - lastAttackTime >= attackCooldown)
        {
            lastAttackTime = Time.time;
            Debug.Log("攻擊玩家！");
            // 這裡可以添加對玩家的傷害處理，例如呼叫生命值腳本
        }
    }

    private IEnumerator SmoothLookAt(Vector3 directionToPlayer)
    {
        Vector3 lookDirection = directionToPlayer.normalized;
        Quaternion targetRotation = Quaternion.LookRotation(lookDirection);

        while (Quaternion.Angle(transform.rotation, targetRotation) > 0.1f)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            yield return null;
        }
    }

    private bool RandomPoint(Vector3 center, float range, out Vector3 result)
    {
        Vector3 randomPoint = center + Random.insideUnitSphere * range;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas))
        {
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
        Vector3 leftBoundary = Quaternion.Euler(0, -viewAngle / 2, 0) * transform.forward * alertRadius;
        Vector3 rightBoundary = Quaternion.Euler(0, viewAngle / 2, 0) * transform.forward * alertRadius;

        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, leftBoundary);
        Gizmos.DrawRay(transform.position, rightBoundary);

        // 畫出攻擊範圍
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}

