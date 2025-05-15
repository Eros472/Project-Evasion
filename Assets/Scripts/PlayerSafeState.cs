using UnityEngine;

public class PlayerSafeState : MonoBehaviour
{
    private int safeZoneCounter = 0;

    public bool IsInSafeZone => safeZoneCounter > 0;

    public void IncrementSafeZone()
    {
        safeZoneCounter++;
    }

    public void DecrementSafeZone()
    {
        safeZoneCounter = Mathf.Max(0, safeZoneCounter - 1);
    }
}

