using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level20Weapondrop : MonoBehaviour
{
    public TriggeredSound trigger;
    public GameObject thingToMove;
    public Vector3 ogTransform;
    public Vector3 moveTransform;
    bool didthething = false;
    bool didthethang = false;
    public GameObject uzipickup, glockpickup;
    public Transform droppos;
    public AudioClip shooooov;
    public AudioSource ad;
    // Start is called before the first frame update
    void Start()
    {
        ogTransform = thingToMove.transform.position;
        moveTransform = thingToMove.transform.position + new Vector3(0, -3, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (didthething)
        {
            if (!didthethang)
            {
                didthethang = true;
                ad.PlayOneShot(shooooov);
                Invoke("doTheThings", 5);
                Invoke("doTheThangs", 3);
            }
        }

        if (trigger.hasTriggered)
        {
            MoveDoor(thingToMove, moveTransform);
            didthething = true;
        }
        else
        {
            MoveDoor(thingToMove, ogTransform);
        }
    }

    private void MoveDoor(GameObject door, Vector3 targetPos)
    {
        if (door.transform.position != targetPos)
        {
            door.transform.position = Vector3.MoveTowards(door.transform.position, targetPos, 3 * Time.deltaTime);
        }
    }

    void doTheThings()
    {
        trigger.hasTriggered = false;
        ad.PlayOneShot(shooooov);
    }

    void doTheThangs()
    {
        Vector3 e = droppos.position;
        if(uzipickup.GetComponent<Pickup>().weaponName== "BattleRifle")
        {
            e += new Vector3(1.5f, 1.5f, 0);
        }
        Instantiate(uzipickup, e, Quaternion.identity);
        Instantiate(glockpickup, droppos.position, Quaternion.identity);
    }
}
