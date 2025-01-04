using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHit : MonoBehaviour
{
    [SerializeField] Animator playerAnimator;
    [SerializeField] Animator swordAnimator;

    PlayerMain playerMain;
    Health playerHealth;
    Rigidbody2D rb;
    
    [SerializeField] float gravityForce;
    [SerializeField] float maxFallVelocity;

    public Vector2 knockbacDirection;
   
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
        playerHealth.TakeKnockback(knockbacDirection, 10f);
    }

   

    private void Update()
    {
        Debug.Log(elapsedTime);
    }

    private void FixedUpdate()
    {
        HandleGravity();
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
        
        playerMain.currentState = PlayerState.Regular;
    }

    void HandleGravity()
    {
        rb.velocity += new Vector2(rb.velocity.x, gravityForce * Time.fixedDeltaTime);
        rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -maxFallVelocity, Mathf.Infinity));
    }

   

}
