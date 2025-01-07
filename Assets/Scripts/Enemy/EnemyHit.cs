using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHit : MonoBehaviour
{
    EnemyMain enemyMain;
    Animator animator;
    Rigidbody2D rb;
    
    [SerializeField] float stopTime;

    float elapsedTime;
    
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

        enemyMain.currentState = EnemyState.Chase;
    }
}
