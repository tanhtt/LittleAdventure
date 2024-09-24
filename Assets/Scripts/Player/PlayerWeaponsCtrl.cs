using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponsCtrl : MonoBehaviour
{
    private Player player;

    [Header("Bullet Info")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float bulletSpeed;
    [SerializeField] private Transform gunPoint;

    [SerializeField] private Transform weaponHolder;

    private void Start()
    {
        player = GetComponent<Player>();

        player.playerInputs.Player.Fire.performed += context => Shoot();
    }

    private void Shoot()
    {
        GameObject newBullet = Instantiate(bulletPrefab, gunPoint.position, Quaternion.LookRotation(gunPoint.forward));

        newBullet.GetComponent<Rigidbody>().velocity = BulletDirection() * bulletSpeed;

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
}
