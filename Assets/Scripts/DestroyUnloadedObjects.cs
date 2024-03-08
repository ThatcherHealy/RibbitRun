using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyUnloadedObjects : MonoBehaviour
{
    [SerializeField] PlayerController pc;
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!pc.dead) 
        {
            if (collision.gameObject.transform.parent != null)
            {
                if (collision.transform.parent.name != "SpringPref(Clone)")
                    Destroy(collision.gameObject.transform.parent.gameObject);
            }
            if (collision.gameObject != null)
            {
                if (collision.gameObject.transform.parent != null)
                {
                    if (collision.transform.parent.name != "SpringPref(Clone)")
                        Destroy(collision.gameObject);
                }
                else
                    Destroy(collision.gameObject);
            }
        }
    }
}