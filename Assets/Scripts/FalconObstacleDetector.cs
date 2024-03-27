using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FalconObstacleDetector : MonoBehaviour
{
    [SerializeField] FalconBehavior falconBehavior;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 14) //Slide
        {
            falconBehavior.diving = false;
        }
        if (collision.gameObject.layer == 6) //Logs
        {
            if (collision.transform.parent != null && (collision.transform.parent.name == "Log(Clone)" || collision.transform.parent.name == "AmazonLog(Clone)"))
            {
                falconBehavior.diving = false;
                
            }

            if (collision.gameObject.name == "Log(Clone)" || collision.gameObject.name == "AmazonLog" //Cypress knees
                || collision.gameObject.name == "CypressTree" || collision.gameObject.name == "CypressStick1" /*|| collision.gameObject.name == "CypressKnee3" || collision.gameObject.name == "CypressKnee4"
                || collision.gameObject.name == "CypressKnee5" || collision.gameObject.name == "CypressKnee6" || collision.gameObject.name == "CypressKnee7" || collision.gameObject.name == "CypressKnee8"
                || collision.gameObject.name == "CypressKnee9" || collision.gameObject.name == "CypressKnee10"*/)
            {
                falconBehavior.diving = false;
            }
        }
    }
}
