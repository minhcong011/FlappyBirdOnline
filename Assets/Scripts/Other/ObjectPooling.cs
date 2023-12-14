using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooling : SingletonBase<ObjectPooling>
{
    [SerializeField] private GameObject holder;
    [SerializeField] private GameObject test;
    [SerializeField] private List<GameObject> listPool = new();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    { 
    }
    public GameObject CreatedObj(GameObject obj)
    {
        if(CheckObjAvailableInPool(obj) == null)
        {
            GameObject newObj = Instantiate(obj);
            newObj.transform.SetParent(holder.transform, false);
            listPool.Add(newObj);
            return newObj;
        }
        return null;
    }
    private GameObject CheckObjAvailableInPool(GameObject objToCheck)
    {
        foreach (GameObject obj in listPool)
        {
            if ((objToCheck.name + "(Clone)").Equals(obj.name) && !obj.activeInHierarchy)
            {
                obj.SetActive(true);
                return obj;
            }
        }
        return null;
    }
    public void DestroyObj(GameObject obj)
    {
        obj.SetActive(false);
    }
    public void SpawnTEst()
    {
        CreatedObj(test);
    }
}
