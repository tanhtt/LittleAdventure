using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private float impactForce;

    [SerializeField] private GameObject bulletImpactFXPrefab;
    private Rigidbody rb;
    private SphereCollider col;
    private MeshRenderer meshRenderer;
    private TrailRenderer trailRenderer;

    private Vector3 startPosition;
    private float flyDistance;
    private bool bulletDisabled;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<SphereCollider>();
        meshRenderer = GetComponent<MeshRenderer>();
        trailRenderer = GetComponent<TrailRenderer>();
    }

    protected virtual void Update()
    {
        FadeTrailIfNeeded();
        DisableBulletIfNeeded();
        ReturnToPoolIfNeeded();
    }

    protected void ReturnToPoolIfNeeded()
    {
        if (trailRenderer.time < 0)
        {
            ObjectPool.instance.ReturnObject(this.gameObject);
        }
    }

    private void DisableBulletIfNeeded()
    {
        if (Vector3.Distance(startPosition, transform.position) > flyDistance && !bulletDisabled)
        {
            col.enabled = false;
            meshRenderer.enabled = false;
            bulletDisabled = true;
        }
    }

    private void FadeTrailIfNeeded()
    {
        if (Vector3.Distance(startPosition, transform.position) > flyDistance - 1.5f)
        {
            trailRenderer.time -= 2f * Time.deltaTime; //magic number for testing
        }
    }

    public void BulletSetup(float flyDistance = 100, float impactForce = 100)
    {
        this.impactForce = impactForce;
        bulletDisabled = false;
        meshRenderer.enabled = true;
        col.enabled = true;

        trailRenderer.time = .25f;
        this.startPosition = transform.position;
        this.flyDistance = flyDistance + 1;
    }

    protected virtual void OnCollisionEnter(Collision collision)
    {
        CreateImpactFX(collision);
        ObjectPool.instance.ReturnObject(this.gameObject);

        Enemy enemy = collision.gameObject.GetComponentInParent<Enemy>();
        EnemyShield enemyShield = collision.gameObject.GetComponent<EnemyShield>();

        if(enemyShield != null)
        {
            enemyShield.ReduceDurability();
            return;
        }

        if (enemy != null)
        {
            Vector3 force = rb.velocity.normalized * impactForce;
            Rigidbody hitRb = collision.collider.attachedRigidbody;
            enemy.GetHit();
            enemy.DeathImpact(force, collision.contacts[0].point, hitRb);
        }
    }

    protected void CreateImpactFX(Collision collision)
    {
        if (collision.contacts.Length > 0)
        {
            ContactPoint contact = collision.contacts[0];
            GameObject newBulletFX = ObjectPool.instance.GetObject(bulletImpactFXPrefab);
            newBulletFX.transform.position = contact.point;
            newBulletFX.transform.rotation = Quaternion.LookRotation(contact.normal);
            
            ObjectPool.instance.ReturnObject(newBulletFX, 1f);
        }
    }
}
