using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayFishSpawnSFX : MonoBehaviour
{
    void Start()
    {
        FindFirstObjectByType<SFXManager>().PlaySFX("Fish Spawn");
    }
}
