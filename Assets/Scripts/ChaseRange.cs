using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class ChaseRange : MonoBehaviour
{
    [SerializeField] PredatorVision pv;
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision != null && collision.gameObject.tag != null)
        {
            if (collision.CompareTag("Player") && pv.huntingMode)
            {
                pv.huntingMode = false;
                pv.frog = null;
                pv.rangeLeft = true;
            }
        }
    }
}
