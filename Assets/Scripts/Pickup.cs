using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    public GameObject itemHolderWeapon;
    public string weaponName;

    public float pickupRange = 3f;

    private PlayerInventory playerInventory;

    private void Start()
    {
        playerInventory = FindObjectOfType<PlayerInventory>();
        if (weaponName == "sword")
        {
            itemHolderWeapon = FindObjectOfType<Player>().sword;
        }
        else if(weaponName == "glock")
        {
            itemHolderWeapon = FindObjectOfType<Player>().glock.gameObject;
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && playerInventory.activeSlot != 2)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, pickupRange))
            {
                if (hit.collider.gameObject == gameObject)
                {
                    playerInventory.PickupWeapon(playerInventory.activeSlot, itemHolderWeapon);
                    Destroy(gameObject);
                }
            }
        }
    }
}
