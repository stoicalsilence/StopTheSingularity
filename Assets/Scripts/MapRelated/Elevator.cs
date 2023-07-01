using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Elevator : MonoBehaviour
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
    public AudioClip doorCloseSound;

    private bool wasOpened = false;

    public BoxCollider playerEnterTrigger;
    public BoxCollider blockadeCollider;
    private bool sceneTransitionStarted = false;
    public string nextSceneName;

    private void Update()
    {
        if (opened != wasOpened)
        {
            if (opened)
            {
                PlaySoundEffect(doorOpenSound);
            }
            else
            {
                PlaySoundEffect(doorCloseSound);
            }
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
        audioSource.PlayOneShot(clip);
    }

    private void MoveDoor(GameObject door, Vector3 targetPos)
    {
        if (door.transform.position != targetPos)
        {
            door.transform.position = Vector3.MoveTowards(door.transform.position, targetPos, doorSpeed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            opened = false;
            playerEnterTrigger.enabled = false;
            blockadeCollider.enabled = true;

            if (!sceneTransitionStarted)
            {
                StartCoroutine(StartSceneTransition());
            }
        }
    }

    private IEnumerator StartSceneTransition()
    {
        sceneTransitionStarted = true;
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(nextSceneName);
    }
}