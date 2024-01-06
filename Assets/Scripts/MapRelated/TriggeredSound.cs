using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggeredSound : MonoBehaviour
{
    public BoxCollider trigger;
    public AudioSource audioSource;
    public AudioClip clip;

    public Subtitles subtitles;

    public bool hasTriggered = false;

    public bool showSubtitles;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasTriggered)
        {
            GetTriggered();
    }
    }

    public void GetTriggered()
    {
        Destroy(trigger);
        hasTriggered = true;
        audioSource.PlayOneShot(clip);

        if (showSubtitles)
        {
            subtitles.gameObject.SetActive(true);
            subtitles.startSubtitles();
        }
    }
}
