using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAxe : MonoBehaviour
{
    private Rigidbody rb;
    [SerializeField] private Transform axeVisual;
    [SerializeField] private Transform impackFx;
    private Transform player;
    private float flySpeed;
    private float rotationSpeed = 1600;

    private Vector3 direction;

    private float timer;

    public void SetUpAxe(Transform player, float flySpeed, float timer)
    {
        this.player = player;
        this.flySpeed = flySpeed;
        this.timer = timer;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        axeVisual.Rotate(Vector3.right * rotationSpeed * Time.deltaTime);
        timer -= Time.deltaTime;

        if(timer > 0)
        {
            direction = player.position + Vector3.up - transform.position;
        }

        rb.velocity = direction.normalized * flySpeed;

        transform.forward = rb.velocity;
    }

    private void OnTriggerEnter(Collider other)
    {
        Bullet bullet = other.GetComponent<Bullet>();
        Player player = other.GetComponent<Player>();

        if(bullet != null || player != null)
        {
            GameObject newFx = ObjectPool.instance.GetObject(impackFx.gameObject);
            newFx.transform.position = transform.position;

            ObjectPool.instance.ReturnObject(gameObject);
            ObjectPool.instance.ReturnObject(newFx, 1f);
        }
    }
}
