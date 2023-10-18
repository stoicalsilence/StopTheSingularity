using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnOffBgm : MonoBehaviour
{
    void Start()
    {
        Destroy(GameObject.Find("BGM"));
        Destroy(this.gameObject);
    }
}
