using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Transactions;
using Unity.VisualScripting;
using UnityEngine;

public class StemController : MonoBehaviour
{
    public Color color;
    private GameObject closestLilypad;

    private LineRenderer lr;
    [HideInInspector] public Vector3 stemBeginning;
    private bool hitMud;
    private float startDistance;
    private PolygonCollider2D polygonCollider;
    private Vector3[] linePositions = new Vector3[2];
    private CapsuleCollider2D lilypadCollider;

    void Start()
    {
        lr = GetComponent<LineRenderer>();
        polygonCollider = GetComponent<PolygonCollider2D>();

        //Finds all objects in the scene with a lilypad script then selects the closest one
        LilypadController[] lilypadScripts = FindObjectsByType<LilypadController>(FindObjectsSortMode.None);
        GameObject[] lilypads = new GameObject[lilypadScripts.Length];
        for (int it = 0;  it < lilypadScripts.Length; it++)
        {
            lilypads[it] = lilypadScripts[it].gameObject;
        }
        GetClosestLilypad(lilypads);

        lilypadCollider = closestLilypad.GetComponent<CapsuleCollider2D>();

        SetColor();

        //Find the first point on the ground under where the lilypad spawns
        RaycastHit2D[] hit = Physics2D.RaycastAll (closestLilypad.transform.position, Vector2.down);

        int i = 0;
        while (hitMud == false) //Stop searching when the raycast hits mud
        {
            i++;

            if (hit[i].collider.transform.gameObject.layer == 3)  //Returns true when the first mud (hit[i]) is detected
            {
                stemBeginning = hit[i].point;
                startDistance = Vector3.Distance(stemBeginning, closestLilypad.transform.position) * 1.04f;
                hitMud = true;
            }
        } 

        //Set the first point to the first point on the ground under where the lilypad spawns
        lr.positionCount = 1;
        lr.SetPosition(0, stemBeginning);
    }

    GameObject GetClosestLilypad(GameObject[] lilypads) 
    {
        float minDist = 1;
        Vector3 currentPos = transform.position;
        foreach (GameObject lilypad in lilypads)
        {
            float dist = Vector3.Distance(lilypad.transform.position, currentPos);
            if (dist < minDist)
            {
                closestLilypad = lilypad;
                minDist = dist;
            }
        }
        return closestLilypad.gameObject;
    }

    void FixedUpdate()
    {
        // Set the second line point on the lilyad
        if (lr.positionCount != 2)
            lr.positionCount = 2;

        lr.SetPosition(1,closestLilypad.transform.position - new Vector3(0,1f,0));

        //SetEdgeCollider();
        ConstrainDistance();

        if(!closestLilypad.activeSelf)
            gameObject.SetActive(false);
    }
    void ConstrainDistance()
    {
        float distance = Vector3.Distance(stemBeginning, closestLilypad.transform.position); //Distance from base to stem
        if (distance > startDistance)
        {
            //Sets a limit on how far the lilypad can be from the base of the stem
            Vector3 inwards = stemBeginning - closestLilypad.transform.position;
            inwards = inwards.normalized;
            inwards *= distance - startDistance;
            closestLilypad.transform.position += inwards;
        }
    }
    void SetColor()
    {
        lr.material = new Material(Shader.Find("Legacy Shaders/Particles/Alpha Blended Premultiply"));
        float alpha = 1.0f;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(color, 0.0f), new GradientColorKey(color, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(alpha, 0.0f), new GradientAlphaKey(alpha, 1.0f) }
        );
        lr.colorGradient = gradient;
    }
    private void SetEdgeCollider()
    {
        lr.GetPositions(linePositions);
        Vector3[] colliderPoints = new Vector3[lr.positionCount * 2];

        Vector2 width = new Vector2(0.25f, 0);
        bool swap = false;

        for (int i = 0; i < lr.positionCount * 2; i += 2)
        {
            Vector2 localLRPos = lr.transform.InverseTransformPoint(lr.GetPosition(i/2)); //The position of the lr points converted to local space

            //Spawns two points per lr point, one a little to the left and one a little to the right.

            //bool swap is used to spawn the points like: 
            // o --> o
            //       |
            // o <-- o

            if (!swap)
            {
                colliderPoints[i] = localLRPos - width;
                colliderPoints[i + 1] = localLRPos + width;
                swap = true;
            }
            else
            {
                colliderPoints[i] = localLRPos + width;
                colliderPoints[i + 1] = localLRPos - width;
                swap = false;
            }
        }

        polygonCollider.points = ToVector2Array(colliderPoints);
    }
    private Vector2[] ToVector2Array(Vector3[] v3)
    {
        return System.Array.ConvertAll<Vector3, Vector2>(v3, getV3fromV2);
    }
    private Vector2 getV3fromV2(Vector3 v3)
    {
        return new Vector2(v3.x, v3.y);
    }
}