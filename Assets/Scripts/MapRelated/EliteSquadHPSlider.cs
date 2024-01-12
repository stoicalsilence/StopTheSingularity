using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EliteSquadHPSlider : MonoBehaviour
{
    public Slider slider;
    public Soldier[] eliteSoldiers;
    public int maxHP;
    void Start()
    {
        calcMaxHP();
    }

    // Update is called once per frame
    void Update()
    {
        setHP();
        if(slider.value < 1)
        {
            slider.gameObject.SetActive(false);
            Destroy(this.gameObject);
        }
        if (areSoldiersTriggered())
        {
            slider.gameObject.SetActive(true);
        }
        else
        {
            slider.gameObject.SetActive(false);
        }
    }

    void calcMaxHP()
    {
        foreach(Soldier sol in eliteSoldiers)
        {
            maxHP += sol.health;
            slider.maxValue = maxHP;
        }
    }

    void setHP()
    {
        slider.value = 0;

        foreach(Soldier sol in eliteSoldiers)
        {
            if(sol.isDead == false && sol.health > 0)
            {
                slider.value += sol.health;
            }
        }
    }

    bool areSoldiersTriggered()
    {
        foreach(Soldier sol in eliteSoldiers)
        {
            if (sol.triggered) return true;
        }

        return false;
    }
}
