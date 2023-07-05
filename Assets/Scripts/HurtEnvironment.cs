using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HurtEnvironment : MonoBehaviour
{
    public int damagePerSecond = 10;
    public bool instakill;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !instakill)
        {
            StartCoroutine(DamagePlayerOverTime());
        }
        else
        {
            Player player = FindObjectOfType<Player>();
            if (!player.dead)
            {
                player.playerDamageSound(damagePerSecond);
            }
            player.HP = 0;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StopCoroutine(DamagePlayerOverTime());
        }
    }

    private IEnumerator DamagePlayerOverTime()
    {
        while (true)
        {
            Player player = FindObjectOfType<Player>();
            player.playerDamageSound(damagePerSecond);
            player.HP -= damagePerSecond;
            yield return new WaitForSeconds(1f); // Wait for 1 second
        }
    }
}
