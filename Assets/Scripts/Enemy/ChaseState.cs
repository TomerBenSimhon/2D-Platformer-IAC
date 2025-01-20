using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ChaseState : MonoBehaviour
{
   [SerializeField] Rigidbody2D rb;
   [SerializeField] Animator animator;
   [SerializeField] GameObject visuals;
   
   GameObject player;
   EnemyMain enemyMain;
   
   bool searchingPlayer = false;
   bool isAttacking = false;
   
   [Header("Chase Movement")]
   [SerializeField] float chaseSpeed = 5f;
   [SerializeField] float chaseAccel = 5f;
   
   [Header("Edge Checks")]
   [SerializeField] Collider2D edgeCheckerR;
   [SerializeField] Collider2D edgeCheckerL;
   [SerializeField] LayerMask ground;

   [Header("Attack")] 
   [SerializeField] float attackRange;
   [SerializeField] float attackCooldown;
   

   private void Awake()
   {
      player = FindObjectOfType<PlayerMovement>().gameObject;
      enemyMain = GetComponent<EnemyMain>();  
   }

   private void OnDisable()
   {
      target = 0;
      moveAxisToPlayer = 0;
   }


   void Update()
   {
      HandleAttack();
      HandleAnimation();
   }
   private void FixedUpdate()
   {
      ChasePlayer();
   }

   
   


   void ChasePlayer()
   {
      if (isAttackLunge)
      {
         rb.velocity = new Vector2(HandleMoveToPlayer() * chaseSpeed * 1.75f, rb.velocity.y);
      }
      else
      {
         rb.velocity = new Vector2(HandleMoveToPlayer() * chaseSpeed, rb.velocity.y);
      }
      
      
      if (!enemyMain.CanSeePlayer() && !searchingPlayer)
      {
         searchingPlayer = true;
         lastKnownTargetPosition = player.transform.position.x;
      }
      else if (searchingPlayer && enemyMain.CanSeePlayer())
      {
         searchingPlayer = false;
      }
   }

   float xDistanceToPlayer;
   float moveAxisToPlayer;
   private float target;
   float lastKnownTargetPosition;
   float HandleMoveToPlayer()
   {
      xDistanceToPlayer = player.transform.position.x - transform.position.x;
      
      if (isAttacking)
      {
         target = 0;
      }
      else
      {
         if ((xDistanceToPlayer < 0 && BarrierDetection() == "L") || (xDistanceToPlayer > 0 && BarrierDetection() == "R"))
         {
            target = 0;
            if (!searchingPlayer && !isAttackLunge)
            {
               HandleVisualsFlip();
            }
         }
         else if (BarrierDetection() == "Both")
         {
            target = 0;
            if (!searchingPlayer && !isAttackLunge)
            {
               HandleVisualsFlip();
            }
         }
         else
         {
            if (searchingPlayer)
            {
               target = CalcPlayersLastKnownPosition();
               visuals.transform.localScale = new Vector3(Mathf.Sign(target), 1, 1);
            }
            else if (isAttackLunge)
            {
               target = visuals.transform.localScale.x;
            }
            else
            {
               target = Mathf.Sign(xDistanceToPlayer);
               HandleVisualsFlip();
            }
         }
      }

      if (isAttackLunge)
      {
         moveAxisToPlayer = Mathf.MoveTowards(moveAxisToPlayer, target, Time.fixedDeltaTime * chaseAccel * 3f);
      }
      else
      {
         moveAxisToPlayer = Mathf.MoveTowards(moveAxisToPlayer, target, Time.fixedDeltaTime * chaseAccel);
      }
      
      return moveAxisToPlayer;
   }

   float CalcPlayersLastKnownPosition()
   {
      float direction = Mathf.Sign(lastKnownTargetPosition - transform.position.x);
      if ((direction < 0 && BarrierDetection() == "L") || (direction > 0 && BarrierDetection() == "R"))
      {
         return 0;
      }

      if (Mathf.Abs(transform.position.x - lastKnownTargetPosition) > 0.5f)
      {
         return direction;
      }
      return 0;
   }


   float currentAttackTime;
   void HandleAttack()
   {
      if (Vector2.Distance(transform.position, player.transform.position) < attackRange && !isAttacking && !searchingPlayer && Time.time > currentAttackTime + attackCooldown)
      {
         if (attackCoroutine == null)
         {
            currentAttackTime = Time.time;
            attackCoroutine = StartCoroutine(AttackVisuals());
         }
      }
   }


    bool isAttackLunge;
   Coroutine attackCoroutine;
   /*Handles the visuals of attacking
   the attack hit box method is in enemyMain and is called in the attack animation in an event*/
   IEnumerator AttackVisuals()
   {
      yield return null;
      
      isAttacking = true;
      animator.Play("Attack");
      animator.speed = 1f;
      
      yield return new WaitForSeconds(0.15f - Time.deltaTime);
      
      isAttacking = false;
      isAttackLunge = true;
     
      yield return new WaitForSeconds(0.4f);
     
      isAttackLunge = false;
      isAttacking = true;
      
      float elapsedTime = 0;
      
      
      yield return null;
      
      float animationTime = animator.GetCurrentAnimatorStateInfo(0).length;
      
      
      while (elapsedTime < animationTime - 0.55f)
      {
         elapsedTime += Time.deltaTime;
         
         yield return null;
      }
      isAttacking = false;
      
      attackCoroutine = null;
   }

  
   
   

   void HandleVisualsFlip()
   {
      visuals.transform.localScale = new Vector3(Mathf.Sign(xDistanceToPlayer), 1, 1);
   }

   
   bool noEdgeR; 
   bool noEdgeL;

   bool wallR;
   bool wallL;
   string BarrierDetection()
   {
      noEdgeL = !Physics2D.OverlapArea(edgeCheckerL.bounds.min, edgeCheckerL.bounds.max, ground);
      noEdgeR = !Physics2D.OverlapArea(edgeCheckerR.bounds.min, edgeCheckerR.bounds.max, ground);
        
      wallL = Physics2D.Raycast(edgeCheckerL.bounds.center, Vector2.left, 0.1f, ground);
      wallR = Physics2D.Raycast(edgeCheckerR.bounds.center, Vector2.right, 0.1f, ground);

      if ((noEdgeL || wallL) && (noEdgeR || wallR))
      {
         return "Both";
      }
      if (noEdgeL || wallL)
      {
         return "L";
      }
      if (noEdgeR || wallR)
      {
         return "R";
      }
      return "null";
   }

   void HandleAnimation()
   {
      if (isAttacking || isAttackLunge)
      {
         animator.Play("Attack");
         animator.speed = 1f;
      }
      else if (Mathf.Abs(rb.velocity.x) > 0.1f)
      {
         animator.Play("Run");
         animator.speed = 1.25f;
         
      }
      else
      {
         animator.Play("Idle");
         animator.speed = 1.25f;
      }
   }

   
}




















