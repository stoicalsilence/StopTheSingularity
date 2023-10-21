using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorKillToProceed : MonoBehaviour
{
    public List<GameObject> objectsToDestroy;
    public GameObject objectToMove;
    public Transform targetPos;
    public float moveSpeed;
    public bool allEnemiesDestroyed = false;
    public AudioSource audiosource;

    private void Update()
    {
        if (!allEnemiesDestroyed)
        {
            if (AreAllEnemiesDestroyed())
            {
                allEnemiesDestroyed = true;
                audiosource.Play();
            }
        }
        else
        {
            MoveDoor(objectToMove, targetPos.position);
        }
    }

    private bool AreAllEnemiesDestroyed()
    {
        foreach (GameObject obj in objectsToDestroy)
        {
            if (obj != null)
            {
                return false; // At least one enemy is still alive
            }
        }
        return true; // All enemies are destroyed
    }

    private void MoveDoor(GameObject door, Vector3 targetPos)
    {
        if (door.transform.position != targetPos)
        {
            door.transform.position = Vector3.MoveTowards(door.transform.position, targetPos, moveSpeed * Time.deltaTime);
        }
    }
}