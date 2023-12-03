using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeWeapon : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip equip, pinpull; //grenade object makes throw sound on start
    public Animator anim;
    public AnimationClip idle, run, throwAnim;
    public float throwForce;

    public bool pulledPin, equipping;
    public GameObject thrownGrenade;

    public void Shoot()
    {
        FindObjectOfType<PlayerInventory>().weaponSwitchBlocked = true;
        pulledPin = true;
        audioSource.PlayOneShot(pinpull);
        Invoke("spawnNade", 1.15f);
        //1:15 seconds delay to throw
        anim.Play(throwAnim.name);
    }

    public void playCockingNoise()
    {
        audioSource.PlayOneShot(equip);
    }

    public void spawnNade()
    {
        GameObject gren = Instantiate(thrownGrenade, transform.position, Quaternion.identity);
        float torqueMagnitude = 2f;
        gren.GetComponent<Rigidbody>().AddForce(transform.forward * throwForce, ForceMode.Impulse);
        gren.GetComponent<Rigidbody>().AddTorque(Random.insideUnitSphere * torqueMagnitude, ForceMode.Impulse);
        FindObjectOfType<Player>().unequipGrenade();
        FindObjectOfType<PlayerInventory>().inventorySlots[FindObjectOfType<PlayerInventory>().activeSlot] = null;

        FindObjectOfType<PlayerInventory>().weaponSwitchBlocked = false;
    }

    public void equipAnim()
    {
        equipping = true;
        anim.Play(equip.name);
        Invoke("untrueEquipping", 0.45f);
    }

    public void untrueEquipping()
    {
        equipping = false;
    }
}
