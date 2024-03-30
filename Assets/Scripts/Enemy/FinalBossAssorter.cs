using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FinalBossAssorter : MonoBehaviour
{
    public Slider tankerSlider;
    public Slider lordPuteySlider;

    public GameObject Ramp;

    public bool hideRamp;

    public Vector3 originalPos;
    public Vector3 hidePos;

    public AudioSource BGM;

    private void Start()
    {
        originalPos = Ramp.transform.position;
    }

    private void Update()
    {
        if (hideRamp)
        {
            MoveRamp(Ramp, hidePos);
        }
        else
        {
            MoveRamp(Ramp, originalPos);
        }
    }

    private void MoveRamp(GameObject door, Vector3 targetPos)
    {
        if (door.transform.position != targetPos)
        {
            door.transform.position = Vector3.MoveTowards(door.transform.position, targetPos, 3 * Time.deltaTime);
        }
    }
}
