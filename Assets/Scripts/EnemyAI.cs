using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public Transform player;              // 玩家对象
    public float attackRange = 2f;        // 敌人的攻击范围
    public float attackCooldown = 2f;     // 攻击冷却时间

    private NavMeshAgent navMeshAgent;    // 导航代理，用于敌人的移动
    private float lastAttackTime;
    private bool isPaused = false;
    private UnityEngine.AI.NavMeshAgent agent;

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        lastAttackTime = -attackCooldown;  // 使敌人一开始可以立即攻击
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
    }

    public static void StartChasingPlayer(GameObject player)
    {
        // 確認敵人開始追蹤邏輯
        EnemyAI[] enemies = FindObjectsOfType<EnemyAI>();
        foreach (EnemyAI enemy in enemies)
        {
            enemy.player = player.transform;
            enemy.agent.SetDestination(player.transform.position);
        }
    }

    void Update()
    {
        if (isPaused) return;
        // 始终设置敌人的目标为玩家的位置
        navMeshAgent.SetDestination(player.position);

        float distanceToPlayer = Vector3.Distance(player.position, transform.position);

        if (distanceToPlayer <= attackRange)  // 如果玩家在攻击范围内
        {
            AttackPlayer();
        }

        if (player != null)
        {
            agent.SetDestination(player.position);
        }
    }

    void AttackPlayer()
    {
        if (Time.time - lastAttackTime >= attackCooldown)  // 检查冷却时间
        {
            Debug.Log("Enemy attacks the player!");  // 在这里触发敌人的攻击动画和伤害逻辑
            lastAttackTime = Time.time;  // 记录攻击时间
            StartCoroutine(PauseAfterAttack(1f));
        }
    }
    IEnumerator PauseAfterAttack(float pauseTime)
    {
        isPaused = true;  // 设置敌人状态为暂停
        navMeshAgent.isStopped = true;  // 暂停敌人的移动
        yield return new WaitForSeconds(pauseTime);  // 等待指定的暂停时间
        navMeshAgent.isStopped = false;  // 恢复敌人移动
        isPaused = false;  // 取消暂停状态
    }
}
