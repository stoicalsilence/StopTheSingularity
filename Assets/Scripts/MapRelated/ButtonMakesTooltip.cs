using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonMakesTooltip : MonoBehaviour
{
    public OpenDoorButton opendoorbutton;
    public string tooltip;
    // Start is called before the first frame update


    // Update is called once per frame
    void Update()
    {
        if (opendoorbutton.isPressed)
        {
            FindObjectOfType<Tooltip>().getReportedTo(tooltip);
            Destroy(this);
        }
    }
}
