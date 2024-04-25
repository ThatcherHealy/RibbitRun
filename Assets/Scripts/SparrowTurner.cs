using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SparrowTurner : MonoBehaviour
{
    public string turnDirection;
    public bool active;
    bool turned;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 14 || collision.gameObject.name == "CypressTree" && active)
        {
            if (!turned)
            {
                if (transform.parent.position.x < collision.transform.position.x) //Approaching from the left
                {
                    turnDirection = "Right";
                }
                if (transform.parent.position.x > collision.transform.position.x) //Approaching from the right
                {
                    turnDirection = "Left";
                }
                turned = true;
            }
        }
    }
}
