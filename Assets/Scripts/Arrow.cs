using UnityEngine;

public class Arrow : MonoBehaviour
{
    public int damage = 1;
    public float lifeTime = 5f;

    private void Start()
    {
        Destroy(gameObject, lifeTime); // Auto-destroy after time
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Arrow hit something: " + other.name);

        if (other.CompareTag("Player"))
        {
            Destroy(other.gameObject);
            Debug.Log("Player was hit and destroyed by an arrow!");
        }

        Destroy(gameObject);
    }


}

