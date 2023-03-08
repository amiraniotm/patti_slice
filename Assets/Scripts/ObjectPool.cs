using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    [SerializeField] List<GameObject> objectPrefabs = new List<GameObject>();
    [SerializeField] public int objectsToPool = 5;    
    
    public List<GameObject> pooledObjects = new List<GameObject>();

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

    public GameObject GetPooledObject(string objectName)
    {
        for (int i = 0; i <  pooledObjects.Count; i++)
        {
            string cleanName = pooledObjects[i].name.Replace("(Clone)", "");

            if(cleanName == objectName && !pooledObjects[i].activeInHierarchy) {
                return pooledObjects[i];
            }
        }
        
        return null;
    }
}
