using UnityEngine;
using UnityEngine.AI;

public class StandEnemy : MonoBehaviour
{
    public float rotationAngle = 90f;
    public float interval = 3f;
    public float rotationSpeed = 90f;
    public float timer = 0f;

    [Header("Chase Settings")]
    public GameObject player;
    public float viewAngle = 25f;
    public float alertRadius = 100f;
    public LayerMask obstacleLayer;
    public float chaseSpeed = 3.5f;
    public int damage = 15;
    public float attackRange = 5f;
    public float attackCooldown = 2f;

    [Header("Audio Settings")]
    public AudioClip chaseAudioClip;
    public AudioClip patrolAudioClip;
    public float minAudioDistance = 5f; // 最小音量距離
    public float maxAudioDistance = 50f; // 最大音量距離

    private NavMeshAgent agent;
    private float targetYRotation;
    private float lastAttackTime;
    public HealthBar healthBar;
    private bool isRotating = false;
    private bool isChasing = false;

    private Animator animator;
    private AudioSource audioSource;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        targetYRotation = transform.eulerAngles.y;
        TurnAround();

        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        // 確保音源被正確設定
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // 設定音源的屬性
        audioSource.loop = true;
        audioSource.spatialBlend = 1f;
        audioSource.minDistance = minAudioDistance;
        audioSource.maxDistance = maxAudioDistance;
        audioSource.dopplerLevel = 0f;
        audioSource.rolloffMode = AudioRolloffMode.Linear;
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
        Vector3 directionToPlayer = player.transform.position - transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;

        if (distanceToPlayer <= alertRadius && Mathf.Abs(Vector3.Angle(transform.forward, directionToPlayer)) < viewAngle)
        {
            if (!Physics.Raycast(transform.position, directionToPlayer.normalized, distanceToPlayer, obstacleLayer))
            {
                StartChasing();
                return;
            }
        }

        // 播放巡邏音效
        if (audioSource.clip != patrolAudioClip || !audioSource.isPlaying)
        {
            audioSource.clip = patrolAudioClip;
            audioSource.Play();
        }
        TurnAround();
        animator.SetBool("IsAttacking", false);
    }

    private void ChaseMode()
    {
        Vector3 directionToPlayer = player.transform.position - transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;

        agent.SetDestination(player.transform.position);
        if (distanceToPlayer <= attackRange)
        {
            AttackPlayer();
        }
        else if (distanceToPlayer > attackRange)
        {
            animator.SetBool("IsAttacking", false);
        }

        if (Mathf.Abs(Vector3.Angle(transform.forward, directionToPlayer)) >= viewAngle)
        {
            StopChasing();
        }

        // 播放追逐音效
        if (audioSource.clip != chaseAudioClip || !audioSource.isPlaying)
        {
            audioSource.clip = chaseAudioClip;
            audioSource.Play();
        }
    }

    private void StartChasing()
    {
        isChasing = true;
        agent.speed = chaseSpeed;
        animator.SetBool("IsChasing", true);

        // 播放追逐音效
        if (!audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }

    private void StopChasing()
    {
        isChasing = false;
        TurnAround();
        animator.SetBool("IsChasing", false);

        // 停止播放追逐音效
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }

    void AttackPlayer()
    {
        Vector3 directionToPlayer = player.transform.position - transform.position;

        Quaternion targetRotation = Quaternion.LookRotation(new Vector3(directionToPlayer.x, 0f, directionToPlayer.z));
        transform.rotation = targetRotation;

        if (Time.time - lastAttackTime >= attackCooldown)
        {
            lastAttackTime = Time.time;
            healthBar.beenAttacked(damage);
        }
        animator.SetBool("IsAttacking", true);
    }

    private void TurnAround()
    {
        if (!isRotating)
        {
            timer += Time.deltaTime;

            if (timer >= interval)
            {
                targetYRotation += rotationAngle;
                isRotating = true;
                timer = 0f;
                animator.SetBool("IsRotating", true);
            }
        }
        else
        {
            float currentYRotation = Mathf.LerpAngle(transform.eulerAngles.y, targetYRotation, rotationSpeed * Time.deltaTime / rotationAngle);
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, currentYRotation, transform.eulerAngles.z);

            if (Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.y, targetYRotation)) < 0.1f)
            {
                transform.eulerAngles = new Vector3(transform.eulerAngles.x, targetYRotation, transform.eulerAngles.z);
                isRotating = false;
                animator.SetBool("IsRotating", false);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 leftBoundary = Quaternion.Euler(0, -viewAngle, 0) * transform.forward;
        Vector3 rightBoundary = Quaternion.Euler(0, viewAngle, 0) * transform.forward;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, alertRadius);

        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, leftBoundary);
        Gizmos.DrawRay(transform.position, rightBoundary);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}

