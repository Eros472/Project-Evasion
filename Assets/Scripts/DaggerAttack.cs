using System.Collections;
using UnityEngine;

public class DaggerAttack : MonoBehaviour
{
    public GameObject hitbox; // Your DaggerHitbox
    public Transform daggerModel; // Drag your dagger model (sprite) here
    public float attackDuration = 0.2f;
    public float swingAngle = 90f;

    public void PerformAttack()
    {
        StartCoroutine(Swing());
    }

    private IEnumerator Swing()
    {
        // Activate hitbox
        hitbox.SetActive(true);

        // Rotate dagger
        float elapsed = 0f;
        float totalTime = attackDuration;
        float halfTime = totalTime / 2f;

        while (elapsed < totalTime)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / totalTime;

            // Swing forward and then back
            float angle = (t < 0.5f)
                ? Mathf.Lerp(0, swingAngle, t * 2)
                : Mathf.Lerp(swingAngle, 0, (t - 0.5f) * 2);

            daggerModel.localRotation = Quaternion.Euler(0, 0, angle);
            yield return null;
        }

        // Reset rotation and disable hitbox
        daggerModel.localRotation = Quaternion.identity;
        hitbox.SetActive(false);
    }
}
