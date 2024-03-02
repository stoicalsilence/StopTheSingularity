using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public Player player;

    [Header("Weapon Scripts")]
    public GameObject grapplingGun;
    public GameObject sword;
    public Firearm glock;
    public GameObject icePick;
    public AssaultRifle assaultRifle;
    public AssaultRifle redDotRifle;
    public AssaultRifle singleShotRifle;
    public Shotgun shotgun;
    public DblBarrelShotgun dblBarrelShotgun;
    public Uzi uzi;
    public GrenadePistol grenadePistol;
    public BattleRifle battleRifle;
    public GrenadeWeapon grenade;
    public Firearm m9suppressed;

    [Header("Pickup References")]
    public GameObject swordPickup;
    public GameObject grapplingGunPickup;
    public GameObject glockPickup;
    public GameObject icePickPickup;
    public GameObject assaultRiflePickup;
    public GameObject redDotRiflePickup;
    public GameObject singleShotRiflePickup;
    public GameObject shotgunPickup;
    public GameObject dblBarrelShotgunPickup;
    public GameObject uziPickup;
    public GameObject grenadePistolPickup;
    public GameObject battleRiflePickup;
    public GameObject grenadePickup;
    public GameObject m9suppressedPickup;

    [Header("Slots")]
    public int activeSlot;
    public int totalInventorySlots = 3;
    public GameObject[] inventorySlots;

    public bool weaponSwitchBlocked;
    void Start()
    {
        // Initialize the inventory slots array
        inventorySlots = new GameObject[totalInventorySlots];
        player = GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!weaponSwitchBlocked)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                int current = activeSlot;
                activeSlot = 0;
                SwitchWeapon(current, activeSlot);
            }

            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                int current = activeSlot;
                activeSlot = 1;
                SwitchWeapon(current, activeSlot);
            }

            if (Input.GetKeyDown(KeyCode.Alpha3)) //Hands, Holstered
            {
                int current = activeSlot;
                activeSlot = 2;
                SwitchWeapon(current, activeSlot);
            }

            if (Input.GetKeyDown(KeyCode.Q))
            {
                DropWeapon(activeSlot);
            }

            // Check for mouse scroll input
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scroll < 0f)
            {
                activeSlot = (activeSlot + 1) % totalInventorySlots;
                SwitchWeapon((activeSlot - 1 + totalInventorySlots) % totalInventorySlots, activeSlot);
            }
            else if (scroll > 0f)
            {
                activeSlot = (activeSlot - 1 + totalInventorySlots) % totalInventorySlots;
                SwitchWeapon((activeSlot + 1) % totalInventorySlots, activeSlot);
            }
        }
    }

    public void PickupWeapon(int slotIndex, GameObject item)
    {
        if(slotIndex != 2 && !inventorySlots[slotIndex])
        {
            inventorySlots[slotIndex] = item.gameObject;
            SwitchWeapon(activeSlot, slotIndex);
        }
    }

    public void DropWeapon(int slotIndex)
    {
        if(slotIndex != 2)
        {
            if (inventorySlots[slotIndex])
            {
                GameObject droppedWeapon = inventorySlots[slotIndex];
                inventorySlots[slotIndex] = null;

                GameObject pickupPrefab = GetPickupPrefab(droppedWeapon);
                if (pickupPrefab != null)
                {
                    GameObject spawnedPickup = Instantiate(pickupPrefab, droppedWeapon.transform.position, droppedWeapon.transform.rotation);
                    if (droppedWeapon.GetComponent<Firearm>())
                    {
                        spawnedPickup.GetComponent<Firearm>().ammoInMag = droppedWeapon.GetComponent<Firearm>().ammoInMag;
                    }
                    else if (droppedWeapon.GetComponent<AssaultRifle>())
                    {
                        spawnedPickup.GetComponent<AssaultRifle>().ammoInMag = droppedWeapon.GetComponent<AssaultRifle>().ammoInMag;
                    }
                    else if (droppedWeapon.GetComponent<BattleRifle>())
                    {
                        spawnedPickup.GetComponent<BattleRifle>().ammoInMag = droppedWeapon.GetComponent<BattleRifle>().ammoInMag;
                    }
                    else if (droppedWeapon.GetComponent<Shotgun>())
                    {
                        spawnedPickup.GetComponent<Shotgun>().ammoInMag = droppedWeapon.GetComponent<Shotgun>().ammoInMag;
                    }
                    else if (droppedWeapon.GetComponent<DblBarrelShotgun>())
                    {
                        spawnedPickup.GetComponent<DblBarrelShotgun>().ammoInMag = droppedWeapon.GetComponent<DblBarrelShotgun>().ammoInMag;
                    }
                    else if (droppedWeapon.GetComponent<Uzi>())
                    {
                        spawnedPickup.GetComponent<Uzi>().ammoInMag = droppedWeapon.GetComponent<Uzi>().ammoInMag;
                    }
                    else if (droppedWeapon.GetComponent<GrenadePistol>())
                    {
                        spawnedPickup.GetComponent<GrenadePistol>().ammoInMag = droppedWeapon.GetComponent<GrenadePistol>().ammoInMag;
                    }
                    Rigidbody pickupRigidbody = spawnedPickup.GetComponent<Rigidbody>();
                    if (pickupRigidbody != null)
                    {
                        float forceMagnitude = 5f;
                        float torqueMagnitude = 2f;
                        pickupRigidbody.AddForce(transform.forward * forceMagnitude, ForceMode.Impulse);
                        pickupRigidbody.AddTorque(Random.insideUnitSphere * torqueMagnitude, ForceMode.Impulse);
                    }
                }
                player.unequipSword();
                player.unequipGlock();
                player.unequipIcePick();
                player.unequipAssaultRifle();
                player.unequipShotgun();
                player.unequipDblBarrelShotgun();
                player.unequipUzi();
                player.unequipGrenadePistol();
                player.unequipGrapplingGun();
                player.unequipRedDotRifle();
                player.unequipSingleShotRifle();
                player.unequipBattleRifle();
                player.unequipGrenade();
                player.unequipM9Suppressed();
                droppedWeapon.gameObject.SetActive(false);
            }
        }
    }

    public void SwitchWeapon(int oldSlot, int newSlot)
    {
        if (inventorySlots[oldSlot])
        {
            inventorySlots[oldSlot].SetActive(false);
        }
        if (inventorySlots[newSlot])
        {
            inventorySlots[newSlot].SetActive(true);
        }

        activeSlot = newSlot;

        if (inventorySlots[activeSlot] && inventorySlots[activeSlot].gameObject == sword)
        {
            player.equipSword();
        }
        else
        {
            player.unequipSword();
        }

        if (inventorySlots[activeSlot] && inventorySlots[activeSlot].gameObject == glock.gameObject)
        {
            player.equipGlock();
        }
        else
        {
            player.unequipGlock();
        }

        if (inventorySlots[activeSlot] && inventorySlots[activeSlot].gameObject == assaultRifle.gameObject)
        {
            player.equipAR();
        }
        else
        {
            player.unequipAssaultRifle();
        }

        if (inventorySlots[activeSlot] && inventorySlots[activeSlot].gameObject == battleRifle.gameObject)
        {
            player.equipBattleRifle();
        }
        else
        {
            player.unequipBattleRifle();
        }

        if (inventorySlots[activeSlot] && inventorySlots[activeSlot].gameObject == shotgun.gameObject)
        {
            player.equipShotgun();
        }
        else
        {
            player.unequipShotgun();
        }

        if (inventorySlots[activeSlot] && inventorySlots[activeSlot].gameObject == dblBarrelShotgun.gameObject)
        {
            player.equipDblBarrelShotgun();
        }
        else
        {
            player.unequipDblBarrelShotgun();
        }

        if (inventorySlots[activeSlot] && inventorySlots[activeSlot].gameObject == uzi.gameObject)
        {
            player.equipUzi();
        }
        else
        {
            player.unequipUzi();
        }

        if (inventorySlots[activeSlot] && inventorySlots[activeSlot].gameObject == grenadePistol.gameObject)
        {
            player.equipGrenadePistol();
        }
        else
        {
            player.unequipGrenadePistol();
        }

        if (inventorySlots[activeSlot] && inventorySlots[activeSlot].gameObject == redDotRifle.gameObject)
        {
            player.equipRedDotAR();
        }
        else
        {
            player.unequipRedDotRifle();
        }

        if (inventorySlots[activeSlot] && inventorySlots[activeSlot].gameObject == singleShotRifle.gameObject)
        {
            player.equipScopeAR();
        }
        else
        {
            player.unequipSingleShotRifle();
        }

        if(inventorySlots[activeSlot] && inventorySlots[activeSlot].gameObject == grenade.gameObject)
        {
            player.equipGrenade();
        }
        else
        {
            player.unequipGrenade();
        }

        if (inventorySlots[activeSlot] && inventorySlots[activeSlot].gameObject == m9suppressed.gameObject)
        {
            player.equipM9Suppressed();
        }
        else
        {
            player.unequipM9Suppressed();
        }
    } 

    public bool IsWeaponInInventory(GameObject weapon)
    {
        foreach (GameObject inventoryItem in inventorySlots)
        {
            if (inventoryItem && inventoryItem == weapon)
            {
                return true; // Weapon is already in the inventory
            }
        }
        return false; // Weapon is not in the inventory
    }

    private GameObject GetPickupPrefab(GameObject weapon)
    {
        if (weapon == grapplingGun)
        {
            return grapplingGunPickup;
        }
        else if (weapon == sword)
        {
            return swordPickup;
        }
        else if (weapon == glock.gameObject)
        {
            return glockPickup;
        }
        else if (weapon == icePick)
        {
            return icePickPickup;
        }
        else if (weapon == assaultRifle.gameObject)
        {
            return assaultRiflePickup;
        }
        else if (weapon == redDotRifle.gameObject)
        {
            return redDotRiflePickup;
        }
        else if (weapon == singleShotRifle.gameObject)
        {
            return singleShotRiflePickup;
        }
        else if (weapon == battleRifle.gameObject)
        {
            return battleRiflePickup;
        }
        else if (weapon == shotgun.gameObject)
        {
            return shotgunPickup;
        }
        else if(weapon == dblBarrelShotgun.gameObject)
        {
            return dblBarrelShotgunPickup;
        }
        else if (weapon == uzi.gameObject)
        {
            return uziPickup;
        }
        else if (weapon == grenadePistol.gameObject)
        {
            return grenadePistolPickup;
        }
        else if (weapon == grenade.gameObject)
        {
            return grenadePickup;
        }
        else if(weapon == m9suppressed.gameObject)
        {
            return m9suppressedPickup;
        }

        return null;
    }
}

