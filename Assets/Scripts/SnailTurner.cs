using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SnailTurner : MonoBehaviour
{
    [SerializeField] SnailMovement sm;
    enum Side {left, right};
    [SerializeField]Side side;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 3) 
        {
            if (side == Side.left)
            {
                sm.direction = 1;
            }
            else if (side == Side.right)
            {
                sm.direction = 2;
            }
        }
    }
}