using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AfterPuteyBossCutscene : MonoBehaviour
{
    public bool cutsceneStarted;
    public GameObject player, elevator, putey, itemholder;
    public Vector3 playerTarget, elevatorTarget, puteyTarget;
    bool started;
    public int openelevatortimer;
    public AudioSource audioS;
    public AudioClip machineSound;
    public TriggeredSound triggeredSound;
    public OpenElevatorTimed elevatorTimed;
   
    void Update()
    {
        if (cutsceneStarted)
        {
            //player.transform.position = Vector3.MoveTowards(player.transform.position, playerTarget, 5 * Time.deltaTime);
            elevator.transform.position = Vector3.MoveTowards(elevator.transform.position, elevatorTarget, 5 * Time.deltaTime);
            if(putey)
            putey.transform.position = Vector3.MoveTowards(putey.transform.position, puteyTarget, 5 * Time.deltaTime);
        }
    }

    public void StartCutscene()
    {
        if (!started)
        {
            started = true;
            Invoke("settottrue", 5);
            Invoke("startDialogue", 6.5f);
            Invoke("openelevator", 22);
        }
    }
    private void settottrue()
    {
        cutsceneStarted = true;
        playMachineSound();
        //itemholder.gameObject.SetActive(false);
        //FindObjectOfType<PlayerMovement>().walkSpeed = 0;
        //FindObjectOfType<PlayerMovement>().crouchSpeed = 0;
    }

    public void playMachineSound()
    {
        audioS.PlayOneShot(machineSound);
    }

    public void startDialogue()
    {
        triggeredSound.GetTriggered();
    }

    public void openelevator()
    {
        elevatorTimed.openElevator();
    }
}
