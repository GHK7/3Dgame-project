using UnityEngine;
using UnityEngine.AI;

public class Villagers : MonoBehaviour
{
    public Transform player; // ���a
    public Transform retreatPoint; // �M�h�I
    public Transform returnPoint; // ��^�I (��^���ޮɪ��ؼ��I)
    public float alertRadius = 200f; // ĵ�٥b�|
    public float fieldOfView = 45f; // �����d��
    public float runSpeed = 50f;
    public float returnSpeed = 15f;

    private NavMeshAgent agent;
    private bool isRetreating = false;
    private bool isReturningToPatrol = false;

    [Header("�]�w��ιB�ʰѼ�")]
    public Vector3 center = Vector3.zero; // ��ߦ�m
    public float radius = 5f; // �ꪺ�b�|
    public float speed = 2f; // ���ʳt�� (���t�סA���רC��)

    private float angle; // ��e������
    private float startAngle; // ���ު��_�l����
    public int npcIndex; // NPC ���ߤ@�s�� (��ʳ]�m�ε{�ǥͦ�)
    public int totalNPCs = 18; // ������ NPC ���`��

    [Header("���ް_�l�y�С]�۰ʭp��^")]
    public Vector3 patrolStartPosition; // NPC ���ް_�l��m (�۰ʭp��)

    private float pauseTimer = 1.6f; // �O���Ȱ��ɶ�
    private bool isPaused = false; // �O�_���b�Ȱ�

    private Animator animator;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>(); // ��� NavMeshAgent

        // �]�w�C�� NPC ���ߤ@�_�l����
        startAngle = (Mathf.PI * 2 / totalNPCs) * npcIndex; // �ھڽs���p�⥭�����t������
        angle = startAngle; // �N��l���ק@����e����

        // �p�⨵�ު��_�l��m�y��
        float startX = center.x + Mathf.Cos(startAngle) * radius;
        float startZ = center.z + Mathf.Sin(startAngle) * radius;
        patrolStartPosition = new Vector3(startX, transform.position.y, startZ);

        // �N NPC ���ʨ�_�l��m
        transform.position = patrolStartPosition;

        // �]�w NPC �¦V���
        Vector3 directionToCenter = (center - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(directionToCenter);

        //Debug.Log($"NPC {npcIndex} ���ް_�l��m: {patrolStartPosition}");

        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // �P�_���a�O�_�i�Jĵ�ٽd��P�����d��
        Vector3 directionToPlayer = player.position - transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;

        if (distanceToPlayer <= alertRadius && IsPlayerInFieldOfView(directionToPlayer))
        {
            if (!isPaused && !isRetreating)
            {
                StartPauseAndFacePlayer(directionToPlayer); 
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
        return angleToPlayer <= fieldOfView / 2; 
    }

    void StartPauseAndFacePlayer(Vector3 directionToPlayer)
    {
        isPaused = true;
        animator.SetBool("IsTerrified", true);
        animator.SetBool("IsDancing", false);
        pauseTimer = 1f; // �]�w�Ȱ��ɶ��� 2 ��
        agent.isStopped = true; // ���� NavMeshAgent ������

        // �p��ó]�m���V���a������
        Vector3 lookDirection = directionToPlayer.normalized;
        Quaternion lookRotation = Quaternion.LookRotation(lookDirection);
        transform.rotation = lookRotation;
    }

    void EndPauseAndStartRetreat(Vector3 directionToPlayer)
    {
        isPaused = false;
        isRetreating = true; // �}�l�M�h���A
        animator.SetBool("IsRetreating", true );
        agent.isStopped = false; // ��_ NavMeshAgent ������
        agent.speed = runSpeed;
        agent.SetDestination(retreatPoint.position); // �]�m�M�h�ؼ�
    }

    void HandleReturnToPatrol()
    {
        if (isRetreating)
        {
            isRetreating = false;
            animator.SetBool("IsTerrified", false);
            animator.SetBool("IsRetreating", false);
            isReturningToPatrol = true;
            agent.speed = returnSpeed;
            agent.SetDestination(returnPoint.position);
        }
        else if (isReturningToPatrol)
        {
            isReturningToPatrol = false;
            angle = startAngle; // ��_��T�w�����ް_�l����
            animator.SetBool("IsDancing", true);
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

        // �e�X�����d��
        Vector3 leftBoundary = Quaternion.Euler(0, -fieldOfView, 0) * transform.forward * alertRadius;
        Vector3 rightBoundary = Quaternion.Euler(0, fieldOfView, 0) * transform.forward * alertRadius;

        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, leftBoundary);
        Gizmos.DrawRay(transform.position, rightBoundary);
    }
}
