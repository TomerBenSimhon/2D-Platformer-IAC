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
   private EnemyMain enemyMain;

   bool spottingPlayer = false;
   bool searchingPlayer = false;
   
   [Header("Chase Movement")]
   [SerializeField] float chaseSpeed = 5f;
   [SerializeField] float chaseAccel = 5f;
   
   [Header("Edge Checks")]
   [SerializeField] Collider2D edgeCheckerR;
   [SerializeField] Collider2D edgeCheckerL;
   [SerializeField] LayerMask ground;


   void Start()
   {
      player = FindObjectOfType<PlayerMovement>().gameObject;
      enemyMain = GetComponent<EnemyMain>();
   }
   
   private void OnEnable()
   {
      if(spotCoroutine != null) {StopCoroutine(spotCoroutine);}
      spotCoroutine = StartCoroutine(SpotPlayer());
   }


   void Update()
   {
      HandleAnimation();
   }
   private void FixedUpdate()
   {
      ChasePlayer();
      Debug.Log(BarrierDetection());
   }

   


   Coroutine spotCoroutine;
   IEnumerator SpotPlayer()
   {
      spottingPlayer = true;
      animator.Play("Shock");
      animator.speed = 1f;
      
      float elapsedTime = 0;
      while (elapsedTime < animator.GetCurrentAnimatorStateInfo(0).length)
      {
         elapsedTime += Time.deltaTime;
         animator.Play("Shock");
         yield return null;
      }
      spottingPlayer = false;
   }


   void ChasePlayer()
   {
      rb.velocity = new Vector2(HandleMoveToPlayer() * chaseSpeed, rb.velocity.y);
      
      if (!enemyMain.CanSeePlayer())
      {
         searchingPlayer = true;
      }
      else
      {
         searchingPlayer = false;
      }
   }

   float directionToPlayer;
   float moveAxisToPlayer;
   private float target;
   float HandleMoveToPlayer()
   {
      directionToPlayer = Mathf.Sign(player.transform.position.x - transform.position.x);
      if (searchingPlayer || spottingPlayer)
      {
         target = 0;
      }
      else
      {
         if ((directionToPlayer < 0 && BarrierDetection() == "L") || (directionToPlayer > 0 && BarrierDetection() == "R"))
         {
            target = 0;
         }
         else
         {
            target = directionToPlayer;
         }
      }
      
      moveAxisToPlayer = Mathf.MoveTowards(moveAxisToPlayer, target, Time.fixedDeltaTime * chaseAccel);
      
      return moveAxisToPlayer;
   }

   void HandleVisualsFlip()
   {
      visuals.transform.localScale = new Vector3(Mathf.Sign(directionToPlayer), 1, 1);
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

      if (noEdgeL || wallL)
      {
         return "L";
      }
      else if (noEdgeR || wallR)
      {
         return "R";
      }
      return "null";
   }

   void HandleAnimation()
   {
      if (rb.velocity.x != 0 && !searchingPlayer && !spottingPlayer)
      {
         animator.Play("Run");
         animator.speed = 1.2f;
         HandleVisualsFlip();
      }
      else
      {
         animator.Play("Idle");
         animator.speed = 1f;
      }
   }

   
}




















