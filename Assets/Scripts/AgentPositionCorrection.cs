using UnityEngine;
using UnityEngine.AI;

public class AgentPositionCorrection : MonoBehaviour
{
    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (!agent.isOnNavMesh)
        {
            Vector3 correctedPosition;
            if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
            {
                correctedPosition = hit.position;
                agent.Warp(correctedPosition); // 移動代理到新的位置
            }
        }
    }
}
