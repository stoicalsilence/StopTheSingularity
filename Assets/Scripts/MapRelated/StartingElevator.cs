using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartingElevator : MonoBehaviour
{
    public Transform leftDoorClosedPos;
    public Transform rightDoorClosedPos;
    public Transform leftDoorOpenedPos;
    public Transform rightDoorOpenedPos;

    public GameObject leftDoor;
    public GameObject rightDoor;

    public bool opened = false;

    public float doorSpeed = 2f;

    public AudioSource audioSource;

    public AudioClip doorOpenSound;
    public AudioClip dingSound;

    private bool wasOpened = false;

    private float elapsedTime = 0f;
    private bool hasPlayedSound = false;

    private void Start()
    {
        Invoke("PlayDingSoundEffect", 1);
    }
    private void Update()
    {
        elapsedTime += Time.deltaTime;

        if (elapsedTime >= 3f && !opened)
        {
            opened = true;
            PlaySoundEffect(doorOpenSound);
        }

        if (opened != wasOpened)
        {
            wasOpened = opened;
        }

        if (opened)
        {
            MoveDoor(leftDoor, leftDoorOpenedPos.position);
            MoveDoor(rightDoor, rightDoorOpenedPos.position);
        }
        else
        {
            MoveDoor(leftDoor, leftDoorClosedPos.position);
            MoveDoor(rightDoor, rightDoorClosedPos.position);
        }
    }

    private void PlaySoundEffect(AudioClip clip)
    {
        if (!hasPlayedSound)
        {
            audioSource.PlayOneShot(clip);
            hasPlayedSound = true;
        }
    }
    private void PlayDingSoundEffect()
    {
        audioSource.PlayOneShot(dingSound);
    }

    private void MoveDoor(GameObject door, Vector3 targetPos)
    {
        if (door.gameObject.transform.position != targetPos)
        {
            door.gameObject.transform.position = Vector3.MoveTowards(door.gameObject.transform.position, targetPos, doorSpeed * Time.deltaTime);
        }
    }
}