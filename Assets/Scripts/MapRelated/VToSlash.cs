using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VToSlash : MonoBehaviour
{
   public bool tooltipfalse;
    // Update is called once per frame
    void Update()
    {
        if (FindObjectOfType<Player>().hasPlasmatana)
        {
            int e = Random.Range(0, FindObjectOfType<Player>().plasmaSwings.Length);
            AudioClip d = FindObjectOfType<Player>().plasmaSwings[e];
            FindObjectOfType<Player>().plamsatanasound.PlayOneShot(d);

            if (!tooltipfalse) FindObjectOfType<Tooltip>().getReportedTo("Press F to slash with the Plasmatana®!");

            Destroy(this.gameObject);
        }
    }
}
