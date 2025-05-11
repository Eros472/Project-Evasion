using UnityEngine;
using UnityEngine.UI;

public class InventoryBar : MonoBehaviour
{
    public Image[] slots; // Assign 4 slot Images in Inspector
    public Color normalColor = Color.white;
    public Color selectedColor = Color.yellow;

    private int currentIndex = 0;

    void Start()
    {
        UpdateSlotHighlight();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) SelectSlot(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SelectSlot(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) SelectSlot(2);
        if (Input.GetKeyDown(KeyCode.Alpha4)) SelectSlot(3);
    }

    void SelectSlot(int index)
    {
        currentIndex = index;
        UpdateSlotHighlight();
    }

    void UpdateSlotHighlight()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].color = (i == currentIndex) ? selectedColor : normalColor;
        }
    }

    public int GetSelectedSlot()
    {
        return currentIndex;
    }
}
