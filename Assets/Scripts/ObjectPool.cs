using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool instance;

    [SerializeField] private int poolSize = 10;

    private Dictionary<GameObject, Queue<GameObject>> poolDictionary = new Dictionary<GameObject, Queue<GameObject>>();

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

    public GameObject GetObject(GameObject prefab)
    {
        if(poolDictionary.ContainsKey(prefab) == false)
        {
            InitializeNewPool(prefab);
        }

        if (poolDictionary[prefab].Count == 0)
        {
            // If all objects in this type of prefab are in use, create a new one
            CreateNewGameObject(prefab);
        }

        GameObject objectToGet = poolDictionary[prefab].Dequeue();
        objectToGet.transform.SetParent(null);
        objectToGet.SetActive(true);
        return objectToGet;
    }

    private IEnumerator DelayReturn(float delay, GameObject objectToReturn)
    {
        yield return new WaitForSeconds(delay);
        ReturnObjectToPool(objectToReturn);
    }

    public void ReturnObject(GameObject objectToReturn, float delay = 0.001f) => StartCoroutine(DelayReturn(delay, objectToReturn));

    private void ReturnObjectToPool(GameObject objectToReturn)
    {
        GameObject originalPrefab = objectToReturn.GetComponent<PooledObject>().originalPrefab;

        objectToReturn.SetActive(false);
        objectToReturn.transform.SetParent(transform);

        poolDictionary[originalPrefab].Enqueue(objectToReturn);
    }

    private void InitializeNewPool(GameObject prefab)
    {
        poolDictionary[prefab] = new Queue<GameObject>();

        for(int i = 0; i < poolSize; i++)
        {
            CreateNewGameObject(prefab);
        }
    }

    private void CreateNewGameObject(GameObject prefab)
    {
        GameObject newObject = Instantiate(prefab, transform);
        newObject.AddComponent<PooledObject>().originalPrefab = prefab;

        newObject.SetActive(false);
        poolDictionary[prefab].Enqueue(newObject);
    }
}
