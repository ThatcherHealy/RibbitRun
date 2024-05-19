using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChangeFloatingTextColor : MonoBehaviour
{
    void Start()
    {
        if (PlayerPrefs.GetInt("HardMode", 0) == 1)
        {
            GetComponent<TextMeshPro>().color = Color.red;
        }
    }
}
