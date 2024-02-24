using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KjoyunBodyPart : MonoBehaviour
{
    public int currentHP;
    public int maxHP;
    public string bodypartName;
    public bool destroyed;
    public GameObject hitSparkles;
    public GameObject fillRectToDisable;
    public Slider hpSlider;

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
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "PlayerAttack")
        {
            hpSlider.value = currentHP;
            Vector3 collisionPoint = collision.GetContact(0).point;
            GameObject spark = Instantiate(hitSparkles, collisionPoint, Quaternion.identity);
            Destroy(spark, 3f);
            if (currentHP > 0)
            {
                FindObjectOfType<HitmarkerEffect>().ShowHitmarker();
            }
            currentHP--;
        }
    }
}
