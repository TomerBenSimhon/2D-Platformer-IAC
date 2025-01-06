using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInvincable : MonoBehaviour
{
    [SerializeField] PlayerMain playerMain;
    [SerializeField] float invincibilityDuration;


    void OnEnable()
    {
        if(invincibility != null) {StopCoroutine(invincibility);}
        invincibility = StartCoroutine(InvincibilityTimerRoutine());

        if(invincibilityVisuals != null) {StopCoroutine(invincibilityVisuals);}
        invincibilityVisuals = StartCoroutine(InvincibilityVisualsRoutine());
    }

    void OnDisable()
    {
        StopAllCoroutines();
        playerSprite.color = Color.white;
        swordSprite.color = Color.white;
    }


    Coroutine invincibility;
    IEnumerator InvincibilityTimerRoutine()
    {
        yield return new WaitForSeconds(invincibilityDuration);
        playerMain.currentState = PlayerState.Default;
    }

    [SerializeField] private float visableTime;
    [SerializeField] private float invisableTime;
    [SerializeField] SpriteRenderer playerSprite;
    [SerializeField] SpriteRenderer swordSprite;

    Color playerColor = Color.white;

    private Coroutine invincibilityVisuals;
    IEnumerator InvincibilityVisualsRoutine()
    {
        while (true)
        {
            playerColor.a = 1;
            playerSprite.color = playerColor;
            swordSprite.color = playerColor;
        
            yield return new WaitForSeconds(visableTime);
        
            playerColor.a = 0;
            playerSprite.color = playerColor;
            swordSprite.color = playerColor;
        
            yield return new WaitForSeconds(invisableTime);
            
            
        }
        
    }
}
