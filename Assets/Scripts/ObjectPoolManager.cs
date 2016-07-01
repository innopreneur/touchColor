using System.Collections;
using UnityEngine;

public class ObjectPoolManager : MonoBehaviour
{

    //is it same type of gameobject?
    [SerializeField]
    private bool isSameObject = true;

    //pool should have atleast one prefab object
    [SerializeField]
    private GameObject[] prefabs = new GameObject[1];

    //create more objects if no objects available
    [SerializeField]
    private bool canCreateMore = true;

    private GameObject[] pool;
    void Start()
    {

        //if its same type of gameobject and only one prefab exist in pool
        if (isSameObject && prefabs.Length == 1)
        {
            //instantiate gameobject
            GameObject obj = (GameObject)Instantiate(prefabs[0]);
            pool[0] = obj;
        }
        //if many prefabs exist in pool, then instantiate each of them
        //and put instantiated gameobjects back at same positions
        else if (!isSameObject && prefabs.Length >= 1)
        {
            for (int i = 0; i < prefabs.Length; i++)
            {
                GameObject obj = (GameObject)Instantiate(prefabs[i]);
                obj.SetActive(false);
                pool[i] = obj;
            }
        }
    }


    void Update()
    {

    }

    //return an instantiated gameobject based on name
    GameObject GetObjectFromPool(string objectName)
    {
        //if gameobject name is provided
        if (objectName != null)
        {
            for (int i = 0; i < pool.Length; i++)
            {
                if (pool[i].name.Equals(objectName) && !pool[i].activeInHierarchy)
                {
                    return pool[i];
                }
            }
        }
        //if no name is provided, return first available object
        else
        {
            for (int i = 0; i < pool.Length; i++)
            {
                if (!pool[i].activeInHierarchy)
                    return pool[i];
            }
        }

        if (canCreateMore && prefabs.Length >= 1)
        {
            return (GameObject)Instantiate(prefabs[0]);
        }

        return null;
    }
}
