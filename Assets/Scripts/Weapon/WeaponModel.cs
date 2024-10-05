using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HoldType
{
    CommonHold = 1,
    LowHold,
    HighHold,
}

public class WeaponModel : MonoBehaviour
{
    [SerializeField] public WeaponType weaponType;
    [SerializeField] public EquipType equipAnimationType;
    [SerializeField] public HoldType holdType;

    [SerializeField] public Transform gunPoint;
    [SerializeField] public Transform holdPoint;
}
