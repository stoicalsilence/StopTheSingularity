using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KyojunTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            FindObjectOfType<Kyojun>().GetTriggered();
            Destroy(this.gameObject);
        }
    }
}
