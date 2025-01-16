using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyStunned : MonoBehaviour
{
    EnemyMain enemyMain;
    Animator animator;
    Rigidbody2D rb;

    [SerializeField] float stunDuration;
    [SerializeField] float stopTime;
    [SerializeField] ParticleSystem stunParticles;
    
    float elapsedTime = 0f;
    void Awake()
    {
        animator = GetComponent<Animator>();
        enemyMain = GetComponent<EnemyMain>();
        rb = GetComponent<Rigidbody2D>();
        
        elapsedTime = 0f;
    }

    private void OnEnable()
    {
        elapsedTime = 0f;
        
        if (hitRoutine != null) { StopCoroutine(hitRoutine); }
        hitRoutine = StartCoroutine(HitRoutine());
        
        stunParticles.Play();
        AudioManager.Instance.PlayPlayerSFX("Enemy_Hit", 0.6f, 0.8f, 1.2f);
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private void Update()
    {
        StopMovement();
    }

    void StopMovement()
    {
        rb.velocity = Vector2.MoveTowards(rb.velocity, Vector2.zero, stopTime * Time.deltaTime);
    }


    Coroutine hitRoutine;
    IEnumerator HitRoutine()
    {
        animator.Play("Hit");
        animator.speed = 1;
        elapsedTime = 0;
        
        yield return null;
        
        float lastFrameTime = Time.deltaTime;
        float time = animator.GetCurrentAnimatorStateInfo(0).length;

        while (elapsedTime < time - lastFrameTime)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        if (stunnedRoutine != null) { StopCoroutine(stunnedRoutine); }
        stunnedRoutine = StartCoroutine(StunnedRoutine());
    }

    
    Coroutine stunnedRoutine;
    IEnumerator StunnedRoutine()
    {
        animator.Play("Stunned");
        animator.speed = 1;
        elapsedTime = 0;
        
        yield return null;

        while (elapsedTime < stunDuration)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        enemyMain.currentState = EnemyState.Chase;
    }
    
    
    
    
    
    
    
    
    
    
}
