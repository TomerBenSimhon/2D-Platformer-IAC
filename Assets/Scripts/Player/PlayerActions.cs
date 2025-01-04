using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerActions : MonoBehaviour
{ 
    Rigidbody2D rb;
    PlayerMain playerMain;
    
    [SerializeField] Animator playerAnimator;
    [SerializeField] Animator swordAnimator;
    
    [Header("Sword Throw")]
    
    [SerializeField] SpriteRenderer swordVisuals;
    [SerializeField] GameObject swordProjectile;
    [SerializeField] float maxDistance;
    
    
    [Header("Sword Attack")]
   
    [SerializeField] float dashForce;
    [SerializeField] float dashDelay;
    [SerializeField] float dashDuration;
    [SerializeField] float endDashDelay;
    [SerializeField] float attackRate;
    
    
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerMain = GetComponent<PlayerMain>();
    }

    
    void Update()
    {
        HandelInputs();
        HandleSwordThrow();
        HandleAttack();
    }

    private void OnDisable()
    {
       StopAllCoroutines();
       rb.velocity = Vector2.zero;
    }


    bool attackInput;
    bool attacAvail;
    
    bool throwInput;
    bool throwAvail;
    
    float currentAttackTime = -10f;
    
    void HandelInputs()
    {
        attackInput = Input.GetButtonDown("Fire1");
        throwInput = Input.GetButtonDown("Fire2");

        if (attackInput && Time.time > currentAttackTime + attackRate)
        {
            attacAvail = true;
            
            if (attackBuffer != null) {StopCoroutine(attackBuffer);}

            attackBuffer = StartCoroutine(AttackBuffer());
        }

        if (throwInput && swordVisuals.enabled)
        {
            throwAvail = true;
        }
    }
    

    #region Sword Throw

    
    
    
    void HandleSwordThrow()
    {
        if (throwAvail)
        {
            throwAvail = false;
            swordVisuals.enabled = false;
            
            Instantiate(swordProjectile, transform.position, Quaternion.identity);
           
        }
    }

    public void SwordVisuallsActive(bool active)
    {
        swordVisuals.enabled = active;
    }
    

    #endregion

    #region Sword Attack

    
    void HandleAttack()
    {
        if (attacAvail && swordVisuals.enabled && playerMain.currentState == PlayerState.Regular)
        {
            attacAvail = false;
            currentAttackTime = Time.time;
            
            
            attackDashRoutine = StartCoroutine(AttackDash());
            attackAnimRoutine = StartCoroutine(AttackDashAnim());
        }
    }

    Coroutine attackBuffer;
    IEnumerator AttackBuffer()
    {
        yield return new WaitForSeconds(0.1f);
        attacAvail = false;
    }

    private float elapsedTime;
    private Coroutine attackDashRoutine;
    IEnumerator AttackDash()
    {
        playerMain.currentState = PlayerState.Attacking;
        Vector2 currentVelocity = rb.velocity;
        
        
        elapsedTime = 0;
        while(elapsedTime < dashDelay)
        {
            elapsedTime += Time.deltaTime;
            rb.velocity = Vector2.Lerp(currentVelocity, Vector2.zero, elapsedTime / dashDelay);

            if (Input.GetAxis("Horizontal") != 0)
            {
                transform.localScale = new Vector3(Mathf.Sign(Input.GetAxis("Horizontal")), 1, 1);
            }
            
            yield return new WaitForEndOfFrame();
        }

        Vector2 dash = new Vector2(dashForce * Mathf.Sign(transform.localScale.x), 0);
        rb.velocity = dash;
        yield return new WaitForSeconds(dashDuration);
        elapsedTime = 0;
        
        while(elapsedTime < endDashDelay)
        {
            elapsedTime += Time.deltaTime;
            rb.velocity = Vector2.Lerp(dash / 3, Vector2.zero, elapsedTime / endDashDelay); 
            yield return new WaitForEndOfFrame();
        }
        
        rb.velocity = Vector2.zero;
        
        playerMain.currentState = PlayerState.Regular;
    }

    private Coroutine attackAnimRoutine;
    IEnumerator AttackDashAnim()
    {
        while (playerMain.currentState == PlayerState.Attacking)
        {
            playerAnimator.Play("Attack");
            playerAnimator.speed = 1;
            
            swordAnimator.Play("Attack");
            swordAnimator.speed = 1;

            yield return new WaitForEndOfFrame();
        }
    }

    #endregion
}
