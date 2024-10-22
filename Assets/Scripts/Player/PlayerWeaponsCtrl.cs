using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponsCtrl : MonoBehaviour
{
    private const float REFERENCE_BULLET_SPEED = 20;
    private Player player;

    [SerializeField] private Weapon currentWeapon;
    [SerializeField] private Weapon_Data defaultWeaponData;
    private bool weaponReady;
    private bool isShooting = false;

    [Header("Bullet Info")]
    [SerializeField] private float bulletImpactForce = 100;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float bulletSpeed;

    [SerializeField] private Transform weaponHolder;

    [Header("Inventory")]
    [SerializeField] private int maxSlot = 2;
    [SerializeField] private List<Weapon> weaponSlots;

    [SerializeField] private GameObject weaponPickupPrefab;


    private void Start()
    {
        player = GetComponent<Player>();
        AssignInputEvents();

        currentWeapon.bulletsInMagazine = currentWeapon.totalReserveAmmo;

        Invoke("EquipsStartingWeapon", .1f);
    }

    private void Update()
    {
        if(isShooting)
        {
            Shoot();
        }
    }

    #region Equip, Drop, Pickup, Ready weapon
    private void EquipsStartingWeapon()
    {
        weaponSlots[0] = new Weapon(defaultWeaponData);
        EquipWeapon(0);
    }

    private void EquipWeapon(int i)
    {
        if (i > weaponSlots.Count) return;

        SetWeaponReady(false);

        currentWeapon = weaponSlots[i];

        //player.playerWeaponVisual.SwitchOffWeaponModels();
        player.playerWeaponVisual.PlayWeaponEquipAnimation();
        CameraManager.instance.SetCameraDistance(currentWeapon.cameraDistance);
    }

    private void DropWeapon()
    {
        if (HasOnlyOneWeapon()) return;
        CreateWeaponOnGround();

        weaponSlots.Remove(currentWeapon);
        EquipWeapon(0);
    }

    private void CreateWeaponOnGround()
    {
        GameObject dropWeapon = ObjectPool.instance.GetObject(weaponPickupPrefab);
        dropWeapon.GetComponent<Pickup_Weapon>()?.UpdatePickupWeapon(currentWeapon, transform);
    }

    public void PickupWeapon(Weapon newWeapon)
    {
        // Check if weapon is already in slots, add bullet to ammo
        if(WeaponInSlots(newWeapon.weaponType) != null)
        {
            WeaponInSlots(newWeapon.weaponType).totalReserveAmmo += newWeapon.bulletsInMagazine;
            return;
        }

        // Check if weapon slot is full, switch current weapon with pickup weapon
        if (weaponSlots.Count >= maxSlot && newWeapon.weaponType != currentWeapon.weaponType)
        {
            int weaponIndex = weaponSlots.IndexOf(currentWeapon);

            CreateWeaponOnGround();
            player.playerWeaponVisual.SwitchOffBackupWeaponModels();
            weaponSlots[weaponIndex] = newWeapon;
            EquipWeapon(weaponIndex);

            return;
        }

        weaponSlots.Add(newWeapon);
        player.playerWeaponVisual.SwitchOnBackupWeaponModel();
    }

    public void SetWeaponReady(bool ready) => weaponReady = ready;

    public bool WeaponReady() => weaponReady;
    #endregion

    private IEnumerator BurstFire()
    {
        SetWeaponReady(false);

        for(int i = 1; i <= currentWeapon.bulletsPerShot; i++)
        {
            FireSingleBullet();

            yield return new WaitForSeconds(currentWeapon.burstFireDelay);

            if(i >= currentWeapon.bulletsPerShot) SetWeaponReady(true);
        }
    }

    private void Shoot()
    {
        if (WeaponReady() == false) return;
        if (currentWeapon.CanShoot() == false) return;

        player.playerWeaponVisual.PlayFireAnimation();

        if (currentWeapon.shootType == ShootType.Single)
        {
            isShooting = false;
        }

        if (currentWeapon.BurstActivated())
        {
            StartCoroutine(BurstFire());
            return;
        }

        FireSingleBullet();
        CanEnemyDodge();
    }

    private void FireSingleBullet()
    {
        currentWeapon.bulletsInMagazine--;

        GameObject newBullet = ObjectPool.instance.GetObject(bulletPrefab);

        newBullet.transform.position = GunPoint().position;
        newBullet.transform.rotation = Quaternion.LookRotation(GunPoint().forward);

        newBullet.GetComponent<Bullet>().BulletSetup(currentWeapon.gunDistance, bulletImpactForce);

        Rigidbody rbNewBullet = newBullet.GetComponent<Rigidbody>();
        Vector3 bulletDirection = currentWeapon.ApplySpread(BulletDirection());

        rbNewBullet.mass = REFERENCE_BULLET_SPEED / bulletSpeed;
        rbNewBullet.velocity = bulletDirection * bulletSpeed;
    }

    private void Reload()
    {
        SetWeaponReady(false);
        player.playerWeaponVisual.PlayReloadAnimation();
    }

    public Vector3 BulletDirection()
    {
        Transform aim = player.playerAim.GetAimTransform();
        Vector3 direction = (aim.position - GunPoint().position).normalized;

        if (player.playerAim.CanAimPrecisly() == false && player.playerAim.Target() == null)
        {
            direction.y = 0;
        }
        return direction;
    }

    public Weapon GetBackupWeapon()
    {
        foreach(Weapon weapon in weaponSlots)
        {
            if(weapon != currentWeapon)
            {
                return weapon;
            }
        }
        return null;
    }

    public Transform GunPoint() => player.playerWeaponVisual.GetCurrentWeaponModel().gunPoint;
    public Weapon CurrentWeapon() => currentWeapon;

    public bool HasOnlyOneWeapon()
    {
        return weaponSlots.Count <= 1;
    }

    public Weapon WeaponInSlots(WeaponType weaponType)
    {
        foreach(Weapon weapon in weaponSlots)
        {
            if(weaponType == weapon.weaponType)
            {
                return weapon;
            }
        }
        return null;
    }

    private void CanEnemyDodge()
    {
        Vector3 rayOrigin = GunPoint().position;
        Vector3 rayDir = BulletDirection();
        if(Physics.Raycast(rayOrigin, rayDir, out RaycastHit hit, Mathf.Infinity))
        {
            EnemyMelee enemyMelee = hit.collider.gameObject.GetComponentInParent<EnemyMelee>();
            if(enemyMelee != null)
            {
                enemyMelee.ActiveDodgeRoll();
            }
        }
    }

    #region Assign Input
    private void AssignInputEvents()
    {
        PlayerInputs controls = player.playerInputs;

        controls.Player.Fire.performed += context => isShooting = true;
        controls.Player.Fire.canceled += context => isShooting = false;

        controls.Player.EquipSlot1.performed += context => EquipWeapon(0);
        controls.Player.EquipSlot2.performed += context => EquipWeapon(1);
        controls.Player.EquipSlot3.performed += context => EquipWeapon(2);
        controls.Player.EquipSlot4.performed += context => EquipWeapon(3);
        controls.Player.EquipSlot5.performed += context => EquipWeapon(4);

        controls.Player.ToggleWeaponMode.performed += context => currentWeapon.ToggleBurst();

        controls.Player.DropCurrentWeapon.performed += context => DropWeapon();

        controls.Player.Reload.performed += context =>
        {
            if (currentWeapon.CanReload() && WeaponReady())
            {
                Reload();
            }
        };
    }
    #endregion
}
