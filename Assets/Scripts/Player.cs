using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Player : Mover
{
    private bool isAlive = true;

    protected override void Start()
    {
        base.Start();
    }

    protected override void ReceiveDamage(Damage dmg)
    {
        if(!isAlive)
            return;
            
        base.ReceiveDamage(dmg);    
        GameManager.instance.OnHitpointChange();
    }

    protected override void Death()
    {
        //GameManager.instance.deathMenuAnim.SetTrigger("Show");
    }
    
    private void FixedUpdate()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        
        if(isAlive)
            UpdateMotor(new Vector3(x, y, 0));
    }

    public void Respawn()
    {
        hitpoint = maxHitpoint;
        isAlive = true;
        lastImmune = Time.time;
    }
}

