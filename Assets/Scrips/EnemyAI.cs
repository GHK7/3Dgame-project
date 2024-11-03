using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class EnemyAI : MonoBehaviour
{
    public Transform player;              // 玩家对象
    public float attackRange = 5f;        // 敌人的攻击范围
    public float attackCooldown = 2f;     // 攻击冷却时间
    public float speed = 3.5f;            // 敌人移动速度
    public int maxAttacks = 3;            // 最大攻击次数
    private int attackCount = 0;          // 当前攻击计数
    private NavMeshAgent navMeshAgent;    // 导航代理，用于敌人的移动
    private Animator animator;             // 动画组件
    private float lastAttackTime;
    private bool isPaused = false;        // 检查敌人是否处于暂停状态



    void Start()
    {

        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();  // 获取动画组件
        navMeshAgent.speed = speed;           // 设置敌人移动速度
        lastAttackTime = -attackCooldown;     // 使敌人一开始可以立即攻击
    }

    void Update()
    {
        if (isPaused) return;  // 如果敌人处于暂停状态，停止更新

        // 设置敌人的目标为玩家的位置
        navMeshAgent.SetDestination(player.position);

        float distanceToPlayer = Vector3.Distance(player.position, transform.position);

        // 更新动画参数，设置是否在“跑动”状态
        if (navMeshAgent.velocity.magnitude > 0.1f)
        {
            animator.SetBool("isRunning", true);
            animator.SetBool("isIdle", false);
        }
        else
        {
            animator.SetBool("isRunning", false);
            animator.SetBool("isIdle", true);
        }

        if (distanceToPlayer <= attackRange)  // 如果玩家在攻击范围内
        {
            AttackPlayer();
        }
    }

    void AttackPlayer()
    {
        if (Time.time - lastAttackTime >= attackCooldown)  // 检查冷却时间
        {
            Debug.Log("Enemy attacks the player!");  // 在这里触发敌人的攻击动画和伤害逻辑
            lastAttackTime = Time.time;  // 记录攻击时间
            animator.SetTrigger("attack");  // 触发攻击动画
            StartCoroutine(PauseAfterAttack(1f));  // 调用协程暂停敌人1秒
            attackCount++;

            if (attackCount >= maxAttacks)
            {
                EndGame();  // 结束游戏逻辑
            }
            else
            {
                StartCoroutine(PauseAfterAttack(1f));  // 调用协程暂停敌人1秒
            }
        }
    }

    IEnumerator PauseAfterAttack(float pauseTime)
    {
        isPaused = true;  // 设置敌人状态为暂停
        navMeshAgent.isStopped = true;  // 暂停敌人的移动
        navMeshAgent.velocity = Vector3.zero;  // 确保 NavMeshAgent 的速度为零
        animator.SetBool("isRunning", false);  // 设置动画为停止状态
        animator.SetBool("isIdle", true);
        Debug.Log("Enemy paused after attack.");

        yield return new WaitForSeconds(pauseTime);  // 等待指定的暂停时间

        navMeshAgent.isStopped = false;  // 恢复敌人移动
        isPaused = false;  // 取消暂停状态
        Debug.Log("Enemy resumed movement.");
    }

    void EndGame()
    {
        Debug.Log("Game Over! Player attacked 3 times.");
        SceneManager.LoadScene("GameOver");
        Time.timeScale = 0f;  // 暂停游戏
    }
}
