using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerActions : MonoBehaviour
{ 
    Rigidbody2D rb;
    [SerializeField] Animator playerAnimator;
    [SerializeField] Animator swordAnimator;
    
    [Header("Sword Throw")]
    
    [SerializeField] GameObject swordVisuals;
    [SerializeField] GameObject swordProjectile;
    [SerializeField] float maxDistance;
    SwordProjectileBehavior swordProjectileScript;
    
    [Header("Sword Attack")]
   
    [SerializeField] float dashForce;
    [SerializeField] float dashDelay;
    [SerializeField] float dashDuration;
    [SerializeField] float attackRate;
    PlayerMovement playerMovement;
    
    
    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        rb = GetComponent<Rigidbody2D>();
    }

    
    void Update()
    {
        HandelInputs();
        HandleSwordThrow();
        HandleAttack();
    }


    bool attackInput;
    bool attacAvail;
    
    bool throwInput;
    bool throwAvail;
    
    float currentAttackTime;
    
    void HandelInputs()
    {
        attackInput = Input.GetButtonDown("Fire1");
        throwInput = Input.GetButtonDown("Fire2");
        
        if(attackInput && Time.time > currentAttackTime + attackRate) { attacAvail = true; }
        if(throwInput && swordVisuals.activeSelf){ throwAvail = true; }
    }
    

    #region Sword Throw

    
    
    
    void HandleSwordThrow()
    {
        if (throwAvail)
        {
            throwAvail = false;
            swordVisuals.SetActive(false);
            
            GameObject swordInstance = Instantiate(swordProjectile, transform.position, Quaternion.identity);
            swordProjectileScript = swordInstance.GetComponent<SwordProjectileBehavior>();
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Sword") && swordProjectileScript.isRetriving)
        {
            swordVisuals.SetActive(true);
            Destroy(swordProjectileScript.gameObject);
        }
    }

    #endregion

    #region Sword Attack

    
    void HandleAttack()
    {
        if (attacAvail)
        {
            attacAvail = false;
            currentAttackTime = Time.time;
            
            
            StartCoroutine(AttacDash());
            StartCoroutine(AttacDashAnim());
        }
    }

    private float elapsedTime;
    bool attacking;
    IEnumerator AttacDash()
    {
        attacking = true;
        Vector2 currentVelocity = rb.velocity;
        playerMovement.enabled = false;
        
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
        
        while(elapsedTime < 0.2f)
        {
            elapsedTime += Time.deltaTime;
            rb.velocity = Vector2.Lerp(dash, Vector2.zero, elapsedTime / 0.2f); 
            yield return new WaitForEndOfFrame();
        }
        
        rb.velocity = Vector2.zero;
        playerMovement.enabled = true;
        attacking = false;
    }

    IEnumerator AttacDashAnim()
    {
        while (attacking)
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
