using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    public Animator swordAnim;
    public bool slashOnCD;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && !slashOnCD)
        {
            StartCoroutine(swordAttack());
        }
    }

    IEnumerator swordAttack()
    {
        slashOnCD = true;
        swordAnim.SetBool("isAttacking", true);
        yield return new WaitForSeconds(0.30f);
        swordAnim.SetBool("isAttacking", false);
        slashOnCD = false;
    }
}
