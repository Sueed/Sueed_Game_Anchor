using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyItem : MonoBehaviour
{
    public enum FlyItemState{HitPlayer, HitEnemy, HitNothing};
    private Rigidbody rb;
    private FlyItemState flyItemState;

    [Header("Basic Settings")]
    public float force;
    public CharacterStatus characterStatus;
    public GameObject target;
    private Vector3 direction;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        flyItemState = FlyItemState.HitPlayer;
        FlyToTarget();
    }

    public void FlyToTarget()
    {
        if(target == null) target = FindObjectOfType<CharacterController>().gameObject;
        direction = (target.transform.position - transform.position + Vector3.up).normalized;
        rb.AddForce(direction * force, ForceMode.Impulse);
    }

   private void OnCollisionEnter(Collision other) 
   {
       switch(flyItemState)
       {
            case FlyItemState.HitPlayer:
                if(other.gameObject.CompareTag("Player"))
                {
                    var tempObject = other.gameObject;
                    var temp = tempObject.GetComponent<CharacterStatus>();
                    temp.LastBeCriticalTime = 0.8f;
                    temp.takeDamage(characterStatus, temp);
                    Destroy(gameObject);
                }
                else flyItemState = FlyItemState.HitNothing;
                break;
            case FlyItemState.HitEnemy:
                // if(other.gameObject.GetComponent<WizardController>())
                // {
                //     var temp = other.gameObject.GetComponent<CharacterStatus>();
                //     temp.takeDamage(characterStatus, temp);
                //     Destroy(gameObject);
                // }
                break;
            case FlyItemState.HitNothing:
                Destroy(gameObject);
                break;
       }
   }
}
