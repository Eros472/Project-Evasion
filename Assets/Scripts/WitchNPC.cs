using UnityEngine;

public class WitchNPC : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();

        // Trigger idle animation if not default
        //animator.SetTrigger("Idle");
        // Optional: Only use this if Idle is not the default
    }
}
