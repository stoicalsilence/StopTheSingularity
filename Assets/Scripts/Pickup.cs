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
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && playerInventory.activeSlot != 2 && !playerInventory.inventorySlots[playerInventory.activeSlot])
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, pickupRange))
            {
                if (hit.collider.gameObject == gameObject)
                {
                    if (weaponName == "sword")
                    {
                        if (!playerInventory.IsWeaponInInventory(FindObjectOfType<Player>().sword))
                        {
                            itemHolderWeapon = FindObjectOfType<Player>().sword;
                        }
                        else
                        {
                            Debug.Log("Can't pick up duplicate weapon!");
                            return; // Exit the method without destroying the duplicate weapon
                        }
                    }
                    else if (weaponName == "glock")
                    {
                        if (!playerInventory.IsWeaponInInventory(FindObjectOfType<Player>().glock.gameObject))
                        {
                            itemHolderWeapon = FindObjectOfType<Player>().glock.gameObject;
                            FindObjectOfType<Player>().glock.ammoInMag = this.GetComponent<Firearm>().ammoInMag;
                        }
                        else
                        {
                            Debug.Log("Can't pick up duplicate weapon!");
                            return; // Exit the method without destroying the duplicate weapon
                        }
                    }
                    playerInventory.PickupWeapon(playerInventory.activeSlot, itemHolderWeapon);
                    Destroy(gameObject);
                }
            }
        }
    }

    
}
