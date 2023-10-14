using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VToSlash : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (FindObjectOfType<Player>().hasPlasmatana)
        {
            int e = Random.Range(0, FindObjectOfType<Player>().plasmaSwings.Length);
            AudioClip d = FindObjectOfType<Player>().plasmaSwings[e];
            FindObjectOfType<Player>().plamsatanasound.PlayOneShot(d);
            FindObjectOfType<Tooltip>().getReportedTo("Press V to slash with the Plasmatana®!");
            Destroy(this.gameObject);
        }
    }
}
