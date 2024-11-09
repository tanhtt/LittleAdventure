using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : Bullet
{
    protected override void OnCollisionEnter(Collision collision)
    {
        CreateImpactFX(collision);
        ObjectPool.instance.ReturnObject(this.gameObject);

        Player player = collision.gameObject.GetComponentInParent<Player>();

        if (player != null)
        {
            Debug.Log("Shoot Player");
        }
    }
}
