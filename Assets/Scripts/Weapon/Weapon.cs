using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponType
{
    Pistol,
    Revolver,
    Riffle,
    Shotgun,
    Sniper,
}

public enum ShootType
{
    Single,
    Auto,
}

[System.Serializable]
public class Weapon
{
    public WeaponType weaponType;

    [Header("Shooting Specifics")]
    public ShootType shootType;
    public float defaultFireRate;
    public float fireRate = 1; // bullets per second
    public int bulletsPerShot;
    private float lastShootTime;

    [Header("Burst")]
    public bool burstAvailable;
    public bool burstActive;

    public int burstBulletsPerShoot;
    public float burstFireRate;
    public float bulletDelay;

    [Header("Magazine Retails")]
    public int bulletsInMagazine;
    public int magazineCapacity;
    public int totalReserveAmmo;

    [Range(1f, 3f)]
    public float reloadSpeed = 1;

    [Range(1f, 3f)]
    public float equipmentSpeed = 1;

    [Range(1f, 12f)]
    public float gunDistance = 4f;

    [Range(3f, 8f)]
    public float cameraDistance = 6f;

    [Header("Spread")]
    public float baseSpread;
    private float currentSpread;
    public float maxSpread;

    public float spreadIncreaseRate = .15f;

    private float lastSpreadUpdateTime;
    private float spreadCooldown = 1;

    public Weapon(WeaponType weaponType)
    {
        defaultFireRate = fireRate;
        this.weaponType = weaponType;
    }

    #region Burst

    public bool BurstActivated()
    {
        if(weaponType == WeaponType.Shotgun)
        {
            burstFireRate = 0;
            return true;
        }

        return burstActive;
    }

    public void ToggleBurst()
    {
        if(burstAvailable == false) return;

        burstActive = !burstActive;

        if (burstActive)
        {
            bulletsPerShot = burstBulletsPerShoot;
            fireRate = burstFireRate;
        }
        else
        {
            bulletsPerShot = 1;
            fireRate = defaultFireRate;
        }
    }

    #endregion


    #region Spread
    public Vector3 ApplySpread(Vector3 originalDirection)
    {
        UpdateSpread();
        float randomizeValue = Random.Range(-currentSpread, currentSpread);

        Quaternion spreadRotation = Quaternion.Euler(0, randomizeValue, randomizeValue);

        return spreadRotation * originalDirection;
    }

    private void UpdateSpread()
    {
        if(Time.time > lastSpreadUpdateTime + spreadCooldown)
        {
            currentSpread = baseSpread;
        }
        else
        {
            IncreaseSpread();
        }
        lastSpreadUpdateTime = Time.time;
    }

    private void IncreaseSpread()
    {
        currentSpread = Mathf.Clamp(currentSpread + spreadIncreaseRate, baseSpread, maxSpread);
    }

    #endregion

    public bool CanShoot() => (HaveEnoughBullet() && ReadyToFire());

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
