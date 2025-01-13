using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHit : MonoBehaviour
{
    [SerializeField] Animator playerAnimator;
    [SerializeField] Animator swordAnimator;
    
    [Header("Ground Check")]
    [SerializeField] Collider2D groundCheck;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] float groundFriction;

    PlayerMain playerMain;
    Health playerHealth;
    Rigidbody2D rb;
    
    [Header("Gravity")]
    [SerializeField] float gravityForce;
    [SerializeField] float maxFallVelocity;

    // set by the enemy when hitting the player
    public Vector2 knockbacDirection;
    public float knockbackForce;
   
    void Awake()
    {
        playerMain = GetComponent<PlayerMain>();
        playerHealth = GetComponent<Health>();
        rb = GetComponent<Rigidbody2D>();
        
        elapsedTime = 0;
    }

    private void OnEnable()
    {
        elapsedTime = 0;
        if(hitRoutine != null) {StopCoroutine(hitRoutine);}
        hitRoutine = StartCoroutine(HitRoutine());
        TakeKnockback(knockbacDirection, knockbackForce);
    }

    private void OnDisable()
    {
        //reset those just in case
        knockbacDirection = Vector2.zero;
        knockbackForce = 0;
        StopAllCoroutines();
    }


    private void FixedUpdate()
    {
        HandleGravity();
        GroundFriction();
    }

    float elapsedTime;
    Coroutine hitRoutine;
    IEnumerator HitRoutine()
    {
        
        playerMain.PlayAnimations("Hit", false, 0);
        playerAnimator.speed = 1f;
        swordAnimator.speed = 1f;
        elapsedTime = 0;
        
        yield return null;
        
        float lastFrameTime = Time.deltaTime;
        float time = playerAnimator.GetCurrentAnimatorStateInfo(0).length;

        while (elapsedTime < time - lastFrameTime)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        playerMain.currentState = PlayerState.God;
    }

    void HandleGravity()
    {
        rb.velocity += new Vector2(0, gravityForce * Physics2D.gravity.y * Time.fixedDeltaTime);
        rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -maxFallVelocity, Mathf.Infinity));
    }
    
    bool isGrounded;
    void GroundFriction()
    {
        isGrounded = Physics2D.OverlapArea(groundCheck.bounds.min, groundCheck.bounds.max, groundLayer);

        if (isGrounded)
        {
            rb.velocity = Vector2.MoveTowards(rb.velocity, Vector2.zero, groundFriction * Time.fixedDeltaTime);
        }
        
    }
    
    void TakeKnockback(Vector2 direction, float knockbackForce)
    {
        rb.velocity = direction * knockbackForce; 
    }

   

}
