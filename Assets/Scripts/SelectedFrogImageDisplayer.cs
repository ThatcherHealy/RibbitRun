using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedFrogImageDisplayer : MonoBehaviour
{
    [SerializeField] Sprite defaultFrog;
    [SerializeField] Sprite treeFrog;
    [SerializeField] Sprite froglet;
    [SerializeField] Sprite bullfrog;
    [SerializeField] Sprite poisonDartFrog;

    void Start()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if(sr != null) 
        {
            if (PlayerPrefs.GetString("Species") == "Default")
            {
                sr.sprite = defaultFrog;

                Vector3 desiredWorldScale = new Vector3(-2, 2, 1);
                Vector3 worldScaleFactor = new Vector3(
           desiredWorldScale.x / transform.lossyScale.x,
           desiredWorldScale.y / transform.lossyScale.y,
           desiredWorldScale.z / transform.lossyScale.z);

                // Apply the scaling factor
                transform.localScale = Vector3.Scale(transform.localScale, worldScaleFactor);
            }
            if (PlayerPrefs.GetString("Species") == "Tree Frog")
            {
                sr.sprite = treeFrog;
                Vector3 desiredWorldScale = new Vector3(-0.8f, 0.8f, 1);
                Vector3 worldScaleFactor = new Vector3(
           desiredWorldScale.x / transform.lossyScale.x,
           desiredWorldScale.y / transform.lossyScale.y,
           desiredWorldScale.z / transform.lossyScale.z);

                // Apply the scaling factor
                transform.localScale = Vector3.Scale(transform.localScale, worldScaleFactor);
            }
            if (PlayerPrefs.GetString("Species") == "Froglet" && froglet != null)
            {
                sr.sprite = froglet;
                Vector3 desiredWorldScale = new Vector3(-1f, 1f, 1);
                Vector3 worldScaleFactor = new Vector3(
           desiredWorldScale.x / transform.lossyScale.x,
           desiredWorldScale.y / transform.lossyScale.y,
           desiredWorldScale.z / transform.lossyScale.z);

                // Apply the scaling factor
                transform.localScale = Vector3.Scale(transform.localScale, worldScaleFactor);
            }
            if (PlayerPrefs.GetString("Species") == "Bullfrog" && bullfrog != null)
            {
                sr.sprite = bullfrog;
                Vector3 desiredWorldScale = new Vector3(-2.5f, 2.5f, 1);
                Vector3 worldScaleFactor = new Vector3(
           desiredWorldScale.x / transform.lossyScale.x,
           desiredWorldScale.y / transform.lossyScale.y,
           desiredWorldScale.z / transform.lossyScale.z);

                // Apply the scaling factor
                transform.localScale = Vector3.Scale(transform.localScale, worldScaleFactor);
            }
            if (PlayerPrefs.GetString("Species") == "Poison Dart Frog")
            {
                sr.sprite = poisonDartFrog;
                Vector3 desiredWorldScale = new Vector3(-1.6f, 1.6f, 1);
                Vector3 worldScaleFactor = new Vector3(
           desiredWorldScale.x / transform.lossyScale.x,
           desiredWorldScale.y / transform.lossyScale.y,
           desiredWorldScale.z / transform.lossyScale.z);

                // Apply the scaling factor
                transform.localScale = Vector3.Scale(transform.localScale, worldScaleFactor);
            }
        }
    }
}
