using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int health = 100;

    // Method to reduce health when damage is taken
    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            // Handle player death (e.g., show game over screen)
            Debug.Log("Player is dead!");
            // Optional: trigger death animation or disable player
        }
    }
}

