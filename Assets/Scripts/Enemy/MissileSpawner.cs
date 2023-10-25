using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileSpawner : MonoBehaviour
{
    public GameObject missilePrefab;
    public Transform spawnPosition, targetTransform;
    public float selfexplodetimer;
    
    void Start()
    {
        InvokeRepeating("spawnMissile", 4, 6);
    }

    public void spawnMissile()
    {
        GameObject missile = Instantiate(missilePrefab, spawnPosition.position, Quaternion.identity);
        missile.GetComponent<MissileController>().target = targetTransform;
        missile.GetComponent<MissileController>().selfexplodetimer = this.selfexplodetimer;
    }
}
