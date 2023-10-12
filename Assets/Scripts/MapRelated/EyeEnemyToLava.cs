using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeEnemyToLava : MonoBehaviour
{
    public GameObject barrier;
    private void OnTriggerEnter(Collider collision)
    {
        Eyenemy eye;
        if (collision.gameObject.TryGetComponent<Eyenemy>(out eye))
        {
            eye.TakeDamage();
            Destroy(barrier.gameObject);
        }
    }
}
