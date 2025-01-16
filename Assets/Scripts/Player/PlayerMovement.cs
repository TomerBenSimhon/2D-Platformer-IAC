using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerMovement : MonoBehaviour
{ 
    Rigidbody2D rb;
    [SerializeField] Animator playerAnimator;
    [SerializeField] Animator swordAnimator;
    [SerializeField] SpriteRenderer swordVisuals;
    [SerializeField] Collider2D playerCollider;
    
    
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
        if(Time.timeScale < 0.1f) {return;}
        
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

        if (jumpUp && canHalf && !isJumpingDown && rb.velocity.y > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
            canHalf = false;
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
    bool canHalf;

    void GroundCheck()
    {
        isGrounded = Physics2D.OverlapArea(groundCheck.bounds.min, groundCheck.bounds.max, groundLayer);

        if (isGrounded && Mathf.Abs(rb.velocity.y) <0.5f)
        {
            didJump = false;
            canCoyote = true;
            canHalf = true;
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
    
    [SerializeField] ParticleSystem jumpEffect;
    void HandleJump()
    {
        if (CanJump())
        {
            if (Input.GetKey(KeyCode.S))
            {
                rb.velocity = new Vector2(rb.velocity.x, 8);
                StartCoroutine(JumpDownPlatform());
                AudioManager.Instance.PlaySFX("Player_Jump", 0.3f, Random.Range(0.8f, 1.2f));
            }
            else
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce); 
                AudioManager.Instance.PlaySFX("Player_Jump", 0.6f, Random.Range(0.8f, 1.2f));
            }
           
            didJump = true;
            
            jumpAvail = false;
            
            jumpEffect.Play();
        }

       
    }

    Coroutine jumpBufferCoroutine;
    IEnumerator JumpBuffer()
    {
        yield return new WaitForSeconds(jumpBufferTime);
        jumpAvail = false;
    }

    private bool isJumpingDown;
    IEnumerator JumpDownPlatform()
    {
        isJumpingDown = true;
        playerCollider.excludeLayers |= 1 << LayerMask.NameToLayer("Platform");
        yield return new WaitForSeconds(0.7f);
        playerCollider.excludeLayers &= ~(1 << LayerMask.NameToLayer("Platform"));
        isJumpingDown = false;
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
        if (isGrounded)
        {
            currentGravity = 0;
            return;
        }

        
        if (!jumpHeld || rb.velocity.y < -apexThreshHold)
        {
            if (rb.velocity.y > -maxFallSpeed/2)
            {
                currentGravity = fastGravity;
            }
            else
            {
                currentGravity = Helpers.MapValue(rb.velocity.y, -maxFallSpeed, -maxFallSpeed/2, 0, fastGravity);
            }
            
        }
        else if (MathF.Abs(rb.velocity.y) < apexThreshHold && jumpHeld)
        {
            currentGravity = defaultGravity * 0.5f;
        }
        else
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

























