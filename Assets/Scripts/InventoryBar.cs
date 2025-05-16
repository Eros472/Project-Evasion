using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class InventoryBar : MonoBehaviour
{
    public Image[] slots; // Assign 4 slot Images in Inspector
    public Sprite daggerIcon;
    public Sprite bowIcon;

    public Color normalColor = Color.white;
    public Color selectedColor = Color.yellow;

    private int currentIndex = 0;
    private Dictionary<WeaponType, Sprite> iconLookup;

    void Start()
    {
        iconLookup = new Dictionary<WeaponType, Sprite>
        {
            { WeaponType.Dagger, daggerIcon },
            { WeaponType.Bow, bowIcon }
        };

        ClearSlots(); // Optional: clear at startup
        UpdateSlotHighlight();
    }

    /*void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) SelectSlot(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SelectSlot(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) SelectSlot(2);
        if (Input.GetKeyDown(KeyCode.Alpha4)) SelectSlot(3);
    }*/

    public void SelectSlot(int index)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].color = (i == index) ? selectedColor : normalColor;
        }

        currentIndex = index;
    }


    public void ClearSelectedSlot()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].color = normalColor;
        }

        currentIndex = -1; // no slot selected
    }




    void UpdateSlotHighlight()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].color = (i == currentIndex) ? selectedColor : normalColor;
        }
    }

    public int GetSelectedSlot() => currentIndex;

    public void AddWeaponIcon(int slotIndex, WeaponType weapon)
    {
        if (slotIndex >= 0 && slotIndex < slots.Length && iconLookup.ContainsKey(weapon))
        {
            slots[slotIndex].sprite = iconLookup[weapon];
            slots[slotIndex].enabled = true;
            Debug.Log("Set icon for slot " + slotIndex + ": " + weapon);
        }
        else
        {
            Debug.LogWarning("Slot index or icon missing for weapon: " + weapon);
        }
    }

    public void SetSlotSprite(int index, Sprite icon)
    {
        if (index >= 0 && index < slots.Length && icon != null)
        {
            slots[index].sprite = icon;
            slots[index].enabled = true;
            slots[index].color = Color.white; // ✅ ensure visibility

            Debug.Log("ICON SET: " + icon.name);
            Debug.Log("Slot sprite after set: " + slots[index].sprite?.name);
            Debug.Log("Slot enabled: " + slots[index].enabled);
            Debug.Log("Slot color: " + slots[index].color);
        }
        else
        {
            Debug.LogWarning($"SetSlotSprite failed: index={index}, icon={(icon != null ? icon.name : "null")}, slots.Length={slots.Length}");
        }
    }




    private void ClearSlots()
    {
        foreach (var img in slots)
        {
            img.sprite = null;
            img.enabled = false;
        }
    }
}
