using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShocked : MonoBehaviour
{
    
     Animator animator;
    EnemyMain enemyMain;
    Rigidbody2D rb;
    
    [SerializeField] float stopTime;
    
    private void Awake()
    {
        enemyMain = GetComponent<EnemyMain>(); 
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }
    
    private void OnEnable()
    {
        if(spotCoroutine != null) {StopCoroutine(spotCoroutine);}
        spotCoroutine = StartCoroutine(SpotPlayer());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    void Update()
    {
        StopMovement();
    }


    Coroutine spotCoroutine;
    IEnumerator SpotPlayer()
    {
        float elapsedTime = 0;
        animator.Play("Shock");
        animator.speed = 1;
        
        yield return null;
        
        float lastFrameTime = Time.deltaTime;
        float time = animator.GetCurrentAnimatorStateInfo(0).length;
        while (elapsedTime < time - lastFrameTime)
        {
            elapsedTime += Time.deltaTime;
        
            yield return null;
        }

        enemyMain.currentState = EnemyState.Chase;
    }
    
    void StopMovement()
    {
        rb.velocity = Vector2.MoveTowards(rb.velocity, Vector2.zero, stopTime * Time.deltaTime);
    }
}
