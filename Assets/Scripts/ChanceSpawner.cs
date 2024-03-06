using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChanceSpawner : MonoBehaviour
{
    [SerializeField] GameObject spawnedObject;

    //There is a 1/probability chance that the object is spawned 
    [SerializeField] int probabilty;
    [SerializeField] bool guaranteed;
    private void Start()
    {
        if (guaranteed || Random.Range(1,probabilty + 1) == 1)
        {
            Instantiate(spawnedObject, transform.position, Quaternion.identity);
        }
    }

}
