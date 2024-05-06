using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TongueLine : MonoBehaviour
{
    [Header("General Refernces:")]
    [SerializeField] Transform tongueStartPoint;
    public TongueLauncher tongueLauncher;
    public LineRenderer m_lineRenderer;
    [SerializeField] SFXManager sfx;

    [Header("General Settings:")]
    [SerializeField] private int precision = 40;
    [Range(0, 20)][SerializeField] private float straightenLineSpeed = 5;

    [Header("Rope Animation Settings:")]
    public AnimationCurve ropeAnimationCurve;
    [Range(0.01f, 4)][SerializeField] private float StartWaveSize = 2;
    float waveSize = 0;

    [Header("Rope Progression:")]
    public AnimationCurve ropeProgressionCurve;
    [Range(1, 50)] public float ropeProgressionSpeed;

    float moveTime = 0;

    public bool isGrappling = true;

    bool straightLine = true;

    bool sfxAllowedToPlay;

    private void OnEnable()
    {
        if(sfxAllowedToPlay)
            sfx.PlaySFX("Grapple");

        moveTime = 0;
        m_lineRenderer.positionCount = precision;
        waveSize = StartWaveSize;
        straightLine = true;

        LinePointsToFirePoint();

        m_lineRenderer.enabled = true;
    }

    private void OnDisable()
    {
        m_lineRenderer.enabled = false;
        isGrappling = false;
        sfxAllowedToPlay = true;
    }

    private void LinePointsToFirePoint()
    {
        for (int i = 0; i < precision; i++)
        {
            m_lineRenderer.SetPosition(i, tongueLauncher.firePoint.position);
        }
    }

    private void Update()
    {
        moveTime += Time.deltaTime;
        DrawRope();
    }

    void DrawRope()
    {
        if (!straightLine)
        {
            if (m_lineRenderer.GetPosition(precision - 1).x == tongueLauncher.grapplePoint.x)
            {
                straightLine = true;
            }
            else
            {
                DrawRopeWaves();
            }
        }
        else
        {
            if (!isGrappling)
            {
                isGrappling = true;
            }
            if (waveSize > 0)
            {
                waveSize -= Time.deltaTime * straightenLineSpeed;
                DrawRopeWaves();
            }
            else
            {
                waveSize = 0;

                if (m_lineRenderer.positionCount != 2) { m_lineRenderer.positionCount = 2; }

                DrawRopeNoWaves();
            }
        }
    }

    void DrawRopeWaves()
    {
        for (int i = 0; i < precision; i++)
        {
            float delta = (float)i / ((float)precision - 1f);
            Vector2 offset = Vector2.Perpendicular(tongueLauncher.grappleDistanceVector).normalized * ropeAnimationCurve.Evaluate(delta) * waveSize;
            Vector2 targetPosition = Vector2.Lerp(tongueLauncher.firePoint.position, tongueLauncher.grapplePoint, delta) + offset;
            Vector2 currentPosition = Vector2.Lerp(tongueLauncher.firePoint.position, targetPosition, ropeProgressionCurve.Evaluate(moveTime) * ropeProgressionSpeed);

            m_lineRenderer.SetPosition(i, currentPosition);
        }
    }

    void DrawRopeNoWaves()
    {
        m_lineRenderer.SetPosition(0, tongueLauncher.firePoint.position);
        m_lineRenderer.SetPosition(1, tongueLauncher.grapplePoint);
    }
}