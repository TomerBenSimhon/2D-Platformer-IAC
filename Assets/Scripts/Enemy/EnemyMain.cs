using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class EnemyMain : MonoBehaviour
{
   GameObject player;
   PlayerMain playerMain;
   Health playerHealth;
   PlayerHit playerHitScript;
   Transform playerTransform;

   Health enemyHealth;
   
   [SerializeField] GameObject visuals;
   
   List<IEnemyState> allStates;
   
   [field: SerializeField] public EnemyState CurrentState { get; private set; }
   
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
   public ParticleSystem attackJump;
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
      // GetComponentsInChildren covers both cases — states living on
      // this same GameObject, or split across child objects — so this
      // works regardless of how the hierarchy is actually set up. If
      // everything's confirmed to be on this one GameObject, plain
      // GetComponents<IEnemyState>() would do the same job slightly
      // cheaper, but the difference is a one-time, negligible cost here.
      allStates = new List<IEnemyState>(GetComponentsInChildren<IEnemyState>());
      ChangeState(EnemyState.Patrol);
   }

   void Update()
   {
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
          CurrentState != EnemyState.Dead && 
          playerMain.currentState != PlayerState.Hit && playerMain.currentState != PlayerState.God && playerMain.currentState != PlayerState.Dead)
      {

         if (playerMain.currentState == PlayerState.Attacking)
         {
            if (CurrentState == EnemyState.Chase || CurrentState == EnemyState.Shocked)
            {
               DamagePlayer(32,onTouchKnockback);
               GameManager.Instance.HitStop(0.1f);
               CameraManager.Instance.CameraShake(touchImpulse);
               if (CurrentState == EnemyState.Patrol)
               {
                  ChangeState(EnemyState.Shocked);
               }
            }
         }
         else
         {
            DamagePlayer(32,onTouchKnockback);
            GameManager.Instance.HitStop(0.1f);
            CameraManager.Instance.CameraShake(touchImpulse);
            if (CurrentState == EnemyState.Patrol)
            {
               ChangeState(EnemyState.Shocked);
            }
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
      AudioManager.Instance.PlayEnemySFX("Enemy_Attack", 0.12f, 0.9f, 1.1f);
   }

   //called in the start of the attack animation
   void PlayAttackJumpEffect()
   {
      attackJump.Play();
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
         if (CurrentState == EnemyState.Patrol && IsPlayerSpotted())
         {
            ChangeState(EnemyState.Shocked);
         }

         if (CurrentState == EnemyState.Chase)
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
         ChangeState(EnemyState.Dead);
      }
   }

   // Replaces the old per-frame switch statement. Only runs when the
   // state actually changes (not every frame), and works for any number
   // of states without needing a new case added each time one is created.
   public void ChangeState(EnemyState newState)
   {
      if (newState == CurrentState) return;

      foreach (var state in allStates)
      {
         if (state.StateId == newState) state.Enter();
         else state.Exit();
      }

      CurrentState = newState;
   }
   
   
   Coroutine chaseExitTimerRoutine;
   IEnumerator ChaseStateExitTimer()
   {
      yield return new WaitForSeconds(chaseStateExitTime);
      ChangeState(EnemyState.Patrol);
   }
   

   #endregion
  
   
   
}