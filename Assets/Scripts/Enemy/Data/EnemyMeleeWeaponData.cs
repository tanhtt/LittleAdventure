using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon Data", menuName = "Enemy Data/Melee Weapon Data")]

public class EnemyMeleeWeaponData : ScriptableObject
{
    public List<Enemy_MeleeAttackData> attackList;
    public float turnSpeed;
}
