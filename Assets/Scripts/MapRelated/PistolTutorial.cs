using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PistolTutorial : MonoBehaviour
{
    // Start is called before the first frame update
    

    // Update is called once per frame
    void Update()
    {
        if (FindObjectOfType<Player>().glockEquipped)
        {
            FindObjectOfType<Tooltip>().getReportedTo("Left click to shoot!\nR to reload!\nQ to drop!\n1-2-3 numbers for inventory (3 is holstered)");
            Destroy(this.gameObject);
        }
    }
}
