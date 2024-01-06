using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartMusicOnAlert : MonoBehaviour
{
    public Soldier[] sols;
    public AudioSource music;
    void Start()
    {
        sols = FindObjectsOfType<Soldier>();
    }

    // Update is called once per frame
    void Update()
    {
        foreach(Soldier sol in sols)
        {
            if (sol.triggered)
            {
                music.Play();
                Destroy(this);
            }
        }
    }
}
