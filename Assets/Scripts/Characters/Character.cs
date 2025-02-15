using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using Unity.VisualScripting;

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

    public Character CurCharTarget
    { get { return curCharTarget; } set { curCharTarget = value; } }

    [SerializeField] protected float attackRange = 2f;
    public float AttackRange { get { return attackDamage; } }
    
    [SerializeField] protected int attackDamage = 3;

    [SerializeField] protected float attackCoolDown = 2f;
    [SerializeField] protected float attackTimer = 0f;

    [SerializeField] protected float findingRange = 20f;
    public float FindingRange { get { return findingRange; } }

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

        navMeshAgent.SetDestination(curCharTarget.transform.position);

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
        
        //attack logic
        AttackLogic();
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

        if (distance > attackRange)
        {
            SetState(CharState.WalkToEnemy);
            navMeshAgent.SetDestination(curCharTarget.transform.position);
            navMeshAgent.isStopped = false;
        }
    }

    protected virtual IEnumerator DestroyObject()
    {
        yield return new WaitForSeconds(5f);
        Destroy(gameObject);
    }

    protected virtual void Die()
    {
        navMeshAgent.isStopped = true;
        SetState(CharState.Die);
        
        anim.SetTrigger("Die");

        StartCoroutine(DestroyObject());
    }

    public void RecieveDamage(Character enemy)
    {
        if(curHP <= 0 || state == CharState.Die)
            return;

        curHP -= enemy.attackDamage;
        if (curHP <= 0)
        {
            curHP = 0;
            Die();
        }
    }

    protected void AttackLogic()
    {
        Character target = curCharTarget.GetComponent<Character>();
        
        if(target != null)
            target.RecieveDamage(this);
    }

    public bool IsMyEnemy(string targetTag)
    {
        string myTag = gameObject.tag;

        if ((myTag == "Hero" || myTag == "Player") && targetTag == "Enemy")
            return true;

        if (myTag == "Enemy" && (targetTag == "Hero" || targetTag == "Player"))
            return true;

        return false;
    }
}
