using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int maxHealth = 20;
    public int currentHealth;

    public InventorySlot[] inventory = new InventorySlot[4];
    private int selectedSlot = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            InitializeInventory();

            if (currentHealth <= 0)
                currentHealth = maxHealth;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeInventory()
    {
        for (int i = 0; i < inventory.Length; i++)
        {
            inventory[i] = new InventorySlot();
        }

        inventory[0].itemType = InventoryItemType.Dagger;
        inventory[0].quantity = 1;
    }

    public void SetSelectedSlot(int index)
    {
        selectedSlot = Mathf.Clamp(index, 0, inventory.Length - 1);
    }

    public int GetSelectedSlot()
    {
        return selectedSlot;
    }

    public InventorySlot GetInventorySlot(int index)
    {
        if (index < 0 || index >= inventory.Length)
            return new InventorySlot();

        return inventory[index];
    }

    public InventoryItemType GetSelectedItemType()
    {
        return inventory[selectedSlot].itemType;
    }

    public void EndGame(string sceneName = "Victory", float delay = 2f)
    {
        Debug.Log("Game over. Loading: " + sceneName);
        StartCoroutine(LoadSceneWithDelay(sceneName, delay));
    }

    private IEnumerator LoadSceneWithDelay(string sceneName, float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(sceneName);
    }
}




