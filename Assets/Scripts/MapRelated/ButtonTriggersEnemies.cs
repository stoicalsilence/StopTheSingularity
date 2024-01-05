using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonTriggersEnemies : MonoBehaviour
{
    public List<Killbot> killbots;
    public List<Eyenemy> puters;
    public List<FlyingPuter> flyingPuters;
    public List<RollingEnemy> rollers;
    public PuteyBoss puteyBoss;
    bool started;
    public OpenDoorButton button;
    public float delay;
    void Update()
    {
        if (button.isPressed && !started)
        {
            started = true;
            Invoke("triggerEnemies", delay);
        }
    }

    public void triggerEnemies()
    {
            foreach (Killbot bot in killbots)
            {
            bot.getTriggered();
            }
            foreach (Eyenemy bot in puters)
            {
                bot.isTriggered = true;
            }
            foreach (FlyingPuter bot in flyingPuters)
            {
                bot.isTriggered = true;
            }
            foreach (RollingEnemy roller in rollers)
            {
            if(roller)
            roller.getTriggered();
            }
            if(puteyBoss != null)
                puteyBoss.startBossFight();
    }
}
