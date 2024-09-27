using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private GameObject bulletImpactFXPrefab;
    private Rigidbody rb => GetComponent<Rigidbody>();

    private void OnCollisionEnter(Collision collision)
    {
        //rb.constraints = RigidbodyConstraints.FreezeAll;

        CreateImpactFX(collision);

        Destroy(this.gameObject);
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
