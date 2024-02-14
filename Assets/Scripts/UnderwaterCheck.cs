using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnderwaterCheck : MonoBehaviour
{
    public bool underwater;
    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Water"))
        {
            underwater = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Water"))
        {
            underwater = false;
        }
    }
}
