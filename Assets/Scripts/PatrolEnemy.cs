using UnityEngine;
using UnityEngine.AI;

public class PatrolEnemy : MonoBehaviour
{
    [Header("Patrol Settings")]
    public Transform[] patrolPoints; // �����I
    private int currentPatrolIndex = 0;

    [Header("Chase Settings")]
    public GameObject player;
    public float viewAngle = 45f;     // ��������
    public float alertRadius = 10f;  // ĵ�٥b�|
    public LayerMask obstacleLayer;  // ��ê���h��
    public float rotationSpeed = 5f; // �l������t��

    [Header("Movement Settings")]
    public float patrolSpeed = 2f;   // ���޳t��
    public float chaseSpeed = 3.5f;  // �l���t��

    private NavMeshAgent agent;
    private bool isChasing = false;  // ��e�Ҧ����A



    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = patrolSpeed;
        currentPatrolIndex = 0;
        PatrolToNextPoint();
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
        // �P�_�O�_�o�{���a
        Vector3 directionToPlayer = player.transform.position - transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;

        if (distanceToPlayer <= alertRadius && Mathf.Abs(Vector3.Angle(transform.forward, directionToPlayer)) < viewAngle)
        {
            // �ϥ� Raycast �ˬd�O�_����ê���B��
            if (!Physics.Raycast(transform.position, directionToPlayer.normalized, distanceToPlayer, obstacleLayer))
            {
                // �o�{���a�A������l���Ҧ�
                StartChasing();
                return;
            }
        }

        // �����I�޿�
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            PatrolToNextPoint();
        }
    }

    /*private void ChaseMode()
    {
        Vector3 directionToPlayer = player.transform.position - transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;

        // ��s�l���ؼ�
        agent.SetDestination(player.transform.position);

        // �p�G���a���}ĵ�ٽd��A���^���޼Ҧ�
        if (distanceToPlayer > alertRadius)
        {
            StopChasing();
        }
    }*/

    private void ChaseMode()
    {
        Vector3 directionToPlayer = (player.transform.position - transform.position).normalized;

        // ���V���a�ò���
        MoveAndRotate(directionToPlayer, chaseSpeed);

        // �p�G���a���}ĵ�٥b�|�A�����^���޼Ҧ�
        if (Vector3.Distance(transform.position, player.transform.position) > alertRadius)
        {
            StopChasing();
        }
    }

    private void MoveAndRotate(Vector3 direction, float speed)
    {
        // �����޿�
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        // �����޿�
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    private void StartChasing()
    {
        isChasing = true;
        agent.speed = chaseSpeed; // ������l���t��
    }

    private void StopChasing()
    {
        isChasing = false;
        agent.speed = patrolSpeed; // �����쨵�޳t��
        PatrolToNextPoint(); // �~����
    }

    private void PatrolToNextPoint()
    {
        if (patrolPoints.Length == 0) return;

        // �]�m�ؼЬ��U�@�Ө����I
        agent.SetDestination(patrolPoints[currentPatrolIndex].position);
        currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length; // �`�����������I
    }

    private void OnDrawGizmosSelected()
    {
        // �e�Xĵ�ٽd��
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, alertRadius);

        // �e�X�����d��
        Vector3 leftBoundary = Quaternion.Euler(0, -viewAngle, 0) * transform.forward * alertRadius;
        Vector3 rightBoundary = Quaternion.Euler(0, viewAngle, 0) * transform.forward * alertRadius;

        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, leftBoundary);
        Gizmos.DrawRay(transform.position, rightBoundary);
    }
}
