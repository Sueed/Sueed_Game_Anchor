using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Data", menuName = "CharacterData")]
public class CharacterData_OS : ScriptableObject
{
    [Header("State Info")]
    public int maxHealth;
    public int currentHealth;
    public int baseDefence;
    public int currentDefence;
}
