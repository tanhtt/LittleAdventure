using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool instance;

    [SerializeField] private int poolSize = 10;
    [SerializeField] private GameObject pfBullet;

    private Queue<GameObject> bulletPool;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        bulletPool = new Queue<GameObject>();

        InitialPool();
    }

    public GameObject GetBullet()
    {
        if(bulletPool.Count <= 0)
        {
            CreateNewBullet();
        }

        GameObject bulletFromPool = bulletPool.Dequeue();
        bulletFromPool.transform.SetParent(null);
        bulletFromPool.SetActive(true);
        return bulletFromPool;
    }

    public void ReturnBulletToPool(GameObject bullet)
    {
        bullet.transform.SetParent(transform);
        bullet.SetActive(false);
        bulletPool.Enqueue(bullet);
    }

    private void InitialPool()
    {
        for(int i = 0; i < poolSize; i++)
        {
            CreateNewBullet();
        }
    }

    private void CreateNewBullet()
    {
        GameObject newBullet = Instantiate(pfBullet, transform);
        newBullet.SetActive(false);
        bulletPool.Enqueue(newBullet);
    }
}
