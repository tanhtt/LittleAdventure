using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Weapon Data", menuName = "Weapon System/Weapon Data")]
public class Weapon_Data : ScriptableObject
{
    public string weaponName;
    public WeaponType weaponType;
    public int bulletsPerShot = 1;
    public float fireRate;

    [Header("Burst Shot")]
    public bool burstAvailable;
    public bool burstActive;

    public int burstBulletsPerShoot;
    public float burstFireRate;
    public float burstFireDelay = .1f;

    [Header("Magazine Details")]
    public int bulletsInMagazine;
    public int magazineCapacity;
    public int totalReserveAmmo;

    [Header("Spread")]
    public float baseSpread;
    public float maxSpread;
    public float spreadIncreaseRate = .15f;

    [Header("Specifics")]
    public ShootType shootType;
    [Range(1f, 3f)]
    public float reloadSpeed = 1;

    [Range(1f, 3f)]
    public float equipmentSpeed = 1;

    [Range(4f, 12f)]
    public float gunDistance = 4f;

    [Range(4f, 8f)]
    public float cameraDistance = 6f;
}
