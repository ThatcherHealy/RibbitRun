using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EatenLowering : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] float lowerDelay;
    bool activated;
    void Update()
    {
        if(gameObject.activeSelf && !activated) 
        {
            if(FindFirstObjectByType<DeathScript>().dontRespawnPressed) 
            {
                lowerDelay = 1f;
            }
            StartCoroutine(WaitThenLower());
            activated = true;
        }
    }
    IEnumerator WaitThenLower() 
    {
        yield return new WaitForSeconds(lowerDelay);
        animator.enabled = true;
    }
}
