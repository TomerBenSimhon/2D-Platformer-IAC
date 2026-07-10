using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class PlayerActions : MonoBehaviour
{ 
    Rigidbody2D rb;
    PlayerMain playerMain;
    PlayerMovement playerMovement;

    [SerializeField] Collider2D attackCollider;
    [SerializeField] Animator playerAnimator;
    [SerializeField] Animator swordAnimator;
    
    [Header("Sword Throw")]
    
    [SerializeField] SpriteRenderer swordVisuals;
    [SerializeField] GameObject swordProjectile;
    [SerializeField] Collider2D edgeCheckCollider;
    [SerializeField] LayerMask edgeCheckLayers;
    [SerializeField] float maxDistance;
    
    
    [Header("Sword Attack")]
   
    [SerializeField] float dashForce;
    [SerializeField] float dashDelay;
    [SerializeField] float dashDuration;
    [SerializeField] float endDashDelay;
    [SerializeField] float attackRate;
    [SerializeField] float attackBuffer;
    [SerializeField] LayerMask enemyLayer;

    [Header("Effects")]
    [SerializeField] GameObject ghostEffect;
    
    
    
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerMain = GetComponent<PlayerMain>();
        playerMovement = GetComponent<PlayerMovement>();
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
       hitEnemies.Clear();
    }


    bool attackInput;
    bool attacAvail;
    
    bool throwInput;
    bool throwAvail;
    
    float currentAttackTime = -10f;
    
    void HandelInputs()
    {
        if (EventManager.Instance != null)
        {
            if(EventManager.Instance.isFreezeEventPlaying) {return;}
        }
        
        attackInput = Input.GetButtonDown("Fire1");
        throwInput = Input.GetButtonDown("Fire2");
        
        if (attackInput && Time.time > currentAttackTime + attackRate)
        {
            attacAvail = true;
            
            if (attackBufferRoutine != null) {StopCoroutine(attackBufferRoutine);}

            attackBufferRoutine = StartCoroutine(AttackBuffer());
        }

        if (throwInput && swordVisuals.enabled && (playerMain.currentState == PlayerState.Default || playerMain.currentState == PlayerState.God))
        {
            throwAvail = true;
        }
    }
    

    #region Sword Throw



     public bool groundDetected;
    void HandleSwordThrow()
    {
        if (throwAvail)
        {
            throwAvail = false;
            playerMain.SwordVisualsEnabled(false);
            playerMain.currentState = PlayerState.Default;
            
            groundDetected = Physics2D.OverlapArea(edgeCheckCollider.bounds.min, edgeCheckCollider.bounds.max, edgeCheckLayers);
            
            Instantiate(swordProjectile, transform.position, Quaternion.identity);
            
            AudioManager.Instance.PlayPlayerSFX("Sword_Throw", 0.6f, 1f, 1.3f);
           
        }
    }
    
    

    #endregion

    #region Sword Attack
    
    void HandleAttack()
    {
        if (attacAvail && playerMain.dashCount < 1 && swordVisuals.enabled && (playerMain.currentState == PlayerState.Default || playerMain.currentState == PlayerState.God))
        {
            attacAvail = false;
            currentAttackTime = Time.time;
            playerMain.dashCount++;
            
            attackDashRoutine = StartCoroutine(AttackDash());
            attackAnimRoutine = StartCoroutine(AttackDashAnim());
        }
    }

    Coroutine attackBufferRoutine;
    IEnumerator AttackBuffer()
    {
        yield return new WaitForSeconds(attackBuffer);
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

        AudioManager.Instance.PlayPlayerSFX("Player_Dash", 0.4f, 0.9f, 1.1f);
        Vector2 dash = new Vector2(dashForce * Mathf.Sign(transform.localScale.x), 0);
        rb.velocity = dash;
        elapsedTime = 0;
        
        isGhostEffect = true;
        StartCoroutine(GhostDashEffect());

        while (elapsedTime < dashDuration)
        {
            elapsedTime += Time.deltaTime;
            rb.velocity = dash;
            AttackHitBox();
            yield return new WaitForEndOfFrame();
        }
        
        
        
        elapsedTime = 0;
        
        while(elapsedTime < endDashDelay)
        {
            elapsedTime += Time.deltaTime;
            rb.velocity = Vector2.Lerp(dash / 3, Vector2.zero, elapsedTime / endDashDelay); 
            AttackHitBox();
            
            yield return new WaitForEndOfFrame();
        }
        
        rb.velocity = Vector2.zero;
        hitEnemies.Clear();
        
        isGhostEffect = false;
        
        playerMain.currentState = PlayerState.Default;
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

            yield return null;
        }
    }

    bool isGhostEffect;
    GameObject lastGhost;
    IEnumerator GhostDashEffect()
    {
        while (isGhostEffect)
        {
            if (lastGhost == null)
            {
                lastGhost = Instantiate(ghostEffect, transform.position, Quaternion.identity);
            }
            else
            {
                if (Vector2.Distance(lastGhost.transform.position, transform.position) > 2)
                {
                    lastGhost = Instantiate(ghostEffect, transform.position, Quaternion.identity);
                }
            }
            yield return null;
            
        }

        lastGhost = null;
    }

    Collider2D[] isEnemyHit;
    List<Collider2D> hitEnemies = new List<Collider2D>();
    
    void AttackHitBox()
    {
        isEnemyHit = Physics2D.OverlapAreaAll(attackCollider.bounds.min, attackCollider.bounds.max, enemyLayer);

        foreach (Collider2D enemy in isEnemyHit)
        {
            if (!hitEnemies.Contains(enemy))
            {
                hitEnemies.Add(enemy);
                
                EnemyMain enemyMain = enemy.transform.parent.parent.GetComponent<EnemyMain>();

                if (enemyMain.CurrentState == EnemyState.Stun || enemyMain.CurrentState == EnemyState.Patrol)
                {
                    Health enemyHealth = enemyMain.GetComponent<Health>();
                    
                    enemyMain.ChangeState(EnemyState.Hit);
                    enemyHealth.TakeDamage(50);
                    
                    GameManager.Instance.HitStopScale(0.5f, 0.1f);
                }
            }
        }
    }

    #endregion
}
