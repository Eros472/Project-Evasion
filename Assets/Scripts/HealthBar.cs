using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider slider;
    public Text healthText;

    private void Start()
    {
        if (GameManager.Instance != null)
        {
            SetMaxHealth(GameManager.Instance.maxHealth);
            SetHealth(GameManager.Instance.currentHealth);
        }
    }

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
            healthText.text = current.ToString(); // or $"{current} / {max}"
    }
}


