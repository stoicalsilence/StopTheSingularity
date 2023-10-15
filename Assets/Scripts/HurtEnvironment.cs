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
        else if(other.CompareTag("Player") && instakill)
        {
            Player player = FindObjectOfType<Player>();
            if (!player.dead)
            {
                player.playerDamageSound(damagePerSecond);
            }
            player.HP = 0;
        }
        Eyenemy eye;
        if (other.gameObject.TryGetComponent<Eyenemy>(out eye))
        {
            int amodmg = other.gameObject.GetComponent<Eyenemy>().health;
            for (int i = 0; i < amodmg; i++)
            {
                eye.TakeDamage();
            }
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
