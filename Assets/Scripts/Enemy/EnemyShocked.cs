using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShocked : MonoBehaviour, IEnemyState
{
    public EnemyState StateId => EnemyState.Shocked;
    public void Enter() => enabled = true;
    public void Exit() => enabled = false;
    
    [SerializeField] float stopTime;
    
    Animator animator;
    EnemyMain enemyMain;
    Rigidbody2D rb;
    
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

        enemyMain.ChangeState(EnemyState.Chase);
    }
    
    void StopMovement()
    {
        rb.velocity = Vector2.MoveTowards(rb.velocity, Vector2.zero, stopTime * Time.deltaTime);
    }
}
