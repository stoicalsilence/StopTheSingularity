using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KjoyunBodyPart : MonoBehaviour
{
    public float currentHP;
    public float maxHP;
    public string bodypartName;
    public bool destroyed;
    public GameObject hitSparkles;
    public GameObject fillRectToDisable;
    public GameObject bodyFillRect;
    public Slider hpSlider;
    public AudioClip explodesound;
    public static float defenseMultiplier = 0.9f;
    public AudioSource audioSource;
    public AudioClip[] damageSounds;
    public bool isGetty; 

    private void Start()
    {
        hpSlider.maxValue = maxHP;
        currentHP = maxHP;
    }
    void Update()
    {
        hpSlider.value = currentHP;
        if (currentHP < 1 && !destroyed)
        {
            ScreenShake.Shake(0.75f, 0.4f);
            destroyed = true;
            fillRectToDisable.SetActive(false);
            if (bodypartName != "Body")
            {
                KjoyunBodyPart.defenseMultiplier -= 0.3f;
                bodyFillRect.GetComponent<Image>().color = new Color(bodyFillRect.GetComponent<Image>().color.r + 50, bodyFillRect.GetComponent<Image>().color.g, bodyFillRect.GetComponent<Image>().color.b, bodyFillRect.GetComponent<Image>().color.a);
                if (isGetty)
                {
                    FindObjectOfType<Getty>().generalAudiosource.PlayOneShot(explodesound);
                }
                else
                {
                    FindObjectOfType<Kyojun>().generalAudiosource.PlayOneShot(explodesound);
                }
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "PlayerAttack" && FindObjectOfType<Getty>().triggered)
        {
            takeDamage();
            
            Vector3 collisionPoint = collision.GetContact(0).point;
            GameObject spark = Instantiate(hitSparkles, collisionPoint, Quaternion.identity);
            Destroy(spark, 3f);
            audioSource.PlayOneShot(damageSounds[Random.Range(0, damageSounds.Length)]);
        }
    }
    public void takeDamage()
    {
        hpSlider.value = currentHP; 
        if (currentHP > 0)
        {
            FindObjectOfType<HitmarkerEffect>().ShowHitmarker();
        }
        if (bodypartName != "Body")
        {
            currentHP--;
        }
        else
        {
            currentHP -= (1 - KjoyunBodyPart.defenseMultiplier);
        }
        hpSlider.value = currentHP;
    }
}
