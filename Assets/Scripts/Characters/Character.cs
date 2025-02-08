using UnityEngine;
using UnityEngine.AI;

public enum CharState
{
    Idle,
    Walk,
    Attack,
    Hit,
    Die
}

public abstract class Character : MonoBehaviour
{
    protected NavMeshAgent navMeshAgent;

    protected Animator anim;
    public Animator Anim { get { return anim; } }

    [SerializeField]
    protected CharState state;
    public CharState State { get { return state; } }

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
    }

    public void SetState(CharState s)
    {
        state = s;

        if (state == CharState.Idle)
        {
            navMeshAgent.isStopped = true;
            navMeshAgent.ResetPath();
        }
    }

    public void WalkToPosition(Vector3 dest)
    {
        if (navMeshAgent != null)
        {
            navMeshAgent.SetDestination(dest);
            navMeshAgent.isStopped = false;
        }
        SetState(CharState.Walk);
    }

    public void WalkUpdate()
    {
        float distance = Vector3.Distance(transform.position, navMeshAgent.destination);
        Debug.Log(distance);
        
        if(distance <= navMeshAgent.stoppingDistance)
            SetState(CharState.Idle);
    }
}
