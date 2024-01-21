using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System.Runtime.CompilerServices;

public class CameraFollow : MonoBehaviour
{
    public Transform player;
    public Transform cameraGuide;
    public Rigidbody2D playerRB;
    public PlayerController playerController;
    public CinemachineVirtualCamera virtualCamera;

    private float xOffset = 5;
    private bool lookingRight = true;
    private float velocityThreshold = 10;

    private float heightDamping = 0.2f;
    private float baseHeight = 5f;
    private float yInwardsBias = 5f;

    private Vector3 velocity = Vector3.zero;
    float mudLevel = -34.20609f;

    private void FixedUpdate()
    {
        LookAhead();
        CameraHeight();
    }

    private void CameraHeight()
    {
        float minHeight = -8;
        float maxHeight = 13;
        if (player.position.y <= maxHeight && player.position.y >= minHeight) //Default view window
        {
            //Set the camera to smoothly move slightly ahead of where the player is moving on the x direction
            //Set the camera to a set Y Value (baseHeight)

            Vector3 targetPosition = new Vector3(player.position.x + xOffset, baseHeight, 0);
            cameraGuide.position = Vector3.SmoothDamp(cameraGuide.position, targetPosition, ref velocity, heightDamping);
        }
        else
        {
            if (player.position.y > maxHeight) //Above default view window
            {
                //Smoothly move the camera out of the baseHeight to a height of yInwardBias *below* the player's position

                Vector3 targetPosition = new Vector3(player.position.x + xOffset, player.position.y - (yInwardsBias + 2), 0);
                cameraGuide.position = Vector3.SmoothDamp(cameraGuide.position, targetPosition, ref velocity, heightDamping);
            }
            else //Below default view window
            {
                //Smoothly move the camera out of the baseHeight to a height of yInwardBias *above* the player's position
                //until it reaches a set distance from the mud, at which point its vertical motion stops

                float offsetAboveMud = 13;
                Vector3 targetPosition = new Vector3(player.position.x + xOffset, player.position.y + yInwardsBias, 0);
                if (targetPosition.y < -21)
                {
                    targetPosition = new Vector3(targetPosition.x, mudLevel + offsetAboveMud, targetPosition.z);
                }
                cameraGuide.position = Vector3.SmoothDamp(cameraGuide.position, targetPosition, ref velocity, heightDamping);
            }
        }
    }
    private void LookAhead()
    {
        //When the player moves faster than the velocityThreshold in the direction they are not currently facing, they turn to that direction
        //xOffset is the max distance that the camera can be in front of the player

        if (lookingRight && playerRB.velocity.x < -velocityThreshold)
        {
            lookingRight = false;
            xOffset = -xOffset;
        }
        else if (!lookingRight && playerRB.velocity.x > velocityThreshold)
        {
            xOffset = Mathf.Abs(xOffset);
            lookingRight = true;
        }
    }
}