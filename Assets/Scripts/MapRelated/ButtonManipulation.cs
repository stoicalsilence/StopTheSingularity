using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonManipulation : MonoBehaviour
{
    public OpenDoorButton openDoorButton;
    public GameObject objectToMove;
    public Transform targetPos;
    public float moveSpeed;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (openDoorButton.isPressed)
        {
            MoveDoor(objectToMove, targetPos.position);
        }
    }

    private void MoveDoor(GameObject door, Vector3 targetPos)
    {
        if (door.transform.position != targetPos)
        {
            door.transform.position = Vector3.MoveTowards(door.transform.position, targetPos, moveSpeed * Time.deltaTime);
        }
    }
}
