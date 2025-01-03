using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMain : MonoBehaviour
{
    [SerializeField] PlayerMovement playerMovement;
    [SerializeField] PlayerActions playerActions;
    [SerializeField] PlayerHit playerHit;
    [SerializeField] Health health;
    
    [SerializeField] Collider2D hitCollider;
    
    public bool isHit = false;
    public bool isAttacking = false;

    private void Start()
    {
        
    }

    void Update()
    {
        StateControl();
    }

    void StateControl()
    {
        if (isHit)
        {
            playerActions.enabled = false;
            playerMovement.enabled = false;
            playerHit.enabled = true;
        }
        else
        {
            playerActions.enabled = true;
            playerMovement.enabled = true;
            playerHit.enabled = false;

            if (isAttacking)
            {
                playerMovement.enabled = false;
            }
            else
            {
                playerMovement.enabled = true;
            }
        }
        
        
    }
    
    
}
