using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public enum EnemyState
{
   Patrol, Chase, Shocked, Hit, Stun, Dead
}

public class EnemyMain : MonoBehaviour
{
   GameObject player;
   PlayerMain playerMain;
   Health playerHealth;
   PlayerHit playerHitScript;
   Transform playerTransform;

   Health enemyHealth;
   
   [SerializeField] GameObject visuals;
   
   [SerializeField] PatrolState patrolState;
   [SerializeField] ChaseState chaseState;
   [SerializeField] EnemyHit enemyHit;
   [SerializeField] EnemyStunned enemyStunned;
   [SerializeField] EnemyShocked enemyShocked;
   [SerializeField] EnemyDeath enemyDeath;
   
   [Header("State")]
   public EnemyState currentState;
   
   [Header("Player Detection")]
   [SerializeField] float maxDetectionDistance;
   [SerializeField] float spottingFOV;
   [SerializeField] LayerMask obstacleLayer;

   [SerializeField] float chaseStateExitTime;
   
   [Header("Player Damage")]
   [SerializeField] CircleCollider2D hitCollider;
   [SerializeField] Collider2D attackCollider;
   [SerializeField] LayerMask playerLayer;
   [SerializeField] float onTouchKnockback;
   [SerializeField] float attackKnockback;

   [Header("Effects")] 
   public CinemachineImpulseSource attackImpulse;
   public ParticleSystem attackRumble;
   public CinemachineImpulseSource touchImpulse;

   

   void Start()
   {
      player = FindObjectOfType<PlayerMovement>().gameObject;
      playerMain = player.GetComponent<PlayerMain>();
      playerHealth = player.GetComponent<Health>();
      playerHitScript = player.GetComponent<PlayerHit>();
      playerTransform = player.transform;
      
      enemyHealth = GetComponent<Health>();
     
   }

   private void Awake()
   {
      patrolState.enabled = true;
      chaseState.enabled = false;
   }

   void Update()
   {
      SwitchState();
      StateControl();
      DamagePlayerOnTouch();
   }

   #region Player Spotting

   bool IsPlayerSpotted()
   {
      //checks if facing the player
      if (!IsFacingPlayer())
      {
         return false;
      }
      
      
      //checks if close enough to player
      float distanceToPlayer = Vector2.Distance(player.transform.position, transform.position);
      if (distanceToPlayer > maxDetectionDistance)
      {
         return false;
      }
      
      
      //checks if player is in the FOV
      Vector2 directionToPlayer = (player.transform.position - transform.position).normalized;
      Vector2 facingDirection = new Vector2(Mathf.Sign(visuals.transform.localScale.x), 0f);
      
      float angleToPlayer = Vector2.Angle(facingDirection, directionToPlayer);

      if (angleToPlayer > spottingFOV / 2)
      {
         return false;
      }
      
      
      //checks LOS
      bool playerInLOS = !Physics2D.Raycast(transform.position, directionToPlayer, distanceToPlayer, obstacleLayer);

      if (!playerInLOS)
      {
         return false;
      }
      
      return true;
   }

   public bool CanSeePlayer()
   {
      //checks if close enough to player
      float distanceToPlayer = Vector2.Distance(player.transform.position, transform.position);
      if (distanceToPlayer > maxDetectionDistance)
      {
         return false;
      }
      
      //checks LOS
      Vector2 directionToPlayer = (player.transform.position - transform.position).normalized;
      bool playerInLOS = !Physics2D.Raycast(transform.position,directionToPlayer, distanceToPlayer, obstacleLayer);

      if (!playerInLOS)
      {
         return false;
      }
      
      return true;
   }
   
   bool IsFacingPlayer()
   {
      float directionToPlayer = player.transform.position.x - transform.position.x;
      float enemyFacingDirection = visuals.transform.localScale.x;

      return Mathf.Approximately(Mathf.Sign(directionToPlayer), Mathf.Sign(enemyFacingDirection));

   }

   #endregion

   #region Damage Player

