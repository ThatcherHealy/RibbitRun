using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChanceSpawner : MonoBehaviour
{
    [SerializeField] GameObject spawnedObject;

    //There is a 1/probability chance that the object is spawned 
    [SerializeField] int probabilty;
    [SerializeField] bool guaranteed;

    [SerializeField] bool child;
    [SerializeField] Transform parent;
    private void Start()
    {
        if (guaranteed || Random.Range(1,probabilty + 1) == 1)
        {
            if(!child) 
            {
                Instantiate(spawnedObject, transform.position, Quaternion.identity);
            }
            else
            {
                Instantiate(spawnedObject, transform.position, Quaternion.identity, parent);
            }
        }
    }

}
