using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public PlayerInventory inv;

    public RawImage slot1;
    public RawImage slot2;
    public RawImage slot3;

    public Texture2D BattleRifle;
    public Texture2D M9Suppressed;
    public Texture2D Glock;
    public Texture2D AssaultRifle;
    public Texture2D Shotgun;
    public Texture2D DblBarrelShotgun;
    public Texture2D Uzi;
    public Texture2D GrenPistol;
    public Texture2D Plasmatana;
    public Texture2D Fist;

    public GameObject helpText;
    public static bool shouldShowHelpText;

    Dictionary<GameObject, Texture2D> gameObjectToTexture;


    // Start is called before the first frame update
    void Start()
    {
        inv = FindObjectOfType<PlayerInventory>();

        gameObjectToTexture = new Dictionary<GameObject, Texture2D>()
        {
    { inv.battleRifle.gameObject, BattleRifle },
    { inv.m9suppressed.gameObject, M9Suppressed },
    { inv.glock.gameObject, Glock },
    { inv.assaultRifle.gameObject, AssaultRifle },
    { inv.shotgun.gameObject, Shotgun},
    { inv.dblBarrelShotgun.gameObject, DblBarrelShotgun},
    { inv.uzi.gameObject, Uzi},
    { inv.grenadePistol.gameObject, GrenPistol},
        };

        if(PlayerPrefs.GetInt("ShowHelpText", 1) == 1)
        {
            shouldShowHelpText = true;
            helpText.gameObject.SetActive(true);
        }
        else
        {
            shouldShowHelpText = false;
            helpText.gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            shouldShowHelpText = !shouldShowHelpText;


            if (shouldShowHelpText)
            {
                PlayerPrefs.SetInt("ShowHelpText", 1);
                helpText.gameObject.SetActive(true);
            }
            else
            {
                PlayerPrefs.SetInt("ShowHelpText", 0);
                helpText.gameObject.SetActive(false);
            }
        }

        if (inv != null && inv.inventorySlots.Length > 0 && inv.inventorySlots[0] != null && inv.inventorySlots[0].gameObject != null)
        {
            if (gameObjectToTexture.ContainsKey(inv.inventorySlots[0].gameObject))
            {
                slot1.texture = gameObjectToTexture[inv.inventorySlots[0].gameObject];
            }
            else
            {
                slot1.texture = Fist;
            }
        }
        else
        {
            slot1.texture = Fist;
        }

        if (inv != null && inv.inventorySlots.Length > 0 && inv.inventorySlots[1] != null && inv.inventorySlots[1].gameObject != null)
        {
            if (gameObjectToTexture.ContainsKey(inv.inventorySlots[1].gameObject))
            {
                slot2.texture = gameObjectToTexture[inv.inventorySlots[1].gameObject];
            }
            else
            {
                slot2.texture = Fist;
            }
        }
        else
        {
            slot2.texture = Fist;
        }

        switch (inv.activeSlot)
        {
            case 0:
                SetTransparency(slot1, 1f);
                SetTransparency(slot2, 0.5f);
                SetTransparency(slot3, 0.5f);
                break;
            case 1:
                SetTransparency(slot1, 0.5f);
                SetTransparency(slot2, 1f);
                SetTransparency(slot3, 0.5f);
                break;
            case 2:
                SetTransparency(slot1, 0.5f);
                SetTransparency(slot2, 0.5f);
                SetTransparency(slot3, 1f);
                break;
            default:
                break;
        }
    }

    void SetTransparency(RawImage image, float alpha)
    {
        Color color = image.color;
        color.a = alpha;
        image.color = color;
    }
}
