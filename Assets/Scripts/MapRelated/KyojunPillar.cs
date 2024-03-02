using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KyojunPillar : MonoBehaviour
{
    public void getBroken()
    {
        gameObject.tag = "Untagged";
        GetComponent<MeshDestroy>().DestroyMesh();
        Destroy(GetComponent<MeshDestroy>());
        Destroy(this);
    }
}
