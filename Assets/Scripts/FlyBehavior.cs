using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class FlyBehavior : MonoBehaviour
{
    public float speed = 2;
    public float scaleX = 2;
    public float scaleY = 3;
    public float offsetX = 0;
    public float offsetY = 0;

    public bool isLinkOffsetScalePositiveX = false;
    public bool isLinkOffsetScaleNegativeX = false;
    public bool isLinkOffsetScalePositiveY = false;
    public bool isLinkOffsetScaleNegativeY = false;

    private float phase;
    private float m_2PI = Mathf.PI * 2;
    private Vector3 originalPosition;
    private Vector3 pivot;
    private Vector3 pivotOffset;
    private bool isInverted = false;

    public Behavior behavior;
    public enum Behavior
    {
        FigureEight,
        Circle,
        Infinity
    }

    void Start()
    {
        int randomInverse = Random.Range(0, 2);
        if (randomInverse == 0)
        {
            isInverted = true;
        }
        else if (randomInverse == 1)
        {
            isInverted = false;
        }

        int randomBehavior = Random.Range(0, 2);
        if (randomBehavior == 0 )
        {
            behavior = Behavior.FigureEight;
        }
        else if (randomBehavior == 1 )
        {
            behavior = Behavior.Circle;
        }

        pivot = transform.position;
        originalPosition = transform.position;

        if (isLinkOffsetScalePositiveX)
            phase = 3.14f / 2f + 3.14f;
        else if (isLinkOffsetScaleNegativeX)
            phase = 3.14f / 2f;
        else if (isLinkOffsetScalePositiveY)
            phase = 3.14f;
        else
            phase = 0;
    }

    void Update()
    {
        switch (behavior)
        {
            case Behavior.FigureEight:

            pivotOffset = Vector3.up * 2 * scaleY;

            phase += speed * Time.deltaTime;

            if (phase > m_2PI)
            {
                //Debug.Log("phase " + phase + " over 2pi: " + isInverted);
                isInverted = !isInverted;
                phase -= m_2PI;
            }
            if (phase < 0)
            {
                //Debug.Log("phase " + phase + " under 0");
                phase += m_2PI;
            }
            break;

            case Behavior.Circle:

            pivotOffset = Vector3.up * 2 * scaleY;

            phase += speed * Time.deltaTime;

            break;
        }

        Vector3 nextPosition = pivot + (isInverted ? pivotOffset : Vector3.zero);
        transform.position = new Vector2(nextPosition.x + Mathf.Sin(phase) * scaleX + offsetX, nextPosition.y + Mathf.Cos(phase) * (isInverted ? -1 : 1) * scaleY + offsetY);
    }
}
