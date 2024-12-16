using UnityEngine;
using UnityEngine.AI;

public class EnemyAI2 : MonoBehaviour
{
    public float rotationAngle = 90f; // �C�����઺����
    public float interval = 3f; // ���઺���j�ɶ��]��^
    public float rotationSpeed = 90f; // ���Ʊ��઺�t�ס]��/��^
    public float timer = 0f;

    [Header("Chase Settings")]
    public GameObject player;
    public float viewAngle = 25f;     // ��������
    public LayerMask obstacleLayer;  // ��ê���h��
    public float chaseSpeed = 3.5f;  // �l���t��

    private NavMeshAgent agent;
    private float targetYRotation; // �ؼ� Y �b���ਤ��
 
    private bool isRotating = false;
    private bool isChasing = false;  // ��e�Ҧ����A

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        // ��l�ƥؼШ��׬���e���� Y �b���ਤ��
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
        // �P�_�O�_�o�{���a
        Vector3 directionToPlayer = player.transform.position - transform.position;

        if (Mathf.Abs(Vector3.Angle(transform.forward, directionToPlayer)) < viewAngle)
        {
            //Debug.Log("Chase");
            // �ϥ� Raycast �ˬd�O�_����ê���B��
            if (!Physics.Raycast(transform.position, directionToPlayer.normalized, 1000,obstacleLayer))
            {
                
                // �o�{���a�A������l���Ҧ�
                StartChasing();
                return;
            }
        }
        TurnAround();
    }
    private void ChaseMode()
    {
        Vector3 directionToPlayer = player.transform.position - transform.position;
        
        // ��s�l���ؼ�
        agent.SetDestination(player.transform.position);

        // �p�G���a���}�����d��A���^���޼Ҧ�
        if (Mathf.Abs(Vector3.Angle(transform.forward, directionToPlayer)) >= viewAngle)
        {
            StopChasing();
        }
    }
    private void StartChasing()
    {
        isChasing = true;
        agent.speed = chaseSpeed; // ������l���t��
    }

    private void StopChasing()
    {
        isChasing = false;
        TurnAround(); // �~����
    }
    private void TurnAround()
    {
        // �֭p�ɶ�
        if (!isRotating)
        {
            timer += Time.deltaTime;

            // �C�j���w���ɶ��}�l����
            if (timer >= interval)
            {
                targetYRotation += rotationAngle; // ��s�ؼШ���
                isRotating = true;
                timer = 0f; // ���m�p�ɾ�
            }
        }
        else
        {
            // ���Ʊ����ؼШ���
            float currentYRotation = Mathf.LerpAngle(transform.eulerAngles.y, targetYRotation, rotationSpeed * Time.deltaTime / rotationAngle);
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, currentYRotation, transform.eulerAngles.z);

            // �ˬd�O�_�F��ؼШ���
            if (Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.y, targetYRotation)) < 0.1f)
            {
                transform.eulerAngles = new Vector3(transform.eulerAngles.x, targetYRotation, transform.eulerAngles.z); // ��T�]�w���ؼШ���
                isRotating = false;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {

        // �e�X�����d��
        Vector3 leftBoundary = Quaternion.Euler(0, -viewAngle, 0) * transform.forward;
        Vector3 rightBoundary = Quaternion.Euler(0, viewAngle, 0) * transform.forward;

        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, leftBoundary);
        Gizmos.DrawRay(transform.position, rightBoundary);
    }
}
