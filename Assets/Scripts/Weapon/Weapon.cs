using System;
using UnityEngine;

public enum WeaponType
{
    Pistol,
    Revolver,
    Riffle,
    Shotgun,
    Sniper,
}

[System.Serializable]
public class Weapon
{
    public WeaponType weaponType;
    public int bulletsInMagazine;
    public int magazineCapacity;
    public int totalReserveAmmo;

    [Range(1f, 3f)]
    [SerializeField] public float reloadSpeed = 1;

    [Range(1f, 3f)]
    [SerializeField] public float equipmentSpeed = 1;

    [Space]
    [SerializeField] public float fireRate = 1; // bullets per second
    private float lastShootTime;

    public bool CanShoot()
    {
        if(this.HaveEnoughBullet() && ReadyToFire())
        {
            bulletsInMagazine--;
            return true;
        }
        return false;
    }

    public bool ReadyToFire()
    {
        // 30 > 25 + 1/1 => true
        if(Time.time > lastShootTime + 1 / fireRate)
        {
            lastShootTime = Time.time;
            return true;
        }
        return false;
    }



    #region Reload
    private bool HaveEnoughBullet() => bulletsInMagazine > 0;

    public bool CanReload()
    {
        if (bulletsInMagazine == magazineCapacity) return false;

        if(this.totalReserveAmmo > 0)
        {
            return true;
        }
        return false;
    }

    public void RefillBullets()
    {

        int bulletToReload = magazineCapacity;

        if(bulletToReload > totalReserveAmmo)
        {
            bulletToReload = totalReserveAmmo;
        }

        totalReserveAmmo -= bulletToReload;
        bulletsInMagazine = bulletToReload;

        if(totalReserveAmmo < 0)
        {
            totalReserveAmmo = 0;
        }
    }
    #endregion
}
