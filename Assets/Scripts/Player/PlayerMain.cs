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
    }

    void StateControl()
    {
        if (currentState == PlayerState.Hit)
        {
            playerActions.enabled = false;
            playerMovement.enabled = false;
            playerHit.enabled = true;
        }
        else
        {
            playerActions.enabled = true;
            playerHit.enabled = false;
            if (currentState == PlayerState.Attacking)
            {
                playerMovement.enabled = false;
            }
            else
            {
                playerMovement.enabled = true;
            }

            if (currentState == PlayerState.God)
            {
                playerMovement.enabled = true;
            }
            else
            {
                playerMovement.enabled = false;
            }
            
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
