using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
// 定義不同敵人狀態的枚舉類型
public enum EnemyState { Patrolling, Idle, Approaching, Retreating, Defence, Watching }
public class PatrollingDefensiveEnemy : MonoBehaviour
{
    // 巡邏點和玩家的引用
    public Transform[] patrolPoints; // 敵人要移動的巡邏點陣列
    public Transform player; // 玩家對象的引用
    public float attackRange = 5f; // 敵人在此距離內會開始撤退遠離玩家
    public float retreatDistance = 8f; // 敵人在此距離內會開始接近玩家
    public float speed = 2f; // 敵人移動的速度
    public Text statusText; // 用於顯示敵人當前狀態的UI文字元件
    private NavMeshAgent navMeshAgent; // 用於導航的NavMeshAgent組件


    private EnemyState currentState = EnemyState.Patrolling; // 初始狀態設置為巡邏
    private int currentPointIndex = 0; // 當前巡邏點的索引
    private Transform currentTarget; // 當前目標巡邏點
    //private bool isGoHead = false; // 是否正在前進

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        navMeshAgent = GetComponent<NavMeshAgent>(); // 獲取NavMeshAgent組件
        // 檢查是否分配了巡邏點
        if (patrolPoints.Length == 0)
        {
            Debug.LogError("沒有分配巡邏點！"); // 如果沒有分配巡邏點，則記錄錯誤
            return;
        }
        currentTarget = patrolPoints[currentPointIndex]; // 設置初始的巡邏目標
        UpdateStatusText(); // 更新狀態文字以反映當前狀態
    }

    void Update()
    {
        // 檢查是否分配了玩家對象
        if (player == null)
        {
            Debug.LogError("沒有分配玩家對象！"); // 如果沒有分配玩家對象，則記錄錯誤
            return;
        }

        //從gameobject前方打出 raycast 射線檢查有沒有打中玩家
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, attackRange))
        {
            if (hit.collider.CompareTag("Player"))
            {
                currentState = EnemyState.Retreating; // 如果玩家在攻擊範圍內，設置狀態為撤退
                Debug.Log("Player");
            }
        }


        float distanceToPlayer = Vector3.Distance(transform.position, player.position); // 計算與玩家之間的距離
        UpdateState(distanceToPlayer); // 根據與玩家的距離更新狀態
        ActBasedOnState(); // 根據當前狀態執行行動
    }

    void UpdateState(float distanceToPlayer)
    {
        if (distanceToPlayer < attackRange) // 玩家在攻擊範圍內
        {
            currentState = EnemyState.Retreating;
        }
        else if (distanceToPlayer < retreatDistance) // 玩家在撤退範圍內
        {
            currentState = EnemyState.Approaching;
        }
        else if (currentState != EnemyState.Patrolling) // 如果不在撤退或接近狀態
        {
            currentState = EnemyState.Patrolling;
        }
        UpdateStatusText();
    }

    void ActBasedOnState()
    {
        // 根據當前狀態執行動作
        switch (currentState)
        {
            case EnemyState.Patrolling:
                Patrol(); // 在巡邏點之間巡邏
                break;
            case EnemyState.Approaching:
                ApproachPlayer(); // 接近玩家
                break;
            case EnemyState.Retreating:
                RetreatFromPlayer(); // 從玩家處撤退
                break;
            case EnemyState.Idle:
                // 什麼也不做，保持靜止
                break;
        }
    }

    void Patrol()
    {
        // 在指定的巡邏點之間巡邏
        if (currentTarget == null)
            return; // 如果沒有當前目標，退出方法

        //用 navMeshAgent 讓敵人平滑移動到巡邏點
        // transform.position = Vector3.MoveTowards(transform.position, currentTarget.position, speed * Time.deltaTime);
        navMeshAgent.SetDestination(currentTarget.position);

        //讓 gameobject 面向移動方向，旋轉的時候不會突然轉向，用 Lerp 讓旋轉平滑
        // transform.LookAt(currentTarget);
        // transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(currentTarget.position - transform.position), Time.deltaTime * 20f);

        if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance < 1f) // 檢查敵人是否到達當前巡邏點
        {
            currentPointIndex++; // 移動到下一個巡邏點
            if (currentPointIndex >= patrolPoints.Length)
            {
                currentPointIndex = 0; // 如果所有巡邏點都已訪問過， 則循環回到第一個巡邏點
            }
            currentTarget = patrolPoints[currentPointIndex]; // 更新當前目標到下一個巡邏點
            UpdateStatusText(); // 更新狀態文字以反映當前狀態
        }
    }

    void ApproachPlayer()
    {
        // 向玩家的位置移動，並保持一定的距離，使用NavMeshAgent組件
        navMeshAgent.SetDestination(player.position);
    }

    void RetreatFromPlayer()
    {
        // 遠離玩家的位置移動
        transform.position = Vector3.MoveTowards(transform.position, player.position, -speed * Time.deltaTime);
    }

    void UpdateStatusText()
    {
        // 更新狀態文字元件以顯示當前狀態
        if (statusText != null)
        {
            statusText.text = $"當前狀態: {currentState}"; // 在UI文字元件中顯示當前狀態
        }
    }

    public void SetApproachPlayer(PlayerMovement player)
    {
        //isGoHead = true;
        // 設置敵人的目標玩家
        this.player = player.transform;
        currentState = EnemyState.Approaching;
    }

    // 繪製敵人的巡邏路徑
    void OnDrawGizmos()
    {
        if (patrolPoints.Length > 0)
        {
            for (int i = 0; i < patrolPoints.Length; i++)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawSphere(patrolPoints[i].position, 0.5f);
                if (i < patrolPoints.Length - 1)
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawLine(patrolPoints[i].position, patrolPoints[i + 1].position);
                }
            }
        }

        //繪製射線
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward * attackRange);
    }
}
