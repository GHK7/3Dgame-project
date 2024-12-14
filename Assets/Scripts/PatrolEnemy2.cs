using UnityEngine;
using UnityEngine.AI;

public class EnemyAI2 : MonoBehaviour
{
    public float rotationAngle = 90f; // 每次旋轉的角度
    public float interval = 3f; // 旋轉的間隔時間（秒）
    public float rotationSpeed = 90f; // 平滑旋轉的速度（度/秒）
    public float timer = 0f;

    [Header("Chase Settings")]
    public GameObject player;
    public float viewAngle = 25f;     // 視野角度
    public LayerMask obstacleLayer;  // 障礙物層級
    public float chaseSpeed = 3.5f;  // 追擊速度

    private NavMeshAgent agent;
    private float targetYRotation; // 目標 Y 軸旋轉角度
 
    private bool isRotating = false;
    private bool isChasing = false;  // 當前模式狀態

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        // 初始化目標角度為當前物件的 Y 軸旋轉角度
        targetYRotation = transform.eulerAngles.y;
        TurnAround();
    }

    void Update()
    {
        if (isChasing)
        {
            ChaseMode();
        }
        if (!isChasing)
        {
            PatrolMode();
        }
    }
    private void PatrolMode()
    {
        // 判斷是否發現玩家
        Vector3 directionToPlayer = player.transform.position - transform.position;

        if (Mathf.Abs(Vector3.Angle(transform.forward, directionToPlayer)) < viewAngle)
        {
            Debug.Log("Chase");
            // 使用 Raycast 檢查是否有障礙物遮擋
            if (!Physics.Raycast(transform.position, directionToPlayer.normalized, 1000,obstacleLayer))
            {
                
                // 發現玩家，切換到追擊模式
                StartChasing();
                return;
            }
        }
        TurnAround();
    }
    private void ChaseMode()
    {
        Vector3 directionToPlayer = player.transform.position - transform.position;
        
        // 更新追擊目標
        agent.SetDestination(player.transform.position);

        // 如果玩家離開視野範圍，切回巡邏模式
        if (Mathf.Abs(Vector3.Angle(transform.forward, directionToPlayer)) >= viewAngle)
        {
            StopChasing();
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
        TurnAround(); // 繼續巡邏
    }
    private void TurnAround()
    {
        // 累計時間
        if (!isRotating)
        {
            timer += Time.deltaTime;

            // 每隔指定的時間開始旋轉
            if (timer >= interval)
            {
                targetYRotation += rotationAngle; // 更新目標角度
                isRotating = true;
                timer = 0f; // 重置計時器
            }
        }
        else
        {
            // 平滑旋轉到目標角度
            float currentYRotation = Mathf.LerpAngle(transform.eulerAngles.y, targetYRotation, rotationSpeed * Time.deltaTime / rotationAngle);
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, currentYRotation, transform.eulerAngles.z);

            // 檢查是否達到目標角度
            if (Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.y, targetYRotation)) < 0.1f)
            {
                transform.eulerAngles = new Vector3(transform.eulerAngles.x, targetYRotation, transform.eulerAngles.z); // 精確設定為目標角度
                isRotating = false;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {

        // 畫出視野範圍
        Vector3 leftBoundary = Quaternion.Euler(0, -viewAngle, 0) * transform.forward;
        Vector3 rightBoundary = Quaternion.Euler(0, viewAngle, 0) * transform.forward;

        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, leftBoundary);
        Gizmos.DrawRay(transform.position, rightBoundary);
    }
}
