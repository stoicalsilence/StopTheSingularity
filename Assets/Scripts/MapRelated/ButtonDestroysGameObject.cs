using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonDestroysGameObject : MonoBehaviour
{
    public OpenDoorButton opendoorbutton;
    public GameObject todestroy;
    // Start is called before the first frame update
 

    // Update is called once per frame
    void Update()
    {
        if (opendoorbutton.isPressed)
        {
            Destroy(todestroy);
            Destroy(this);
        }
    }
}
