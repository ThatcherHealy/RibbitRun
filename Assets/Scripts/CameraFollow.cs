using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System.Runtime.CompilerServices;
using UnityEngine.SceneManagement;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] Transform player;
    [SerializeField] Transform cameraGuide;
    [SerializeField] Rigidbody2D playerRB;
    [SerializeField] PlayerController playerController;
    [SerializeField] CinemachineVirtualCamera virtualCamera;

    private float xOffset = 5;
    private bool lookingRight = true;
    private float velocityThreshold = 10;

    private float heightDamping = 0.2f;
    [HideInInspector] public float baseHeight = 5f;
    private float yInwardsBias = 5f;

    private Vector3 velocity = Vector3.zero;
    [SerializeField]public float mudLevel = -34.20609f;
    public float offsetAboveMud = 13;
    public float lowerBound = 26;

    Vector3 pausePosition;
    bool pausePositionSet;
    bool paused;

    private void FixedUpdate()
    {
        if (playerController.eaten && SceneManager.GetActiveScene().name != "Tutorial")
        {
            StartCoroutine(Pause());
        }
        
        if (!paused) 
        {
            LookAhead();
            CameraHeight();
        }
    }

    private void CameraHeight()
    {
        float minHeight = baseHeight - 13;
        float maxHeight = baseHeight + 8;
        if (player.position.y <= maxHeight && player.position.y >= minHeight) //Default view window
        {
            //Set the camera to smoothly move slightly ahead of where the player is moving on the x direction
            //Set the camera to a set Y Value (baseHeight)

            Vector3 targetPosition = new Vector3(player.position.x + xOffset, baseHeight, 0);

            if (playerController.killer == "Falcon(Clone)" || playerController.killer == "Falcon" || playerController.eatenByFalcon) //Move the target up when eaten by a falcon to keep it in frame
            {
                targetPosition = new Vector3(targetPosition.x, targetPosition.y + 20, targetPosition.z);
            }

            cameraGuide.position = Vector3.SmoothDamp(cameraGuide.position, targetPosition, ref velocity, heightDamping);
        }
        else
        {
            if (player.position.y > maxHeight) //Above default view window
            {
                //Smoothly move the camera out of the baseHeight to a height of yInwardBias *below* the player's position

                Vector3 targetPosition = new Vector3(player.position.x + xOffset, player.position.y - (yInwardsBias + 2), 0);

                if (playerController.killer == "Falcon(Clone)" || playerController.killer == "Falcon" || playerController.eatenByFalcon) //Move the target up when eaten by a falcon to keep it in frame
                {
                    targetPosition = new Vector3(targetPosition.x, targetPosition.y + 20, targetPosition.z);
                }

                cameraGuide.position = Vector3.SmoothDamp(cameraGuide.position, targetPosition, ref velocity, heightDamping);
            }
            else //Below default view window
            {
                //Smoothly move the camera out of the baseHeight to a height of yInwardBias *above* the player's position
                //until it reaches a set distance from the mud, at which point its vertical motion stops

                Vector3 targetPosition = new Vector3(player.position.x + xOffset, player.position.y + yInwardsBias, 0);
                if (targetPosition.y < baseHeight - lowerBound)
                {
                    targetPosition = new Vector3(targetPosition.x, mudLevel + offsetAboveMud, targetPosition.z);
                }

                if (playerController.killer == "Falcon(Clone)" || playerController.killer == "Falcon" || playerController.eatenByFalcon) //Move the target up when eaten by a falcon to keep it in frame
                {
                    targetPosition = new Vector3(targetPosition.x, targetPosition.y + 20, targetPosition.z);
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
    IEnumerator Pause()
    {
        yield return new WaitForSeconds(2);
        paused = true;

        if (!pausePositionSet)
        {
            pausePosition = cameraGuide.position;
            pausePositionSet = true;
        }
        cameraGuide.position = pausePosition;
    }
}