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
        GroundCheck();
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
    bool jumpHeld;
    private bool jumpAvail;
    void HandleInputs()
    {
        moveHorizontal = Input.GetAxis("Horizontal");
        
        jumpDown = Input.GetButtonDown("Jump");
        jumpHeld = Input.GetButton("Jump");
        
        if (jumpDown)
        {
            jumpAvail = true;
            
            if (jumpBufferCoroutine != null) {StopCoroutine(jumpBufferCoroutine);}
            
            jumpBufferCoroutine = StartCoroutine(JumpBuffer());
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

    void GroundCheck()
    {
        isGrounded = Physics2D.OverlapArea(groundCheck.bounds.min, groundCheck.bounds.max, groundLayer); 
    }
    bool CanJump()
    {
        if(!jumpAvail) {return false;}

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
            didFastFall = false;
        }

       
    }

    Coroutine jumpBufferCoroutine;
    IEnumerator JumpBuffer()
    {
        yield return new WaitForSeconds(jumpBufferTime);
        jumpAvail = false;
    }

   

    #endregion

    #region Gravity

    [Header("Gravity")]
    [SerializeField] float defaultGravity = 1;
    [SerializeField] float fastGravity = 1;
    
    [SerializeField] float maxFallSpeed = 20f;
    
    float currentGravity;
    
    void HandleGravity()
    {
        if (isGrounded && rb.velocity.y == 0)
        {
            didFastFall = false;
        }
        
        if (CanFastFall())
        {
            rb.velocity = new Vector2(rb.velocity.x, 0.4f * rb.velocity.y);
            currentGravity = fastGravity;
            didFastFall = true;
        }
        else if (!didFastFall)
        {
            currentGravity = defaultGravity;
        }
        
        
        rb.velocity += new Vector2(rb.velocity.x, currentGravity * Physics2D.gravity.y * Time.fixedDeltaTime);
        rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -maxFallSpeed, Mathf.Infinity));
    }
    
    bool didFastFall;
    bool CanFastFall()
    {
        if (rb.velocity.y > 0.1 && !jumpHeld && !didFastFall)
        {
            return true;
        }
        return false;
    }

    #endregion

   
}

























