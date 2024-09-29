using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    [SerializeField] private Weapon weapon;

    private void OnTriggerEnter(Collider other)
    {
        other.GetComponent<PlayerWeaponsCtrl>()?.PickupWeapon(weapon);
    }
}
