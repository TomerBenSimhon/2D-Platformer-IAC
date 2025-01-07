using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShocked : MonoBehaviour
{
    
     Animator animator;
    EnemyMain enemyMain;
    
    private void Awake()
    {
        enemyMain = GetComponent<EnemyMain>(); 
        animator = GetComponent<Animator>();
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
}
