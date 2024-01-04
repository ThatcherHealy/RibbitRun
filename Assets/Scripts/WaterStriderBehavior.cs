using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterStriderBehavior : MonoBehaviour
{
    public Rigidbody2D rb;
    public Transform sprite;
    bool biasLeft;
    void Start()
    {
         StartCoroutine(JumpDelay());

        int direction = Random.Range(1, 3);
        if (direction == 1)
            biasLeft = true;
        else
            biasLeft = false;
    }
    private void Update()
    {
    }

    private void RandomJump()
    {
        int direction = Random.Range(1, 101);
        int power = Random.Range(8, 13);

        rb.velocity = Vector2.zero;
        if (biasLeft)
        {
            if (direction <= 40)
            {
                sprite.transform.eulerAngles = new Vector3(0, 180, 0);
                rb.AddForce(Vector2.right * power, ForceMode2D.Impulse);
            }
            else
            {
                sprite.transform.eulerAngles = new Vector3(0, 0, 0);
                rb.AddForce(Vector2.left * power, ForceMode2D.Impulse);
                biasLeft = false;
            }
        }
        else
        {
            if (direction <= 60)
            {
                sprite.transform.eulerAngles = new Vector3(0, 180, 0);
                rb.AddForce(Vector2.right * power, ForceMode2D.Impulse);
                biasLeft = true;
            }
            else
            {
                sprite.transform.eulerAngles = new Vector3(0, 0, 0);
                rb.AddForce(Vector2.left * power, ForceMode2D.Impulse);
            }
            biasLeft = true;
        }
        StartCoroutine(JumpDelay());
    }
    IEnumerator JumpDelay()
    {
        float delay = Random.Range(1.5f, 4);
        yield return new WaitForSeconds(delay);
        
        RandomJump();
    }
}