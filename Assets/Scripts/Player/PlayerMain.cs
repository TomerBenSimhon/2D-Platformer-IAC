using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum PlayerState 
{
    Hit, Attacking, Default, God, Dead, Start
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
    private GameObject swordTrail;
    ParticleSystem swordTrailParticles;

    private float gridSize = 0.0625f;
    
    //used for the dash attack so there wont be infinite dashes in air
    public int dashCount = 0;
    
    public PlayerState currentState;
    private void Start()
    {
        if (currentState != PlayerState.Start)
        {
            currentState = PlayerState.Default;
        }
        
        transform.position = GameManager.Instance.currentCheckpoint;
        
        swordTrail = swordAnimator.transform.GetChild(0).gameObject;
        swordTrailParticles = swordTrail.GetComponent<ParticleSystem>();
    }

    void Update()
    {
        StateControl();
        SwitchState();
        HandleSwordTrail();
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
            
            case PlayerState.Start:
                playerHit.enabled = false;
                playerInvincable.enabled = false;
                playerActions.enabled = false;
                
                playerMovement.enabled = true;
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

    
    void HandleSwordTrail()
    {
        if (swordVisuals.enabled)
        {
            var main = swordTrailParticles.main;
            if (transform.localScale.x > 0)
            {
                main.startRotation = 50f * Mathf.Deg2Rad;
            }
            else if (transform.localScale.x < 0)
            {
                main.startRotation = -50f * Mathf.Deg2Rad;
            }
        }
    }

    public Vector2 SnapToGrid(Vector2 position)
    {
        float x = Mathf.Round(position.x / gridSize) * gridSize;
        float y = Mathf.Round(position.y / gridSize) * gridSize;
        return new Vector2(x, y);
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

    public void SwordVisualsEnabled(bool isEnabled)
    {
        if (isEnabled)
        {
            swordVisuals.enabled = true;
            swordTrail.SetActive(true);
        }
        else
        {
            swordVisuals.enabled = false;
            swordTrail.SetActive(false);
        }
    }
    

}
