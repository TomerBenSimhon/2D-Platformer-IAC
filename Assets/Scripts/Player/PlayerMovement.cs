using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerMovement : MonoBehaviour
{
    // =================================================================
    // Serialized Fields (visible/configurable in the Inspector)
    // =================================================================
    [SerializeField] Animator playerAnimator;
    [SerializeField] Animator swordAnimator;
    [SerializeField] SpriteRenderer swordVisuals;
    [SerializeField] GameObject playerColliderObject;

    [Header("Movement")]
    [SerializeField] float moveSpeed = 12f;
    [SerializeField] float accel = 6;
    [SerializeField] LayerMask wallLayer;

    [Header("GroundCheck")]
    [SerializeField] Collider2D groundCheck;
    [SerializeField] LayerMask groundLayer;

    [Header("Jump")]
    [SerializeField] float jumpForce = 5f;
    [SerializeField] float jumpBufferTime = 0.2f;
    [SerializeField] float coyoteTime = 0.1f;
    [SerializeField] ParticleSystem jumpEffect;

    [Header("Gravity")]
    [SerializeField] float defaultGravity = 1;
    [SerializeField] float fastGravity = 1;
    [SerializeField] float maxFallSpeed = 20f;
    [SerializeField] float apexThreshHold = 3f;


    // =================================================================
    // Private Fields (internal state, not exposed in the Inspector)
    // =================================================================

    // References
    Rigidbody2D rb;
    Collider2D[] playerColliders;
    PlayerMain playerMain;

    // Inputs
    float moveInput;
    bool jumpDown;
    bool jumpHeld;
    bool jumpUp;
    bool jumpAvail;

    // Movement
    float moveHorizontal;

    // Ground check
    bool isGrounded;
    bool isCoyote;
    bool canCoyote;
    bool didJump;
    bool canHalf;

    // Jump
    Coroutine coyoteCoroutine;
    Coroutine jumpBufferCoroutine;
    bool isJumpingDown;

    // Gravity
    float currentGravity;
    bool isFastFalling;


    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null) {Debug.LogError("rb is null in PlayerMovement");}
        
        playerMain = GetComponent<PlayerMain>();
        
        playerColliders = playerColliderObject.GetComponents<Collider2D>();
    }

    void OnEnable()
    {
        moveHorizontal = 0;
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

    void HandleInputs()
    {
        if (EventManager.Instance != null)
        {
            if(EventManager.Instance.isFreezeEventPlaying) {return;}
        }
        
        moveInput = Input.GetAxis("Horizontal");
        
        jumpDown = Input.GetButtonDown("Jump");
        jumpHeld = Input.GetButton("Jump"); 
        jumpUp = Input.GetButtonUp("Jump");
        
        if (jumpDown)
        {
            jumpAvail = true;
            
            if (jumpBufferCoroutine != null) {StopCoroutine(jumpBufferCoroutine);}
            
            jumpBufferCoroutine = StartCoroutine(JumpBuffer());
        }

        if (jumpUp && canHalf && !isJumpingDown && rb.velocity.y > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
            canHalf = false;
        }
    }
    
    

    #endregion
    
    #region Movement

    void HandleMovement()
    {
        moveHorizontal = Mathf.MoveTowards(moveHorizontal, moveInput, accel * Time.fixedDeltaTime);
        rb.velocity = new Vector2(moveHorizontal * moveSpeed, rb.velocity.y);
        
        bool isTouchingWall = Physics2D.Raycast(transform.position, Vector2.right * Mathf.Sign(rb.velocity.x), 0.3f, wallLayer);

        if (isTouchingWall && Mathf.Abs(rb.velocity.x) > 0.1f)
        {
            moveHorizontal = 0;
        }
        
    }

    
   

    void HandleSpriteFlip()
    {
        if(moveInput == 0) {return;}
        transform.localScale = new Vector3(Mathf.Sign(moveInput), 1, 1);
    }
    
    #endregion

    #region Jump

    void GroundCheck()
    {
        isGrounded = Physics2D.OverlapArea(groundCheck.bounds.min, groundCheck.bounds.max, groundLayer);

        if (isGrounded && Mathf.Abs(rb.velocity.y) <0.5f)
        {
            didJump = false;
            canCoyote = true;
            canHalf = true;
            isFastFalling = false;
            playerMain.dashCount = 0;
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

    IEnumerator CoyoteTimer()
    {
        yield return new WaitForSeconds(coyoteTime);
        isCoyote = false;
    }
    
    

    void HandleJump()
    {
        if (CanJump())
        {
            isFastFalling = false;
            
            if (Input.GetKey(KeyCode.S))
            {
                rb.velocity = new Vector2(rb.velocity.x, 8);
                StartCoroutine(JumpDownPlatform());
                AudioManager.Instance.PlayPlayerSFX("Player_Jump", 0.6f, 0.6f, 0.7f);
            }
            else
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce); 
                AudioManager.Instance.PlayPlayerSFX("Player_Jump", 0.6f, 0.9f, 1.2f);
            }
           
            didJump = true;
            
            jumpAvail = false;
            
            jumpEffect.Play();
        }

       
    }

    IEnumerator JumpBuffer()
    {
        yield return new WaitForSeconds(jumpBufferTime);
        jumpAvail = false;
    }

    IEnumerator JumpDownPlatform()
    {
        isJumpingDown = true;
        foreach (Collider2D collider in playerColliders)
        {
            collider.excludeLayers |= 1 << LayerMask.NameToLayer("Platform");
        }
        
        yield return new WaitForSeconds(0.7f);
        
        foreach (Collider2D collider in playerColliders)
        {
            collider.excludeLayers &= ~(1 << LayerMask.NameToLayer("Platform"));
        }
        
        isJumpingDown = false;
    }

   

    #endregion

    #region Gravity

    void HandleGravity()
    {
        if (isGrounded)
        {
            currentGravity = 0;
            return;
        }

        
        if (!jumpHeld || rb.velocity.y < -apexThreshHold)
        {
            isFastFalling = true;
            
            if (rb.velocity.y > -maxFallSpeed/2)
            {
                currentGravity = fastGravity;
            }
            else
            {
                currentGravity = Helpers.MapValue(rb.velocity.y, -maxFallSpeed, -maxFallSpeed/2, 0, fastGravity);
            }
            
        }
        else if (MathF.Abs(rb.velocity.y) < apexThreshHold && jumpHeld && !isFastFalling)
        {
            currentGravity = defaultGravity * 0.5f;
        }
        else if(!isFastFalling)
        {
            currentGravity = defaultGravity;
        }
        
        
        rb.velocity += new Vector2(0, currentGravity * Physics2D.gravity.y * Time.fixedDeltaTime);
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
            if (Mathf.Abs(moveInput) > 0)
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