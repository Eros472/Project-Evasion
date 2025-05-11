using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider slider;
    public Text healthText;

    public void SetMaxHealth(int health)
    {
        slider.maxValue = health;
        slider.value = health;
        UpdateHealthText(health, health);
    }

    public void SetHealth(int health)
    {
        slider.value = health;
        UpdateHealthText(health, (int)slider.maxValue);
    }

    private void UpdateHealthText(int current, int max)
    {
        if (healthText != null)
            healthText.text = current.ToString();
    }
}

