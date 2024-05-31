using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IpadScaler : MonoBehaviour
{
    [SerializeField] float xScale;
    [SerializeField] bool mainMenu;
    [SerializeField] RectTransform[] buttons;
    private void Awake()
    {
        if (SystemInfo.deviceModel.Contains("iPad") || SystemInfo.deviceModel.StartsWith("iPad"))
        {
            GetComponent<CanvasScaler>().referenceResolution = new Vector2(xScale, GetComponent<CanvasScaler>().referenceResolution.y);

            if (mainMenu)
                foreach (RectTransform button in buttons)
                    button.anchoredPosition = new Vector2(button.anchoredPosition.x, button.anchoredPosition.y);
        }
    }
}