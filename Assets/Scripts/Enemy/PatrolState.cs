using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolState : MonoBehaviour
{
    Rigidbody2D rb;
    Animator animator;
    EnemyMain enemyMain;
    
    [SerializeField] private GameObject visuals;
    
    [Header("Patrol Settings")]
    [SerializeField] float patrolSpeed = 2f;
    [SerializeField] float patrolTime = 0.5f;
    [SerializeField] float idleTime = 2f;
    
    [SerializeField] bool isIdle = false;

    [Header("Edge Checks")]
    [SerializeField] Collider2D edgeCheckerR;
    [SerializeField] Collider2D edgeCheckerL;
    [SerializeField] LayerMask ground;
    void Awake()
    {
       rb = GetComponent<Rigidbody2D>(); 
       animator = GetComponent<Animator>();
       enemyMain = GetComponent<EnemyMain>();
    }

    
    Coroutine patrolRoutine;
    private void OnEnable()
    {
        if (patrolRoutine != null) {StopCoroutine(patrolRoutine);}
        patrolRoutine = StartCoroutine(Patrol());
    }

    void OnDisable()
    {
        StopCoroutine(patrolRoutine);
    }


    float elapsedTime = 0f;
    IEnumerator Patrol()
    {
        while (true)
        {
            elapsedTime = 0f;
            while (elapsedTime < patrolTime && !isIdle)
            {
                elapsedTime += Time.deltaTime;
                HandleDirectionChange();
                rb.velocity = new Vector2(patrolSpeed * Mathf.Sign(visuals.transform.localScale.x) , rb.velocity.y);
                animator.Play("Run");
                animator.speed = 0.6f;
                yield return new WaitForEndOfFrame();
            }
        
            elapsedTime = 0f;
            while (elapsedTime < idleTime)
            {
                elapsedTime += Time.deltaTime;
                rb.velocity = new Vector2(0f, rb.velocity.y);
                animator.Play("Idle");
                animator.speed = 1f;
                yield return new WaitForEndOfFrame();
            }
        }
        
    }


    bool noEdgeR; 
    bool noEdgeL;

    bool wallR;
    bool wallL;
    
    void HandleDirectionChange()
    {
        noEdgeL = !Physics2D.OverlapArea(edgeCheckerL.bounds.min, edgeCheckerL.bounds.max, ground);
        noEdgeR = !Physics2D.OverlapArea(edgeCheckerR.bounds.min, edgeCheckerR.bounds.max, ground);
        
        wallL = Physics2D.Raycast(edgeCheckerL.bounds.center, Vector2.left, 0.1f, ground);
        wallR = Physics2D.Raycast(edgeCheckerR.bounds.center, Vector2.right, 0.1f, ground);

        if (noEdgeL || wallL)
        {
            visuals.transform.localScale = new Vector3(1f, 1f, 1f);
        }
        else if (noEdgeR || wallR)
        {
            visuals.transform.localScale = new Vector3(-1f, 1f, 1f);
        }
    }
}



















