using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponsCtrl : MonoBehaviour
{
    private const float REFERENCE_BULLET_SPEED = 20;
    private Player player;

    [SerializeField] private Weapon currentWeapon;

    [Header("Bullet Info")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float bulletSpeed;
    [SerializeField] private Transform gunPoint;

    [SerializeField] private Transform weaponHolder;

    [Header("Inventory")]
    [SerializeField] private int maxSlot = 2;
    [SerializeField] private List<Weapon> weaponsList;


    private void Start()
    {
        player = GetComponent<Player>();
        AssignInputEvents();

        currentWeapon.ammo = currentWeapon.maxAmmo;
    }

    #region Equip, Drop, Pickup weapon
    private void EquipWeapon(int i)
    {
        currentWeapon = weaponsList[i];
    }

    private void DropWeapon()
    {
        if (weaponsList.Count <= 1) return;
        weaponsList.Remove(currentWeapon);

        EquipWeapon(0);
    }

    public void PickupWeapon(Weapon weapon)
    {
        if(weaponsList.Count >= maxSlot) return;

        weaponsList.Add(weapon);
    }
    #endregion

    private void Shoot()
    {
        if(currentWeapon.CanShoot() == false) return;

        GameObject newBullet = Instantiate(bulletPrefab, gunPoint.position, Quaternion.LookRotation(gunPoint.forward));

        Rigidbody rbNewBullet = newBullet.GetComponent<Rigidbody>();

        rbNewBullet.mass = REFERENCE_BULLET_SPEED / bulletSpeed;
        rbNewBullet.velocity = BulletDirection() * bulletSpeed;

        Destroy(newBullet, 10);

        GetComponentInChildren<Animator>().SetTrigger("Fire");
    }

    public Vector3 BulletDirection()
    {
        Transform aim = player.playerAim.GetAimTransform();
        Vector3 direction = (aim.position - gunPoint.position).normalized;

        //weaponHolder.LookAt(aim);
        //gunPoint.LookAt(aim); TODO: Find better place to put it

        if(player.playerAim.CanAimPrecisly() == false && player.playerAim.Target() == null)
        {
            direction.y = 0;
        }
        return direction;
    }

    public Vector3 GunPoint => gunPoint.position;

    //private void OnDrawGizmos()
    //{
    //    Gizmos.DrawLine(weaponHolder.position, weaponHolder.position + weaponHolder.forward * 25);
    //    Gizmos.color = Color.yellow;

    //    Gizmos.DrawLine(gunPoint.position, gunPoint.position + BulletDirection() * 25);
    //}

    #region Assign Input
    private void AssignInputEvents()
    {
        PlayerInputs controls = player.playerInputs;
        controls.Player.Fire.performed += context => Shoot();

        controls.Player.EquipSlot1.performed += context => EquipWeapon(0);
        controls.Player.EquipSlot2.performed += context => EquipWeapon(1);

        controls.Player.DropCurrentWeapon.performed += context => DropWeapon();
    }
    #endregion
}
