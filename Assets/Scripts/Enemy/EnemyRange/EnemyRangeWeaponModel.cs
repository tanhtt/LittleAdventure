using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyRange_WeaponHoldType { Common, Low, High }

public class EnemyRangeWeaponModel : MonoBehaviour
{
    public Transform gunPoint;

    public Enemy_RangeWeaponType weaponType;
    public EnemyRange_WeaponHoldType weaponHoldType;

    public Transform leftHandTarget;
    public Transform leftHandHint;
}
