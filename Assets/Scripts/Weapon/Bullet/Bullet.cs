using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private GameObject bulletImpactFXPrefab;
    private Rigidbody rb;
    private SphereCollider col;
    private MeshRenderer meshRenderer;
    private TrailRenderer trailRenderer;

    private Vector3 startPosition;
    private float flyDistance;
    private bool bulletDisabled;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<SphereCollider>();
        meshRenderer = GetComponent<MeshRenderer>();
        trailRenderer = GetComponent<TrailRenderer>();
    }

    private void Update()
    {
        FadeTrailIfNeeded();
        DisableBulletIfNeeded();
        ReturnToPoolIfNeeded();
    }

    private void ReturnToPoolIfNeeded()
    {
        if (trailRenderer.time < 0)
        {
            ObjectPool.instance.ReturnBulletToPool(this.gameObject);
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

    public void BulletSetup(float flyDistance)
    {
        bulletDisabled = false;
        meshRenderer.enabled = true;
        col.enabled = true;

        trailRenderer.time = .25f;
        this.startPosition = transform.position;
        this.flyDistance = flyDistance + 1;
    }

    private void OnCollisionEnter(Collision collision)
    {
        //rb.constraints = RigidbodyConstraints.FreezeAll;

        CreateImpactFX(collision);

        //Destroy(this.gameObject);
        ObjectPool.instance.ReturnBulletToPool(this.gameObject);
    }

    private void CreateImpactFX(Collision collision)
    {
        if (collision.contacts.Length > 0)
        {
            ContactPoint contact = collision.contacts[0];
            GameObject newBulletFXPrefab = Instantiate(bulletImpactFXPrefab, contact.point, Quaternion.LookRotation(contact.normal));
            Destroy(newBulletFXPrefab, 1f);
        }
    }
}
