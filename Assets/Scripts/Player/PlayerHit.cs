using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHit : MonoBehaviour
{
    [SerializeField] Animator playerAnimator;

    PlayerMain playerMain;

   
    void Awake()
    {
        playerMain = GetComponent<PlayerMain>();
        elapsedTime = 0;
    }

    private void OnEnable()
    {
        elapsedTime = 0;
        StartCoroutine(HitRoutine());
    }

    private void Update()
    {
        Debug.Log(elapsedTime);
    }

    float elapsedTime;
    IEnumerator HitRoutine()
    {
        
        playerMain.PlayAnimations("Hit", false, 0);
        elapsedTime = 0;
        
        yield return null;
        
        float lastFrameTime = Time.deltaTime;
        float time = playerAnimator.GetCurrentAnimatorStateInfo(0).length;

        while (elapsedTime < time - lastFrameTime)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        playerMain.currentState = PlayerState.Regular;
    }

   

}
