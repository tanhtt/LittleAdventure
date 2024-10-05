using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackupWeaponModel : MonoBehaviour
{
    [SerializeField] private WeaponType weaponType;
    public WeaponType WeaponType => weaponType;
}
