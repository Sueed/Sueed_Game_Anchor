using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CharacterStatus : MonoBehaviour
{
    public event Action<int, int> UpdateHealthBarOnAttack;
    public CharacterData_OS templateData;
    public CharacterData_OS characterData;
    public AttackData_SO attackData;
    public float LastBeCriticalTime;
    void Awake() 
    {
        if(templateData != null)
        {
            characterData = Instantiate(templateData);
        }
    }

    [HideInInspector]
    public bool isCritical;

    #region Read from Data_SO
    public int maxHealth
    {
        get
        {
            if(characterData != null)
                return characterData.maxHealth;
            else return 0;
        }

        set
        {
            characterData.maxHealth = value;
        }
    }

    public int currentHealth
    {
        get
        {
            if(characterData != null)
                return characterData.currentHealth;
            else return 0;
        }

        set
        {
            characterData.currentHealth = value;
        }
    }

    public int baseDefence
    {
        get
        {
            if(characterData != null)
                return characterData.baseDefence;
            else return 0;
        }

        set
        {
            characterData.baseDefence = value;
        }
    }

    public int currentDefence
    {
        get
        {
            if(characterData != null)
                return characterData.currentDefence;
            else return 0;
        }

        set
        {
            characterData.currentDefence = value;
        }
    }
    #endregion

    #region Charachter Combat

    public void takeDamage(CharacterStatus attacker, CharacterStatus defender)
    {   
        
        int damage = Mathf.Max(attacker.currentDamage() - defender.currentDefence, 1);
        // Debug.Log(damage);
        currentHealth = Mathf.Max(currentHealth - damage, 0);
        if(attacker.isCritical)
        {
            defender.GetComponent<Animator>().SetTrigger("Hit");
            defender.LastBeCriticalTime = 0.8f;
        }

        // update UI
        UpdateHealthBarOnAttack?.Invoke(currentHealth, maxHealth);
    }

    private int currentDamage()
    {
        float coreDamage = UnityEngine.Random.Range(attackData.minDamage, attackData.maxDamage);

        if(isCritical) coreDamage *= attackData.criticalMultiplier;

        return (int)coreDamage;
    }

    #endregion
}
