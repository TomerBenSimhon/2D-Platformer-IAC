using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] int health = 100;

    private Rigidbody2D rb;
    Animator animator;

    [SerializeField] GameObject[] objectsDestroyOnDeath;
    [SerializeField] MonoBehaviour[] componentsDestroyOnDeath;
    
    public bool isDead = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            isDead = true;
            Die();
        }
    }

    public void TakeKnockback(Vector2 direction, float knockbackForce)
    {
        rb.velocity = direction * knockbackForce;
    }

    void Die()
    {
        foreach (GameObject obj in objectsDestroyOnDeath)
        {
            obj.SetActive(false);
        }

        foreach (MonoBehaviour mono in componentsDestroyOnDeath)
        {
            mono.enabled = false;
        }
    }
}
