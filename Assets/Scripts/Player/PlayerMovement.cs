using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{ 
    Rigidbody2D rb;
    [SerializeField] Animator playerAnimator;
    [SerializeField] Animator swordAnimator;
    [SerializeField] SpriteRenderer swordVisuals;
    
    PlayerMain playerMain;
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null) {Debug.LogError("rb is null in PlayerMovement");}
        
        playerMain = GetComponent<PlayerMain>();
    }

    void OnEnable()
    {
        //playerMain.currentState = PlayerState.Default;
    }

    // Update is called once per frame
    void Update()
    {
        HandleInputs();
        HandleCoyote();
        GroundCheck();
        HandleAnimations();
        HandleSpriteFlip();
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
    bool jumpUp;
    private bool jumpAvail;
    void HandleInputs()
    {
        moveHorizontal = Input.GetAxis("Horizontal");
        
        jumpDown = Input.GetButtonDown("Jump");
        jumpHeld = Input.GetButton("Jump"); 
        jumpUp = Input.GetButtonUp("Jump");
        
        if (jumpDown)
        {
            jumpAvail = true;
            
            if (jumpBufferCoroutine != null) {StopCoroutine(jumpBufferCoroutine);}
            
            jumpBufferCoroutine = StartCoroutine(JumpBuffer());
        }

        if (jumpUp && rb.velocity.y > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
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

    void HandleSpriteFlip()
    {
        if(moveHorizontal == 0) {return;}
        transform.localScale = new Vector3(Mathf.Sign(moveHorizontal), 1, 1);
    }
    
    #endregion

    #region Jump

    [Header("GroundCheck")]
    
    [SerializeField] Collider2D groundCheck;
    [SerializeField] LayerMask groundLayer;
    
    bool isGrounded;
    bool isCoyote;
    bool canCoyote;
    bool didJump;

    void GroundCheck()
    {
        isGrounded = Physics2D.OverlapArea(groundCheck.bounds.min, groundCheck.bounds.max, groundLayer);

        if (isGrounded && Mathf.Abs(rb.velocity.y) <0.5f)
        {
            didJump = false;
            canCoyote = true;
        }
    }
    bool CanJump()
    {
        if(!jumpAvail) {return false;}

        if (!isGrounded && !isCoyote) {return false;}

        return true;
    }

    void HandleCoyote()
    {
        if (!didJump && !isGrounded && canCoyote)
        {
            isCoyote = true;
            canCoyote = false;
            
            if(coyoteCoroutine != null) {StopCoroutine(coyoteCoroutine);}

            coyoteCoroutine = StartCoroutine(CoyoteTimer());
            
        }
    }

    Coroutine coyoteCoroutine;
    IEnumerator CoyoteTimer()
    {
        yield return new WaitForSeconds(coyoteTime);
        isCoyote = false;
    }
    
    

    [Header("Jump")]
    [SerializeField] float jumpForce = 5f;
    [SerializeField] float jumpBufferTime = 0.2f;
    [SerializeField] float coyoteTime = 0.1f;
    
    
    void HandleJump()
    {
        if (CanJump())
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            didJump = true;
            
            jumpAvail = false;
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
    [SerializeField] float apexThreshHold = 3f;
    
    float currentGravity;
    
    void HandleGravity()
    {
        
        
        if (!jumpHeld || rb.velocity.y < -apexThreshHold)
        {
            currentGravity = fastGravity;
        }
        else if (MathF.Abs(rb.velocity.y) < apexThreshHold && jumpHeld)
        {
            currentGravity = defaultGravity * 0.5f;
        }
        else
        {
            currentGravity = defaultGravity;
        }
        
        
        rb.velocity += new Vector2(rb.velocity.x, currentGravity * Physics2D.gravity.y * Time.fixedDeltaTime);
        rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -maxFallSpeed, Mathf.Infinity));
    }
    
  

    #endregion

    #region Animations

    void HandleAnimations()
    {
        if (isGrounded)
        {
            playerAnimator.speed = 1;
            if(swordVisuals.enabled) {swordAnimator.speed = 1;}
            if (Mathf.Abs(moveHorizontal) > 0)
            {
                playerMain.PlayAnimations("Run",  false, 0);
            }
            else
            {
                playerMain.PlayAnimations("Idle",false, 0);
            }
        }
        else
        {
            float time = Helpers.MapValue(rb.velocity.y, -jumpForce, jumpForce, 0, 1f);
            
            playerMain.PlayAnimations("Jump", true, 0.99f - time);
            
        }
        
    }

    

    #endregion
   
}

























