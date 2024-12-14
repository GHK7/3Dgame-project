using UnityEngine;
using UnityEngine.AI;

public class Villagers : MonoBehaviour
{
    public Transform player; // 玩家
    public Transform retreatPoint; // 撤退點
    public Transform returnPoint; // 返回點 (返回巡邏時的目標點)
    public float alertRadius = 200f; // 警戒半徑
    public float fieldOfView = 45f; // 視野範圍
    public float runSpeed = 50f;
    public float returnSpeed = 15f;

    private NavMeshAgent agent;
    private bool isRetreating = false;
    private bool isReturningToPatrol = false;

    [Header("設定圓形運動參數")]
    public Vector3 center = Vector3.zero; // 圓心位置
    public float radius = 5f; // 圓的半徑
    public float speed = 2f; // 移動速度 (角速度，弧度每秒)

    private float angle; // 當前的角度
    private float startAngle; // 巡邏的起始角度
    public int npcIndex; // NPC 的唯一編號 (手動設置或程序生成)
    public int totalNPCs = 18; // 場景中 NPC 的總數

    [Header("巡邏起始座標（自動計算）")]
    public Vector3 patrolStartPosition; // NPC 巡邏起始位置 (自動計算)

    private float pauseTimer = 0f; // 記錄暫停時間
    private bool isPaused = false; // 是否正在暫停

    void Start()
    {
        agent = GetComponent<NavMeshAgent>(); // 獲取 NavMeshAgent

        // 設定每個 NPC 的唯一起始角度
        startAngle = (Mathf.PI * 2 / totalNPCs) * npcIndex; // 根據編號計算平均分配的角度
        angle = startAngle; // 將初始角度作為當前角度

        // 計算巡邏的起始位置座標
        float startX = center.x + Mathf.Cos(startAngle) * radius;
        float startZ = center.z + Mathf.Sin(startAngle) * radius;
        patrolStartPosition = new Vector3(startX, transform.position.y, startZ);

        // 將 NPC 移動到起始位置
        transform.position = patrolStartPosition;

        // 設定 NPC 朝向圓心
        Vector3 directionToCenter = (center - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(directionToCenter);

        Debug.Log($"NPC {npcIndex} 巡邏起始位置: {patrolStartPosition}");
    }

    void Update()
    {
        // 判斷玩家是否進入警戒範圍與視野範圍
        Vector3 directionToPlayer = player.position - transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;

        if (distanceToPlayer <= alertRadius && IsPlayerInFieldOfView(directionToPlayer))
        {
            if (!isPaused && !isRetreating)
            {
                StartPauseAndFacePlayer(directionToPlayer); // 開始暫停並面向玩家
            }
        }
        else
        {
            if (isRetreating || isReturningToPatrol)
            {
                if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
                {
                    HandleReturnToPatrol();
                }
            }
            else if (!isPaused)
            {
                PatrolAroundPoint();
            }
        }

        // 處理暫停計時
        if (isPaused)
        {
            pauseTimer -= Time.deltaTime;
            if (pauseTimer <= 0f)
            {
                EndPauseAndStartRetreat(directionToPlayer);
            }
        }
    }

    bool IsPlayerInFieldOfView(Vector3 directionToPlayer)
    {
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);
        return angleToPlayer <= fieldOfView / 2; // 檢查是否在視野範圍內
    }

    void StartPauseAndFacePlayer(Vector3 directionToPlayer)
    {
        isPaused = true;
        pauseTimer = 2f; // 設定暫停時間為 2 秒
        agent.isStopped = true; // 停止 NavMeshAgent 的移動

        // 計算並設置面向玩家的旋轉
        Vector3 lookDirection = directionToPlayer.normalized;
        Quaternion lookRotation = Quaternion.LookRotation(lookDirection);
        transform.rotation = lookRotation;
    }

    void EndPauseAndStartRetreat(Vector3 directionToPlayer)
    {
        isPaused = false;
        isRetreating = true; // 開始撤退狀態
        agent.isStopped = false; // 恢復 NavMeshAgent 的移動
        agent.speed = runSpeed;
        agent.SetDestination(retreatPoint.position); // 設置撤退目標
    }

    void HandleReturnToPatrol()
    {
        if (isRetreating)
        {
            isRetreating = false;
            isReturningToPatrol = true;
            agent.speed = returnSpeed;
            agent.SetDestination(returnPoint.position);
        }
        else if (isReturningToPatrol)
        {
            isReturningToPatrol = false;
            angle = startAngle; // 恢復到固定的巡邏起始角度
            PatrolAroundPoint();
        }
    }

    void PatrolAroundPoint()
    {
        angle += speed * Time.deltaTime;
        if (angle >= Mathf.PI * 2) angle -= Mathf.PI * 2;

        float x = center.x + Mathf.Cos(angle) * radius;
        float z = center.z + Mathf.Sin(angle) * radius;

        transform.position = new Vector3(x, transform.position.y, z);

        Vector3 directionToCenter = (center - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(directionToCenter);
        transform.rotation = lookRotation;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, alertRadius);

        // 畫出視野範圍
        Vector3 leftBoundary = Quaternion.Euler(0, -fieldOfView, 0) * transform.forward * alertRadius;
        Vector3 rightBoundary = Quaternion.Euler(0, fieldOfView, 0) * transform.forward * alertRadius;

        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, leftBoundary);
        Gizmos.DrawRay(transform.position, rightBoundary);
    }
}
