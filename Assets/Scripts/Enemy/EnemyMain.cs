using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyState
{
   Patrol, Chase, Hit, Stun, Dead
}

public class EnemyMain : MonoBehaviour
{
   GameObject player;
   PlayerMain playerMain;
   Health playerHealth;
   PlayerHit playerHitScript;
   Transform playerTransform;
   [SerializeField] GameObject visuals;
   
   [SerializeField] PatrolState patrolState;
   [SerializeField] ChaseState chaseState;
   
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

   

   void Start()
   {
      player = FindObjectOfType<PlayerMovement>().gameObject;
      playerMain = player.GetComponent<PlayerMain>();
      playerHealth = player.GetComponent<Health>();
      playerHitScript = player.GetComponent<PlayerHit>();
      playerTransform = player.transform;
     
   }

   private void Awake()
   {
      patrolState.enabled = true;
      chaseState.enabled = false;
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
      bool playerInLOS = !Physics2D.Raycast(transform.position,directionToPlayer, distanceToPlayer, obstacleLayer);

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
      
      if (playerHitTouch)
      {
         if (currentState == EnemyState.Chase && playerMain.currentState != PlayerState.Hit && playerMain.currentState != PlayerState.God)
         {
            float directionToPlayer = Mathf.Sign(playerTransform.position.x - transform.position.x);
            
            playerHitScript.knockbacDirection = new Vector2(directionToPlayer, 1);
            playerHitScript.knockbackForce = onTouchKnockback;
            
            playerMain.currentState = PlayerState.Hit;
            playerHealth.TakeDamage(10);
            
         }
      }
   }

   private bool playerHitAttack;
   
   //used in an event in the attack animation
   void AttackHitBox()
   {
      playerHitAttack = Physics2D.OverlapArea(attackCollider.bounds.min, attackCollider.bounds.max, playerLayer);
      
      if (playerHitAttack && playerMain.currentState != PlayerState.Hit)
      {
         float directionToPlayer = Mathf.Sign(playerTransform.position.x - transform.position.x);
            
         playerHitScript.knockbacDirection = new Vector2(directionToPlayer, 1);
         playerHitScript.knockbackForce = attackKnockback;
         
         playerMain.currentState = PlayerState.Hit;
         playerHealth.TakeDamage(50);
      }
   }

   #endregion

   #region State Control

   void StateControl()
   {
      if (currentState == EnemyState.Patrol && IsPlayerSpotted())
      {
         patrolState.enabled = false;
         chaseState.enabled = true;
         currentState = EnemyState.Chase;
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

   Coroutine chaseExitTimerRoutine;
   IEnumerator ChaseStateExitTimer()
   {
      yield return new WaitForSeconds(chaseStateExitTime);
      chaseState.enabled = false;
      patrolState.enabled = true;
      currentState = EnemyState.Patrol;
   }

   #endregion
  
   
   
}
