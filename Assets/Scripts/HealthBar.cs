using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider slider;
    public Text healthText;

    public void SetMaxHealth(int health)
    {
        Debug.Log("[HealthBar] SetMaxHealth called with: " + health);

        if (slider == null)
        {
            Debug.LogError("[HealthBar] SLIDER IS NULL!");
            return;
        }

        slider.maxValue = health;
        slider.value = health;
        UpdateHealthText(health, health);
    }


    public void SetHealth(int health)
    {
        if (slider == null)
        {
            Debug.LogError("[HealthBar] SLIDER IS NULL on SetHealth!");
            return;
        }

        slider.value = health;
        UpdateHealthText(health, (int)slider.maxValue);
    }

    private void UpdateHealthText(int current, int max)
    {
        if (healthText != null)
            healthText.text = current.ToString();
    }
}

