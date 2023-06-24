using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    // Start is called before the first frame update
  
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "PlayerAttack")
        {
            Destroy(GetComponent<BoxCollider>());
            GetComponent<MeshDestroy>().DestroyMesh();
        }
    }
}
