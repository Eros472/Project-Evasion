using System;
using UnityEngine;

public class Fireball
{
    public Vector3 position;
    public float orbitDistance;
    public float speed;
    public int damage;

    public Fireball(float orbitDistance, float speed, int damage)
    {
        this.orbitDistance = orbitDistance;
        this.speed = speed;
        this.damage = damage;
    }

    public void UpdateOrbit(Vector3 bossPosition, float time)
    {
        float t = time * speed;
        position = bossPosition + new Vector3(
            -Mathf.Cos(t) * orbitDistance,
            Mathf.Sin(t) * orbitDistance,
            0
        );
    }

    public void TryHitPlayer(Vector3 playerPos, float hitRadius, Action<int> damageCallback)
    {
        if (Vector3.Distance(position, playerPos) < hitRadius)
        {
            damageCallback?.Invoke(damage);
        }
    }
}

