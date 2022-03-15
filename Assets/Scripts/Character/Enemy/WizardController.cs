using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WizardController : EnemyController
{
    //利用NavMeshAgent进行击退效果
    /*[Header("Skill")]
    public float kickForce = 10;
    public void KickOff()
    {
        if(attackTarget != null && transform.isFacingTarget(attackTarget.transform))
        {
            transform.LookAt(attackTarget.transform);
            var targetStatus = attackTarget.GetComponent<CharacterStatus>();

            Vector3 direction = (attackTarget.transform.position - transform.position).normalized;
            var agentto = attackTarget.GetComponent<NavMeshAgent>();
            agentto.isStopped = true;
            agentto.velocity = direction * kickForce;

            targetStatus.takeDamage(characterStatus, targetStatus);
        }
    }*/
    [Header("BaseFireBall")]
    public GameObject staticHandBall;

    [Header("FlyItemSettings")]
    public GameObject ballPrefab;
    public Transform handPos;

    void Start() 
    {
        this.staticHandBall.SetActive(true);
    }

    public void ThrowBall()
    {
        if(attackTarget != null && staticHandBall != null)
        {
            transform.LookAt(attackTarget.transform);
            var ball = Instantiate(ballPrefab, handPos.position, Quaternion.identity);
            ball.GetComponent<FlyItem>().target = attackTarget;
            ball.GetComponent<FlyItem>().characterStatus = characterStatus;
        }
    }
}
