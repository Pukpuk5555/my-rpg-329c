using UnityEngine;
using UnityEngine.AI;

public enum CharState
{
    Idle,
    Walk,
    WalkToEnemy,
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

    [SerializeField] protected int curHP = 10;
    public int CurHP {get { return curHP; } }

    [SerializeField] protected Character curCharTarget;

    [SerializeField] protected float attackRange = 2f;

    [SerializeField] protected float attackCoolDown = 2f;
    [SerializeField] protected float attackTimer = 0f;

    [SerializeField]
    protected GameObject ringSelection;
    public GameObject RingSelection {get { return ringSelection; } }

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

    public void ToggleSelection(bool flag)
    {
        ringSelection.SetActive(flag);
    }

    public void ToAttackCharacter(Character target)
    {
        if(curHP <= 0 || state == CharState.Die)
            return;
        
        //lock target
        curCharTarget = target;
        
        //start walking to enemy
        navMeshAgent.SetDestination(target.transform.position);
        navMeshAgent.isStopped = false;
        
        SetState(CharState.WalkToEnemy);
    }

    protected void WalkToEnemyUpdate()
    {
        if (curCharTarget == null)
        {
            SetState(CharState.Idle);
            return;
        }

        float distance = Vector3.Distance(transform.position, 
            curCharTarget.transform.position);

        if (distance <= attackRange)
        {
            SetState(CharState.Attack);
            Attack();
        }
    }

    protected void Attack()
    {
        transform.LookAt(curCharTarget.transform);
        anim.SetTrigger("Attack");
    }

    protected void AttackUpdate()
    {
        if (curCharTarget == null || curCharTarget.CurHP <= 0)
        {
            SetState(CharState.Idle);
            return;
        }

        navMeshAgent.isStopped = true;

        attackTimer += Time.deltaTime;
        if (attackTimer >= attackCoolDown)
        {
            attackTimer = 0f;
            Attack();
        }

        float distance = Vector3.Distance(transform.position,
            curCharTarget.transform.position);
        
        if(distance > attackRange)
            SetState(CharState.WalkToEnemy);
    }
}
