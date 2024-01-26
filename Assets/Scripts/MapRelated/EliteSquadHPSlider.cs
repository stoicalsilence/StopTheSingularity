using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EliteSquadHPSlider : MonoBehaviour
{
    public Slider slider;
    public Soldier[] eliteSoldiers;
    public int maxHP;

    public AudioSource auds;
    public AudioClip bossmusic;
    bool musicstarted = false;
    public GameObject turnoffbgm;
    void Start()
    {
        calcMaxHP();
    }

    // Update is called once per frame
    void Update()
    {
        CalcHP();
        if(slider.value == 0)
        {
            if (maxHP > 0) { auds.PlayOneShot(auds.clip); }
            Destroy(slider.gameObject);
            Destroy(slider);
            Destroy(this.gameObject);
        }
        if (areSoldiersTriggered())
        {
            slider.gameObject.SetActive(true);
            if (!musicstarted)
            {
                auds.clip = bossmusic; Destroy(GameObject.Find("BGM").GetComponent<DoNotDestroyOnLoad>());
                Destroy(GameObject.Find("BGM").gameObject); auds.loop = true; auds.volume = 0.33f; auds.Play(); musicstarted = true;
            }
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

    

    void CalcHP()
    {
        int hp = 0;

        foreach (Soldier sol in eliteSoldiers)
        {
            if (sol.isDead == false && sol.health > 0)
            {
                hp+= sol.health;
            }
        }
        slider.value = hp;
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
