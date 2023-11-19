using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuteyBossPillars : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<MissileController>())
        {
            GetComponent<MeshDestroy>().DestroyMesh();
            Destroy(GetComponent<MeshDestroy>());
            Destroy(this);
        }
    }
}
