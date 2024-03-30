using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockAndCloseDoor : MonoBehaviour
{
    public bool blocking;
    public BoxCollider collider;
    public ButtonManipulation door;

    public bool shouldTriggerPutey;
    public bool shouldTriggerZaps;
    public bool shouldTriggerGetty;

    public PuteyBoss putey;
    public MrZaps zaps;
    public Getty getty;

    bool zapstriggered;
    public GameObject zapslid;
    public Transform zapsLidPos;
    bool triggered;

    private void Update()
    {
        if (zapstriggered)
        {
            MoveDoor(zapslid, zapsLidPos.position);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player" && !triggered)
        {
            triggered = true;
            blocking = true;
            collider.enabled = true;
            if (door != null) door.openDoorButton.isPressed = false;

            if (shouldTriggerPutey)
                putey.startBossFight();

            if (shouldTriggerZaps)
            {
                Invoke("zapstrigger", 1);
                zaps.gameObject.SetActive(true);
            }

            if (shouldTriggerGetty)
            {
                getty.GetTriggered();
            }
        }
    }

    private void MoveDoor(GameObject door, Vector3 targetPos)
    {
        if (door.transform.position != targetPos)
        {
            door.transform.position = Vector3.MoveTowards(door.transform.position, targetPos, 5 * Time.deltaTime);
        }
    }

    void zapstrigger()
    {
        zapstriggered = true;
    }
}
