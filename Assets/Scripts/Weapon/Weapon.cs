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
    #region Regular mode variables
    public WeaponType weaponType;

    public ShootType shootType;
    public float defaultFireRate;
    public float fireRate = 1; // bullets per second
    public int bulletsPerShot { get; private set; }
    private float lastShootTime;
    #endregion

    #region Burst mode variables
    private bool burstAvailable;
    public bool burstActive;

    private int burstBulletsPerShoot;
    private float burstFireRate;
    public float burstFireDelay { get; private set; }
    #endregion

    [Header("Magazine Details")]
    public int bulletsInMagazine;
    public int magazineCapacity;
    public int totalReserveAmmo;

    #region Generic Info
    public float reloadSpeed { get; private set; }

    public float equipmentSpeed { get; private set; }

    public float gunDistance { get; private set; }

    public float cameraDistance { get; private set; }
    #endregion

    #region Spread variables
    private float baseSpread;
    private float currentSpread;
    private float maxSpread;

    private float spreadIncreaseRate = .15f;

    private float lastSpreadUpdateTime;
    private float spreadCooldown = 1;
    #endregion

    public Weapon_Data weaponData; // Serve as weapon data

    public Weapon(Weapon_Data weaponData)
    {
        this.weaponType = weaponData.weaponType;
        this.shootType = weaponData.shootType;
        this.bulletsPerShot = weaponData.bulletsPerShot;
        this.fireRate = weaponData.fireRate;

        this.burstActive = weaponData.burstActive;
        this.burstAvailable = weaponData.burstAvailable;
        this.burstFireRate = weaponData.burstFireRate;
        this.burstBulletsPerShoot = weaponData.burstBulletsPerShoot;
        this.burstFireDelay = weaponData.burstFireDelay;

        this.bulletsInMagazine = weaponData.bulletsInMagazine;
        this.magazineCapacity = weaponData.magazineCapacity;
        this.totalReserveAmmo = weaponData.totalReserveAmmo;

        this.baseSpread = weaponData.baseSpread;
        this.maxSpread = weaponData.maxSpread;
        this.spreadIncreaseRate = weaponData.spreadIncreaseRate;

        this.reloadSpeed = weaponData.reloadSpeed;
        this.equipmentSpeed = weaponData.equipmentSpeed;
        this.gunDistance = weaponData.gunDistance;
        this.cameraDistance = weaponData.cameraDistance;

        defaultFireRate = fireRate;

        this.weaponData = weaponData;
    }

    #region Burst

    public bool BurstActivated()
    {
        if(weaponType == WeaponType.Shotgun)
        {
            burstFireDelay = 0;
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
