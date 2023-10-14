using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerToolTip : MonoBehaviour
{
    public string message;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            FindObjectOfType<Tooltip>().getReportedTo(message);
            Destroy(this.gameObject);
        }
    }
}
