using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

public enum CharState
{
    Idle,
    Walk,
    WalkToEnemy,
    Attack,
    WalkToMagicCast,
    MagicCast,
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

    [SerializeField]
    protected List<Magic> magicSkills = new List<Magic>();
    public List<Magic> MagicSkills
    { get { return magicSkills; } set { magicSkills = value; } }

    [SerializeField]
    protected Magic curMagicCast = null;
    public Magic CurMagicCast
    { get { return curMagicCast; } set { curMagicCast = value; } }

    [SerializeField]
    protected bool isMagicMode = false;
    public bool IsMagicMode
    { get { return isMagicMode; } set { isMagicMode = value; } }

    protected VFXManager vfxManager;

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
    }

    public void charInit(VFXManager vfxM)
    {
        vfxManager = vfxM;
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

        if (isMagicMode)
            SetState(CharState.WalkToMagicCast);
        else
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
        if (curCharTarget == null)
            return;;

        if (curCharTarget.CurHP <= 0)
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

    public void RecieveDamage(int damage)
    {
        if(curHP <= 0 || state == CharState.Die)
            return;

        curHP -= damage;
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
            target.RecieveDamage(attackDamage);
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

    protected void MagicCastLogic(Magic magic)
    {
        Character target = curCharTarget.GetComponent<Character>();

        if (target != null)
            target.RecieveDamage(magic.Power);
    }

    private IEnumerator ShootMagicCast(Magic curMagicCast)
    {
        if (vfxManager != null)
            vfxManager.ShootMagic(curMagicCast.ShootId,
                                   transform.position,
                                   curCharTarget.transform.position,
                                   curMagicCast.ShootTime);

        yield return new WaitForSeconds(curMagicCast.ShootTime);

        //Cast logic
        MagicCastLogic(curMagicCast);
        isMagicMode = false;

        SetState(CharState.Idle);
    }

    private IEnumerator LoadMagicCast(Magic curMagicCast)
    {
        if (vfxManager != null)
            vfxManager.LoadMagic(curMagicCast.LoadId,
                                transform.position,
                                curMagicCast.LoadTime);

        yield return new WaitForSeconds(curMagicCast.LoadTime);

        StartCoroutine(ShootMagicCast(curMagicCast));
    }

    private void MagicCast(Magic curMagicCast)
    {
        transform.LookAt(curCharTarget.transform);
        anim.SetTrigger("MagicAttack");

        StartCoroutine(LoadMagicCast(curMagicCast));
    }

    protected void WalkToMagicCastUpdate()
    {
        if(curCharTarget == null || curMagicCast == null)
        {
            SetState(CharState.Idle);
            return;
        }

        navMeshAgent.SetDestination(curCharTarget.transform.position);

        float distance = Vector3.Distance(transform.position,
                             curCharTarget.transform.position);

        if(distance <= curMagicCast.Range)
        {
            navMeshAgent.isStopped = true;
            SetState(CharState.MagicCast);

            MagicCast(curMagicCast);
        }
    }
}