   private Collider2D playerHitTouch;
   void DamagePlayerOnTouch()
   {
      
      playerHitTouch = Physics2D.OverlapCircle(hitCollider.bounds.center, hitCollider.radius, playerLayer);
      
      if (playerHitTouch && 
          currentState != EnemyState.Dead && 
          playerMain.currentState != PlayerState.Hit && playerMain.currentState != PlayerState.God && playerMain.currentState != PlayerState.Dead)
      {

         if (playerMain.currentState == PlayerState.Attacking)
         {
            if (currentState == EnemyState.Chase || currentState == EnemyState.Shocked)
            {
               DamagePlayer(32,onTouchKnockback);
               GameManager.Instance.HitStop(0.1f);
               CameraManager.Instance.CameraShake(touchImpulse);
            }
         }
         else
         {
            DamagePlayer(32,onTouchKnockback);
            GameManager.Instance.HitStop(0.1f);
            CameraManager.Instance.CameraShake(touchImpulse);
         }
      }
   }

   private bool playerHitAttack;
   
   //used in an event in the attack animation
   void AttackHitBox()
   {
      playerHitAttack = Physics2D.OverlapArea(attackCollider.bounds.min, attackCollider.bounds.max, playerLayer);

      if (playerHitAttack && playerMain.currentState != PlayerState.Hit && playerMain.currentState != PlayerState.God && playerMain.currentState != PlayerState.Dead)
      {
        DamagePlayer(64, attackKnockback);
        GameManager.Instance.HitStop(0.2f);
      }
   }
   //used in the same event in the attack animation
   void PlayAttackEffect()
   {
      attackRumble.Play();
      CameraManager.Instance.CameraShake(attackImpulse);
   }

   void DamagePlayer(int damage, float knockbackForce)
      {
         float directionToPlayer = Mathf.Sign(playerTransform.position.x - transform.position.x);
            
         playerHitScript.knockbacDirection = new Vector2(directionToPlayer, 1);
         playerHitScript.knockbackForce = knockbackForce;
            
         playerMain.currentState = PlayerState.Hit;
         playerHealth.TakeDamage(damage);
      }

   #endregion

   #region State Control

   void StateControl()
   {
      if (!enemyHealth.isDead)
      {
         if (currentState == EnemyState.Patrol && IsPlayerSpotted())
         {
            currentState = EnemyState.Shocked;
         }

         if (currentState == EnemyState.Chase)
         {
            if (!CanSeePlayer() && chaseExitTimerRoutine == null)
            {
               chaseExitTimerRoutine = StartCoroutine(ChaseStateExitTimer());
            }
            else if (CanSeePlayer() && chaseExitTimerRoutine != null)
            {
               StopCoroutine(chaseExitTimerRoutine);
               chaseExitTimerRoutine = null;
            }
       
         }
      }
      else
      {
         currentState = EnemyState.Dead;
      }
      
      
   }

   void SwitchState()
   {
      switch (currentState)
      {
         case EnemyState.Chase:
            patrolState.enabled = false;
            enemyStunned.enabled = false;
            enemyHit.enabled = false;
            enemyShocked.enabled = false;
            
            chaseState.enabled = true;
            break;
         
         case EnemyState.Shocked:
            patrolState.enabled = false;
            enemyStunned.enabled = false;
            enemyHit.enabled = false;
            chaseState.enabled = false;
            
            enemyShocked.enabled = true;
            break;
           
         case EnemyState.Patrol:
            chaseState.enabled = false;
            enemyStunned.enabled = false;
            enemyHit.enabled = false;
            enemyShocked.enabled = false;
            
            patrolState.enabled = true;
            break;
         
         case EnemyState.Stun:
            chaseState.enabled = false;
            patrolState.enabled = false;
            enemyHit.enabled = false;
            enemyShocked.enabled = false;

            enemyStunned.enabled = true;
            break;
         
         case EnemyState.Hit:
            chaseState.enabled = false;
            patrolState.enabled = false;
            enemyStunned.enabled = false;
            enemyShocked.enabled = false;

            enemyHit.enabled = true;
            break;
         
         case EnemyState.Dead:
            chaseState.enabled = false;
            patrolState.enabled = false;
            enemyStunned.enabled = false;
            enemyShocked.enabled = false;
            enemyHit.enabled = false;
            
            enemyDeath.enabled = true;
            break;
      }
   }
   
   
   Coroutine chaseExitTimerRoutine;
   IEnumerator ChaseStateExitTimer()
   {
      yield return new WaitForSeconds(chaseStateExitTime);
      currentState = EnemyState.Patrol;
   }
   

   #endregion
  
   
   
}
