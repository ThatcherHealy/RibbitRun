using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayFishSpawnSFX : MonoBehaviour
{
    [SerializeField] bool salmon;
    void Start()
    {
        if(salmon)
            FindFirstObjectByType<SFXManager>().PlaySFX("Salmon Spawn");
        else
            FindFirstObjectByType<SFXManager>().PlaySFX("Arapaima Spawn");
    }
}
