using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenDoorButton : MonoBehaviour
{
    public float interactionRange = 3f; // The range within which the player can interact with the button
    public float moveDistance = 1f; // The distance the button moves
    public Transform targetTransform; // The target position for the button to move
    public bool clickableMultipleTimes = true; // Determines if the button can be clicked multiple times

    public bool isPressed = false; // Indicates if the button has been pressed
    private Vector3 initialPosition; // Stores the initial position of the button

    public AudioClip buttonClickSound; // Sound to be played when the button is clicked
    private AudioSource audioSource; // Reference to the audio source component

    private void Start()
    {
        initialPosition = gameObject.transform.position;
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, interactionRange))
            {
                if (hit.collider.gameObject == gameObject)
                {
                    if (!isPressed)
                    {
                        PressButton();
                    }
                    else if (clickableMultipleTimes)
                    {
                        ResetButton();
                    }
                }
            }
        }
    }

    private void PressButton()
    {
        if (buttonClickSound != null)
        {
            audioSource.PlayOneShot(buttonClickSound);
        }

        StartCoroutine(MoveButton());
        isPressed = true;
    }

    private IEnumerator MoveButton()
    {
        Vector3 startPosition = gameObject.transform.position;
        Vector3 targetPosition = targetTransform.position;

        float elapsedTime = 0f;
        float duration = 1f; // The time it takes to move the button

        while (elapsedTime < duration)
        {
            gameObject.transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    private void ResetButton()
    {
        if (buttonClickSound != null)
        {
            audioSource.PlayOneShot(buttonClickSound);
        }
        StartCoroutine(MoveButtonBack());
        isPressed = false;
    }

    private IEnumerator MoveButtonBack()
    {
        Vector3 startPosition = gameObject.transform.position;
        Vector3 targetPosition = initialPosition;

        float elapsedTime = 0f;
        float duration = 1f; // The time it takes to move the button back

        while (elapsedTime < duration)
        {
            gameObject.transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
}
