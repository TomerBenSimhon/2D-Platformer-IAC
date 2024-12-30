using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{ 
    Rigidbody2D rb;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null) {Debug.LogError("rb is null in PlayerMovement");}
        
        
    }

    // Update is called once per frame
    void Update()
    {
        HandleInputs();
    }
    void FixedUpdate()
    {
        HandleMovement();
        HandleGravity();
        HandleJump();
    }


    #region Inputs

    float moveHorizontal;

    bool jumpDown;
    private bool jumpAvail;
    void HandleInputs()
    {
        moveHorizontal = Input.GetAxis("Horizontal");
        
        jumpDown = Input.GetButtonDown("Jump");

        if (jumpDown)
        {
            jumpAvail = true;
        }
    }
    
    

    #endregion
    
    #region Movement

    
    [Header("Movement")]
    [SerializeField] float moveSpeed = 12f;
    void HandleMovement()
    {
        rb.velocity = new Vector2(moveHorizontal * moveSpeed, rb.velocity.y);
    }
    
    #endregion

    #region Jump

    [Header("GroundCheck")]
    
    [SerializeField] Collider2D groundCheck;
    [SerializeField] LayerMask groundLayer;
    bool isGrounded;
    bool CanJump()
    {
        if(!jumpAvail) {return false;}
        
        isGrounded = Physics2D.OverlapArea(groundCheck.bounds.min, groundCheck.bounds.max, groundLayer);
        
        if (!isGrounded) {return false;}

        return true;
    }

    [Header("Jump")]
    [SerializeField] float jumpForce = 5f;
    [SerializeField] float jumpBufferTime = 0.2f;
    void HandleJump()
    {
        if (CanJump())
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            
            jumpAvail = false;
        }
    }

    IEnumerator JumpBuffer()
    {
        yield return new WaitForSeconds(jumpBufferTime);
        jumpAvail = false;
    }

    #endregion

    #region Gravity

    [Header("Gravity")]
    [SerializeField] float defaultGravity = 1;
    [SerializeField] float apexGravity = 1;
    
    float currentGravity;
    
    void HandleGravity()
    {
        currentGravity = Physics2D.gravity.y * defaultGravity * Time.fixedDeltaTime;
        
        rb.velocity += new Vector2(rb.velocity.x,currentGravity);
    }

    #endregion

   
}

























