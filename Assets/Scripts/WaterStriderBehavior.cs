using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterStriderBehavior : MonoBehaviour
{
    public Rigidbody2D rb;
    public Transform sprite;
    [SerializeField] Animator animator;
    bool biasLeft;
    string currentState;
    void Start()
    {
         StartCoroutine(JumpDelay());

        int direction = Random.Range(1, 3);
        if (direction == 1)
            biasLeft = true;
        else
            biasLeft = false;
    }

    private void RandomJump()
    {
        int direction = Random.Range(1, 101);
        int power = Random.Range(8, 13);

        ChangeAnimationState("WaterStriderAnimation");

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
        yield return new WaitForSeconds(0.2f);
        ChangeAnimationState("WaterStriderIdle");
        float delay = Random.Range(1.3f, 3.8f);
        yield return new WaitForSeconds(delay);

        RandomJump();
    }
    void ChangeAnimationState(string newState)
    {
        //Stop the same animation from interrupting itself
        if (currentState == newState) return;

        //Play the new animation
        animator.Play(newState);
        currentState = newState;
    }
}