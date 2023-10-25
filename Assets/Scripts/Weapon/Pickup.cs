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
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (playerInventory.activeSlot != 2 && !playerInventory.inventorySlots[playerInventory.activeSlot])
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, pickupRange))
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
                                FindObjectOfType<Tooltip>().getReportedTo("Can't pick up a duplicate weapon!");
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
                                FindObjectOfType<Tooltip>().getReportedTo("Can't pick up a duplicate weapon!");
                                return; // Exit the method without destroying the duplicate weapon
                            }
                        }
                        else if (weaponName == "assaultRifle")
                        {
                            if (!playerInventory.IsWeaponInInventory(FindObjectOfType<Player>().assaultRifle.gameObject))
                            {
                                itemHolderWeapon = FindObjectOfType<Player>().assaultRifle.gameObject;
                                FindObjectOfType<Player>().assaultRifle.ammoInMag = this.GetComponent<AssaultRifle>().ammoInMag;
                            }
                            else
                            {
                                Debug.Log("Can't pick up duplicate weapon!");
                                FindObjectOfType<Tooltip>().getReportedTo("Can't pick up a duplicate weapon!");
                                return; // Exit the method without destroying the duplicate weapon
                            }
                        }
                        else if(weaponName == "BattleRifle")
                        {
                            if (!playerInventory.IsWeaponInInventory(FindObjectOfType<Player>().battleRifle.gameObject))
                            {
                                itemHolderWeapon = FindObjectOfType<Player>().battleRifle.gameObject;
                                FindObjectOfType<Player>().battleRifle.ammoInMag = this.GetComponent<BattleRifle>().ammoInMag;
                            }
                            else
                            {
                                Debug.Log("Can't pick up duplicate weapon!");
                                FindObjectOfType<Tooltip>().getReportedTo("Can't pick up a duplicate weapon!");
                                return; // Exit the method without destroying the duplicate weapon
                            }
                        }
                        else if (weaponName == "shotgun")
                        {
                            if (!playerInventory.IsWeaponInInventory(FindObjectOfType<Player>().shotgun.gameObject))
                            {
                                itemHolderWeapon = FindObjectOfType<Player>().shotgun.gameObject;
                                FindObjectOfType<Player>().shotgun.ammoInMag = this.GetComponent<Shotgun>().ammoInMag;
                            }
                            else
                            {
                                Debug.Log("Can't pick up duplicate weapon!");
                                FindObjectOfType<Tooltip>().getReportedTo("Can't pick up a duplicate weapon!");
                                return; // Exit the method without destroying the duplicate weapon
                            }
                        }
                        else if (weaponName == "DblBarrelShotgun")
                        {
                            if (!playerInventory.IsWeaponInInventory(FindObjectOfType<Player>().dblBarrelShotgun.gameObject))
                            {
                                itemHolderWeapon = FindObjectOfType<Player>().dblBarrelShotgun.gameObject;
                                FindObjectOfType<Player>().dblBarrelShotgun.ammoInMag = this.GetComponent<DblBarrelShotgun>().ammoInMag;
                            }
                            else
                            {
                                Debug.Log("Can't pick up duplicate weapon!");
                                FindObjectOfType<Tooltip>().getReportedTo("Can't pick up a duplicate weapon!");
                                return; // Exit the method without destroying the duplicate weapon
                            }
                        }
                        else if (weaponName == "uzi")
                        {
                            if (!playerInventory.IsWeaponInInventory(FindObjectOfType<Player>().uzi.gameObject))
                            {
                                itemHolderWeapon = FindObjectOfType<Player>().uzi.gameObject;
                                FindObjectOfType<Player>().uzi.ammoInMag = this.GetComponent<Uzi>().ammoInMag;
                            }
                            else
                            {
                                Debug.Log("Can't pick up duplicate weapon!");
                                FindObjectOfType<Tooltip>().getReportedTo("Can't pick up a duplicate weapon!");
                                return; // Exit the method without destroying the duplicate weapon
                            }
                        }
                        else if (weaponName == "grenadePistol")
                        {
                            if (!playerInventory.IsWeaponInInventory(FindObjectOfType<Player>().grenadePistol.gameObject))
                            {
                                itemHolderWeapon = FindObjectOfType<Player>().grenadePistol.gameObject;
                                FindObjectOfType<Player>().grenadePistol.ammoInMag = this.GetComponent<GrenadePistol>().ammoInMag;
                            }
                            else
                            {
                                Debug.Log("Can't pick up duplicate weapon!");
                                FindObjectOfType<Tooltip>().getReportedTo("Can't pick up a duplicate weapon!");
                                return; // Exit the method without destroying the duplicate weapon
                            }
                        }
                        else if (weaponName == "redDotRifle")
                        {
                            if (!playerInventory.IsWeaponInInventory(FindObjectOfType<Player>().redDotRifle.gameObject))
                            {
                                itemHolderWeapon = FindObjectOfType<Player>().redDotRifle.gameObject;
                                FindObjectOfType<Player>().redDotRifle.ammoInMag = this.GetComponent<AssaultRifle>().ammoInMag;
                            }
                            else
                            {
                                Debug.Log("Can't pick up duplicate weapon!");
                                FindObjectOfType<Tooltip>().getReportedTo("Can't pick up a duplicate weapon!");
                                return; // Exit the method without destroying the duplicate weapon
                            }
                        }
                        else if (weaponName == "singleShotRifle")
                        {
                            if (!playerInventory.IsWeaponInInventory(FindObjectOfType<Player>().singleShotRifle.gameObject))
                            {
                                itemHolderWeapon = FindObjectOfType<Player>().singleShotRifle.gameObject;
                                FindObjectOfType<Player>().singleShotRifle.ammoInMag = this.GetComponent<AssaultRifle>().ammoInMag;
                            }
                            else
                            {
                                Debug.Log("Can't pick up duplicate weapon!");
                                FindObjectOfType<Tooltip>().getReportedTo("Can't pick up a duplicate weapon!");
                                return; // Exit the method without destroying the duplicate weapon
                            }
                        }
                        else if (weaponName == "grapplingGun")
                        {
                            if (!playerInventory.IsWeaponInInventory(FindObjectOfType<Player>().grapplingGun))
                            {
                                itemHolderWeapon = FindObjectOfType<Player>().grapplingGun;
                            }
                            else
                            {
                                Debug.Log("Can't pick up duplicate weapon!");
                                FindObjectOfType<Tooltip>().getReportedTo("Can't pick up a duplicate weapon!");
                                return; // Exit the method without destroying the duplicate weapon
                            }
                        }
                        else if (weaponName == "plasmatana")
                        {
                            FindObjectOfType<Player>().hasPlasmatana = true;
                        }

                        if (itemHolderWeapon)
                        {
                            playerInventory.PickupWeapon(playerInventory.activeSlot, itemHolderWeapon);
                        }
                        Destroy(gameObject);
                    }
                }
            }
        }
    }
}
