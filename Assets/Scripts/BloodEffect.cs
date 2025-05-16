using UnityEngine;

public class BloodEffect : MonoBehaviour
{
    public float destroyDelay = 1f;
    public SpriteRenderer spriteRenderer;

    private void Start()
    {
        Destroy(gameObject, destroyDelay);
    }

    public void SetColor(Color color)
    {
        if (spriteRenderer != null)
        {
            if (spriteRenderer.material.HasProperty("_Color"))
            {
                spriteRenderer.material.SetColor("_Color", color);
            }
            else
            {
                Debug.LogWarning("[BloodEffect] Material missing _Color property.");
            }
        }
        else
        {
            Debug.LogWarning("[BloodEffect] SpriteRenderer is not assigned.");
        }
    }
}
