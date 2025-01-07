using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum PlayerState 
{
    Hit, Attacking, Default, God, Dead
}



public class PlayerMain : MonoBehaviour
{
    [SerializeField] PlayerMovement playerMovement;
    [SerializeField] PlayerActions playerActions;
    [SerializeField] PlayerHit playerHit;
    [SerializeField] PlayerInvincable playerInvincable;
    [SerializeField] PlayerDeath playerDeath;
    [SerializeField] Health health;
    
    [SerializeField] Animator playerAnimator;
    [SerializeField] Animator swordAnimator;
    [SerializeField] SpriteRenderer swordVisuals;
    
    public PlayerState currentState;
    private void Start()
    {
        currentState = PlayerState.Default;
    }

    void Update()
    {
        StateControl();
        SwitchState();
    }

    void StateControl()
    {

        switch (currentState)
        {
            case PlayerState.Default:
                playerHit.enabled = false;
                playerInvincable.enabled = false;
                
                playerActions.enabled = true;
                playerMovement.enabled = true;
                break;

            case PlayerState.Attacking:
                playerMovement.enabled = false;
                playerHit.enabled = false;
                playerInvincable.enabled = false;
                
                playerActions.enabled = true;
                break;

            case PlayerState.Hit:
                playerActions.enabled = false;
                playerMovement.enabled = false;
                playerInvincable.enabled = false;
                
                playerHit.enabled = true;
                break;

            case PlayerState.God:
                playerHit.enabled = false;
                
                playerInvincable.enabled = true;
                playerActions.enabled = true;
                playerMovement.enabled = true;
                break;
            
            case PlayerState.Dead:
                playerActions.enabled = false;
                playerMovement.enabled = false;
                playerInvincable.enabled = false;
                playerHit.enabled = false;
                
                playerDeath.enabled = true;
                break;
        }
    }

    void SwitchState()
    {
        if (health.isDead)
        {
            currentState = PlayerState.Dead;
        }
    }

    public void PlayAnimations(string name, bool isTime, float time)
    {
        if (!isTime)
        {
            playerAnimator.Play(name);
            swordAnimator.Play(name);
        }
        else
        {
            playerAnimator.Play(name, 0, time);
            playerAnimator.speed = 0;
            swordAnimator.Play(name, 0, time);
            swordAnimator.speed = 0;
        }
    }

}
