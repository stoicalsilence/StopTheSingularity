using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KjoyunBodyPart : MonoBehaviour
{
    public int currentHP;
    public int maxHP;
    public string bodypartName;
    public bool destroyed;

    private void Start()
    {
        currentHP = maxHP;
    }
    void Update()
    {
        if(currentHP < 0)
        {
            destroyed = true;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "PlayerAttack")
        {
            Debug.Log(bodypartName + " HIT!");
            currentHP--;
        }
    }
}
