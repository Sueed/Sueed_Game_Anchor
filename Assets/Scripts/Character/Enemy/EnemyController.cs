using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public enum EnemyStates{GUARD, PATROL, CHASE, DEAD};
// 确保在没有该组件是自动添加NavMeshAgent组件
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(CharacterStatus))]
public class EnemyController : MonoBehaviour,IEndGmaeObserver
{
    private EnemyStates enemyStates;
    private NavMeshAgent agent;
    private Animator animator;
    protected CharacterStatus characterStatus;

    [Header("Basic Settings")]
    public float sightRadius;
    public bool isGuard;
    private float lastAttackTime;
    private float speed;
    protected GameObject attackTarget;
    public float lookAtTime;
    private float remainLookAtTime;

    [Header("Patrol State")]
    public float patrolRange;

    private Vector3 wayPoint;
    private Vector3 guardPos;
    private Quaternion guardRotation;
    private float baseStoppingDistance;

    // bool for animation switch
    bool isWalk;
    bool isChase;
    bool isFollow;
    bool isDead;
    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        characterStatus = GetComponent<CharacterStatus>();
        speed = agent.speed;
        guardPos = transform.position;
        guardRotation = transform.rotation; 
        remainLookAtTime = lookAtTime;
        baseStoppingDistance = agent.stoppingDistance;
    }

    // Start is called before the first frame update
    void Start()
    {
        if(isGuard) enemyStates = EnemyStates.GUARD;
        else
        {
            enemyStates = EnemyStates.PATROL;
            getNewWayPoint();
        }
        //FIXME:场景切换修改
        GameManager.Instance.AddObserver(this);
    }

    // void OnEnable() 
    // {
    //     GameManager.Instance.AddObserver(this);
    // }

    void OnDisable() 
    {
        if(!GameManager.IsInitialized) return;
        GameManager.Instance.RemoveObserver(this);
    }

    // Update is called once per frame
    void Update()
    {
        if(characterStatus.currentHealth == 0)
            isDead = true;
        SwitchStates();
        SwitchAnimation();
        lastAttackTime -= Time.deltaTime;
    }

    void SwitchAnimation()
    {
        animator.SetBool("Walk", isWalk);
        animator.SetBool("Chase", isChase);
        animator.SetBool("Follow", isFollow);
        animator.SetBool("Critical", characterStatus.isCritical);
        animator.SetBool("Death", isDead);
    }

    //状态机切换
    void SwitchStates()
    {
        if(isDead)
        {
            enemyStates = EnemyStates.DEAD;
        }else if(isFoundPlayer())
        {
            enemyStates = EnemyStates.CHASE;
        }

        switch(enemyStates)
        {
            case EnemyStates.GUARD:
                guard();
                break;
            case EnemyStates.PATROL:
                patrol();
                break;
            case EnemyStates.CHASE:
                chase();
                break;
            case EnemyStates.DEAD:
                dead();
                break;
        }
    }
    void dead()
    {
        agent.enabled = false;
        if(gameObject.GetComponent<WizardController>())
        {
            var temp = gameObject.GetComponent<WizardController>();
            temp.staticHandBall.SetActive(false);
            temp.enabled = false;
        }
        if(gameObject.GetComponent<BossController>())
        {
            GameManager.Instance.NotifyObserver(true);
        }
    }

    void guard()
    {
        isChase = false;
        if(transform.position != guardPos)
        {
            isWalk = true;
            agent.isStopped = false;
            agent.destination = guardPos;
            

            if(Vector3.SqrMagnitude(guardPos - transform.position) <= agent.stoppingDistance)
            {
                isWalk = false;
                transform.rotation = Quaternion.Lerp(transform.rotation, guardRotation, 0.05f);
            }
                
        }
    }

    void patrol()
    {
        isWalk = true;
        isChase = false;
        agent.speed = speed * 0.5f;
        agent.isStopped = false;

        if(Vector3.Distance(wayPoint, transform.position) <= agent.stoppingDistance)
        {
            isWalk = false;
            agent.isStopped = true;
            if(remainLookAtTime > 0)
                remainLookAtTime -= Time.deltaTime;    
            else
            {
                getNewWayPoint();
                agent.isStopped = false;
                remainLookAtTime = lookAtTime;
            }
                
        }else
        {
            isWalk = true;
            agent.destination = wayPoint;
        }
    }

    void chase()
    {   
        isWalk = false;
        isChase = true;
        agent.speed = speed * 1.5f;
        

        if(!isFoundPlayer())
        {
            isFollow = false;
            if(remainLookAtTime > 0)
            {
                agent.destination = transform.position;
                remainLookAtTime -= Time.deltaTime;
            }else if(isGuard)
            {
                isChase = false;
                enemyStates = EnemyStates.GUARD;
            }else
            {
                isChase = false;
                isWalk = true;
                enemyStates = EnemyStates.PATROL;
            }
        }else
        {
            //if in the range of attack
            if(TargetAttackRange() || TargetSkillRange())
            {
                isFollow = false;
                agent.isStopped = true;
                if(lastAttackTime < 0)
                {
                    Vector3 direction = transform.position - attackTarget.transform.position;
                    Quaternion faceRotation = Quaternion.FromToRotation(transform.forward, direction);
                    transform.rotation = Quaternion.Lerp(transform.rotation, faceRotation, 0.05f);

                    lastAttackTime = characterStatus.attackData.cooldown;
                    characterStatus.isCritical = Random.value < characterStatus.attackData.criticalChance;
                    Attack();
                }
            }else
            {
                agent.isStopped = false;
                isFollow = true;
                agent.destination = attackTarget.transform.position;
            }
        }
    }

    void Attack()
    {
        
        // if(this.CompareTag("Wizard")) transform.LookAt(attackTarget.transform);
        
        if(TargetAttackRange())
        {
            animator.SetTrigger("Attack");
        }
        if(TargetSkillRange())
        {
            animator.SetTrigger("Skill");
        }
    }

    bool TargetAttackRange()
    {
        if(attackTarget != null)
        {
            return Vector3.Distance(attackTarget.transform.position, transform.position) <= characterStatus.attackData.attackRange;
        }else return false;
    }

    bool TargetSkillRange()
    {
        if(attackTarget != null)
        {
            return Vector3.Distance(attackTarget.transform.position, transform.position) <= characterStatus.attackData.skillRange;
        }else return false;
    }

    void getNewWayPoint()
    {
        float randomX = Random.Range(-patrolRange, patrolRange);
        float randomZ = Random.Range(-patrolRange, patrolRange);
        Vector3 randomPoint = new Vector3(guardPos.x + randomX, transform.position.y, guardPos.z+randomZ);
        // wayPoint = randomPoint;
        NavMeshHit hit;
        wayPoint = NavMesh.SamplePosition(randomPoint,out hit, patrolRange,1)? hit.position : transform.position;
    }

    bool isFoundPlayer()
    {
        var colliders = Physics.OverlapSphere(transform.position, sightRadius);
        foreach(var target in colliders)
        {
            if(target.CompareTag("Player"))
            {
                attackTarget = target.gameObject;
                return true;
            }
        }
        attackTarget = null;
        return false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, sightRadius);
        Gizmos.DrawWireSphere(transform.position, patrolRange);
    }

    //动画关键帧触发事件
    void Hit()
    {
        if(attackTarget != null && transform.isFacingTarget(attackTarget.transform))
        {
            var targetStatus = attackTarget.GetComponent<CharacterStatus>();
            targetStatus.takeDamage(characterStatus, targetStatus);
        }
    }

    //玩家死后
    public void EndNotify()
    {
        isFollow = false;
        isChase = false;
        attackTarget = null;
        if(isGuard)
        {
            enemyStates = EnemyStates.GUARD;
        }
        else
        {
            getNewWayPoint();
            enemyStates = EnemyStates.PATROL;
        }
    }
}
