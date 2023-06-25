using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlideDestroy : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag =="Player" && FindObjectOfType<PlayerMovement>().isCrouching)
        {
            gameObject.GetComponent<Destructable>().explode();
        }
    }
}
