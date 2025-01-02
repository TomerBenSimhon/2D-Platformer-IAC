using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMain : MonoBehaviour
{
   GameObject player;
   [SerializeField] GameObject visuals;
   
   [SerializeField] PatrolState patrolState;
   [SerializeField] ChaseState chaseState;
   
   [Header("Player Detection")]
   [SerializeField] float maxDetectionDistance;
   [SerializeField] float spottingFOV;
   [SerializeField] LayerMask obstacleLayer;

   [SerializeField] float chaseStateExitTime;

   void Start()
   {
      player = FindObjectOfType<PlayerMovement>().gameObject;
      
      patrolState.enabled = true;
      chaseState.enabled = false;
   }

   void Update()
   {
      StateControl();
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

   bool CanSeePlayer()
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
   
   private bool IsFacingPlayer()
   {
      float directionToPlayer = player.transform.position.x - transform.position.x;
      float enemyFacingDirection = visuals.transform.localScale.x;

      return Mathf.Approximately(Mathf.Sign(directionToPlayer), Mathf.Sign(enemyFacingDirection));

   }

   #endregion

   #region State Control

   void StateControl()
   {
      if (patrolState.enabled && IsPlayerSpotted())
      {
         patrolState.enabled = false;
         chaseState.enabled = true;
      }

      if (chaseState.enabled)
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
   }

   #endregion
  
   
   
}
