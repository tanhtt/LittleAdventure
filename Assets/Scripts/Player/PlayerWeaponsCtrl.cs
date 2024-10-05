using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponsCtrl : MonoBehaviour
{
    private const float REFERENCE_BULLET_SPEED = 20;
    private Player player;

    [SerializeField] private Weapon currentWeapon;
    private bool weaponReady;

    [Header("Bullet Info")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float bulletSpeed;

    [SerializeField] private Transform weaponHolder;

    [Header("Inventory")]
    [SerializeField] private int maxSlot = 2;
    [SerializeField] private List<Weapon> weaponSlots;


    private void Start()
    {
        player = GetComponent<Player>();
        AssignInputEvents();

        currentWeapon.bulletsInMagazine = currentWeapon.totalReserveAmmo;

        Invoke("EquipAtStart", .1f);
    }

    #region Equip, Drop, Pickup, Ready weapon
    private void EquipAtStart()
    {
        EquipWeapon(0);
    }

    private void EquipWeapon(int i)
    {
        SetWeaponReady(false);

        currentWeapon = weaponSlots[i];

        //player.playerWeaponVisual.SwitchOffWeaponModels();
        player.playerWeaponVisual.PlayWeaponEquipAnimation();
    }

    private void DropWeapon()
    {
        if (HasOnlyOneWeapon()) return;
        weaponSlots.Remove(currentWeapon);

        EquipWeapon(0);
    }

    public void PickupWeapon(Weapon weapon)
    {
        if (weaponSlots.Count >= maxSlot) return;

        weaponSlots.Add(weapon);
        player.playerWeaponVisual.SwitchOnBackupWeaponModel();
    }

    public void SetWeaponReady(bool ready) => weaponReady = ready;

    public bool WeaponReady() => weaponReady;
    #endregion

    private void Shoot()
    {
        if (WeaponReady() == false) return;
        if (currentWeapon.CanShoot() == false) return;

        //GameObject newBullet = Instantiate(bulletPrefab, gunPoint.position, Quaternion.LookRotation(gunPoint.forward));
        GameObject newBullet = ObjectPool.instance.GetBullet();
        newBullet.transform.position = GunPoint().position;
        newBullet.transform.rotation = Quaternion.LookRotation(GunPoint().forward);

        Rigidbody rbNewBullet = newBullet.GetComponent<Rigidbody>();

        rbNewBullet.mass = REFERENCE_BULLET_SPEED / bulletSpeed;
        rbNewBullet.velocity = BulletDirection() * bulletSpeed;



        player.playerWeaponVisual.PlayFireAnimation();
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

    #region Assign Input
    private void AssignInputEvents()
    {
        PlayerInputs controls = player.playerInputs;
        controls.Player.Fire.performed += context => Shoot();

        controls.Player.EquipSlot1.performed += context => EquipWeapon(0);
        controls.Player.EquipSlot2.performed += context => EquipWeapon(1);

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
