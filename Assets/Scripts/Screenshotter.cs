using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Screenshotter : MonoBehaviour
{
    int count = 1;
    bool pause;
    public bool takeIpadScreenshot;
    void Update()
    {
        if (pause && !takeIpadScreenshot)
            Time.timeScale = 0;
        if (takeIpadScreenshot)
            Time.timeScale = 1;

        if(Input.GetKeyDown(KeyCode.Space) || takeIpadScreenshot)
        {
            Debug.Log("Screenshot Captured");
            pause = true;
            ScreenCapture.CaptureScreenshot("C:\\Users\\Thatc\\OneDrive\\Desktop\\AppleScreenshot-" + count + ".png");
            count++;
            takeIpadScreenshot = false;
        }
    }
}
