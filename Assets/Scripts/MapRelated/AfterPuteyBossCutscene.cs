using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AfterPuteyBossCutscene : MonoBehaviour
{
    public bool cutsceneStarted;
    public GameObject player, elevator, putey, gun;
    public Vector3 playerTarget, elevatorTarget, puteyTarget, gunTarget;

    public AudioSource audioS;
    public AudioClip machineSound, dialogue;
   
    void Update()
    {
        if (cutsceneStarted)
        {
            player.transform.position = Vector3.MoveTowards(player.transform.position, playerTarget, 5 * Time.deltaTime);
            elevator.transform.position = Vector3.MoveTowards(elevator.transform.position, elevatorTarget, 5 * Time.deltaTime);
            putey.transform.position = Vector3.MoveTowards(putey.transform.position, puteyTarget, 5 * Time.deltaTime);
            gun.transform.position = Vector3.MoveTowards(gun.transform.position, gunTarget, 5 * Time.deltaTime);
        }
    }

    public void StartCutscene()
    {
        cutsceneStarted = true;
    }
}
