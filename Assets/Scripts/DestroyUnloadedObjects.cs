using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyUnloadedObjects : MonoBehaviour
{
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.transform.parent != null)
        {
            collision.gameObject.transform.parent.gameObject.SetActive(false);
        }
        if (collision.gameObject != null)
        {
            collision.gameObject.SetActive(false);
        }
    }
}