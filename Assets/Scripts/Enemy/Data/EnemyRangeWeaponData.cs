using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu( fileName = "New Weapon Data", menuName = "Enemy Data/Range Weapon Data")]

public class EnemyRangeWeaponData : ScriptableObject
{
    [Header("Weapon Details")]
    public Enemy_RangeWeaponType weaponType;
    public float fireRate = 1f;

    public int minBulletsPerAttack = 1;
    public int maxBulletsPerAttack = 1;

    public float minWeaponCooldown = 1f;
    public float maxWeaponCooldown = 1f;

    [Header("Bullet Details")]
    public float bulletSpeed = 10f;
    public float weaponSpread = .1f;

    public int GetBulletsPerAttack() => Random.Range(minBulletsPerAttack, maxBulletsPerAttack);

    public float GetWeaponCooldown() => Random.Range(minWeaponCooldown, maxWeaponCooldown);

    public Vector3 ApplyWeaponSpread(Vector3 originalDirection)
    {
        float randomizeValue = Random.Range(-weaponSpread, weaponSpread);

        Quaternion spreadRotation = Quaternion.Euler(0, randomizeValue, randomizeValue);

        return spreadRotation * originalDirection;
    }
}
