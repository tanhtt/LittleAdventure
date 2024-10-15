using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    [SerializeField] private Weapon_Data weaponData;

    private void OnTriggerEnter(Collider other)
    {
        other.GetComponent<PlayerWeaponsCtrl>()?.PickupWeapon(weaponData);
    }
}
