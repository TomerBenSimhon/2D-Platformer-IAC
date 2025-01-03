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
        
    }
}
