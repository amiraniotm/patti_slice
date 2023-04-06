using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    // Object pool for every game object that is to be re-used like enemies or items

    // List of prefabs to pool and how many will be pooled 
    [SerializeField] List<GameObject> objectPrefabs = new List<GameObject>();
    [SerializeField] public int objectsToPool = 5;    
    // Runtime list of pooled objects
    public List<GameObject> pooledObjects = new List<GameObject>();

    // Pooling each prefab times the pool limit
    protected void Start()
    {
        foreach (GameObject prefab in objectPrefabs) {
            for (int i = 0; i < objectsToPool; i++) {
                GameObject newObj = Instantiate(prefab);
                newObj.SetActive(false);
                pooledObjects.Add(newObj);
            }
        }
    }
    // Called from spawning scripts to get an object from the pool by its name
    public GameObject GetPooledObject(string objectName)
    {
        for (int i = 0; i <  pooledObjects.Count; i++)
        {
            // Cleaning name for match, as same-name objects will be called ObjectName(Clone) by unity
            string cleanName = pooledObjects[i].name.Replace("(Clone)", "");

            if(cleanName == objectName && !pooledObjects[i].activeInHierarchy) {
                return pooledObjects[i];
            }
        }
        
        return null;
    }
}
