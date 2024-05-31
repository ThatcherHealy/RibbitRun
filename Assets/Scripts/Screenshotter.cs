using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Screenshotter : MonoBehaviour
{
    int count = 1;
    bool pause;
    void Update()
    {
        if (pause)
            Time.timeScale = 0;

        if(Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Screenshot Captured");
            pause = true;
            ScreenCapture.CaptureScreenshot("C:/Users/Thatc/Desktop/AppleScreenshot-" + count + ".png");
            count++;
        }
    }
}
