using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
// �w�q���P�ĤH���A���T�|����
public enum EnemyState { Patrolling, Idle, Approaching, Retreating, Defence, Watching }
public class PatrollingDefensiveEnemy : MonoBehaviour
{
    // �����I�M���a���ޥ�
    public Transform[] patrolPoints; // �ĤH�n���ʪ������I�}�C
    public Transform player; // ���a��H���ޥ�
    public float attackRange = 5f; // �ĤH�b���Z�����|�}�l�M�h�������a
    public float retreatDistance = 8f; // �ĤH�b���Z�����|�}�l���񪱮a
    public float speed = 2f; // �ĤH���ʪ��t��
    public Text statusText; // �Ω���ܼĤH��e���A��UI��r����
    private NavMeshAgent navMeshAgent; // �Ω�ɯ誺NavMeshAgent�ե�


    private EnemyState currentState = EnemyState.Patrolling; // ��l���A�]�m������
    private int currentPointIndex = 0; // ��e�����I������
    private Transform currentTarget; // ��e�ؼШ����I
    private bool isGoHead = false; // �O�_���b�e�i

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>(); // ���NavMeshAgent�ե�
        // �ˬd�O�_���t�F�����I
        if (patrolPoints.Length == 0)
        {
            Debug.LogError("�S�����t�����I�I"); // �p�G�S�����t�����I�A�h�O�����~
            return;
        }
        currentTarget = patrolPoints[currentPointIndex]; // �]�m��l�����ޥؼ�
        UpdateStatusText(); // ��s���A��r�H�ϬM��e���A
    }

    void Update()
    {
        // �ˬd�O�_���t�F���a��H
        if (player == null)
        {
            Debug.LogError("�S�����t���a��H�I"); // �p�G�S�����t���a��H�A�h�O�����~
            return;
        }

        //�qgameobject�e�襴�X raycast �g�u�ˬd���S���������a
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, attackRange))
        {
            if (hit.collider.CompareTag("Player"))
            {
                // currentState = EnemyState.Retreating; // �p�G���a�b�����d�򤺡A�]�m���A���M�h
            }
        }


        float distanceToPlayer = Vector3.Distance(transform.position, player.position); // �p��P���a�������Z��
        UpdateState(distanceToPlayer); // �ھڻP���a���Z����s���A
        ActBasedOnState(); // �ھڷ�e���A������
    }

    void UpdateState(float distanceToPlayer)
    {
        // �ھڻP���a���Z����s�ĤH�����A
        if (distanceToPlayer < attackRange)
        {
            // currentState = EnemyState.Retreating; // �p�G���a�b�����d�򤺡A�]�m���A���M�h
        }
        else if (distanceToPlayer < retreatDistance || isGoHead)
        {
            currentState = EnemyState.Approaching; // �p�G���a�b�M�h�Z���������b�����d�򤺡A�]�m���A������
        }
        else if (currentState == EnemyState.Approaching || currentState == EnemyState.Retreating)
        {
            currentState = EnemyState.Patrolling; // �p�G���a�W�X�M�h�Z���A�]�m���A������
        }
        UpdateStatusText(); // ��s���A��r�H�ϬM��e���A
    }

    void ActBasedOnState()
    {
        // �ھڷ�e���A����ʧ@
        switch (currentState)
        {
            case EnemyState.Patrolling:
                Patrol(); // �b�����I��������
                break;
            case EnemyState.Approaching:
                ApproachPlayer(); // ���񪱮a
                break;
            case EnemyState.Retreating:
                RetreatFromPlayer(); // �q���a�B�M�h
                break;
            case EnemyState.Idle:
                // ����]�����A�O���R��
                break;
        }
    }

    void Patrol()
    {
        // �b���w�������I��������
        if (currentTarget == null)
            return; // �p�G�S����e�ؼСA�h�X��k

        //�� navMeshAgent ���ĤH���Ʋ��ʨ쨵���I
        // transform.position = Vector3.MoveTowards(transform.position, currentTarget.position, speed * Time.deltaTime);
        navMeshAgent.SetDestination(currentTarget.position);

        //�� gameobject ���V���ʤ�V�A���઺�ɭԤ��|��M��V�A�� Lerp �����७��
        // transform.LookAt(currentTarget);
        // transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(currentTarget.position - transform.position), Time.deltaTime * 20f);

        if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance < 0.5f) // �ˬd�ĤH�O�_��F��e�����I
        {
            currentPointIndex++; // ���ʨ�U�@�Ө����I
            if (currentPointIndex >= patrolPoints.Length)
            {
                currentPointIndex = 0; // �p�G�Ҧ������I���w�X�ݹL�A�h�`��^��Ĥ@�Ө����I
            }
            currentTarget = patrolPoints[currentPointIndex]; // ��s��e�ؼШ�U�@�Ө����I
            UpdateStatusText(); // ��s���A��r�H�ϬM��e���A
        }
    }

    void ApproachPlayer()
    {
        // �V���a����m���ʡA�ëO���@�w���Z���A�ϥ�NavMeshAgent�ե�
        navMeshAgent.SetDestination(player.position);
    }

    void RetreatFromPlayer()
    {
        // �������a����m����
        transform.position = Vector3.MoveTowards(transform.position, player.position, -speed * Time.deltaTime);
    }

    void UpdateStatusText()
    {
        // ��s���A��r����H��ܷ�e���A
        if (statusText != null)
        {
            statusText.text = $"��e���A: {currentState}"; // �bUI��r������ܷ�e���A
        }
    }

    public void SetApproachPlayer(PlayerMovement player)
    {
        isGoHead = true;
        // �]�m�ĤH���ؼЪ��a
        this.player = player.transform;
        currentState = EnemyState.Approaching;
    }

    // ø�s�ĤH�����޸�|
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

        //ø�s�g�u
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward * attackRange);
    }
}
