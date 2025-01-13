using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] int health = 100;
    
    public bool isDead = false;
    
    HealthBar healthBar;

    [SerializeField] bool isPlayer;

    void Start()
    {
        if (isPlayer)
        {
            healthBar = FindObjectOfType<HealthBar>();
            healthBar.SetMaxHealth(health);
        }
    }
    

    public void TakeDamage(int damage)
    {
        health -= damage;
        
        if (isPlayer)
        {
            healthBar.SetHealth(health);
        }
        
        if (health <= 0)
        {
            isDead = true;
        }
    }
    

  
}
