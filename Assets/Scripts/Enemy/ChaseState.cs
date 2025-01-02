using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ChaseState : MonoBehaviour
{
   [SerializeField] Rigidbody2D rb;
   [SerializeField] Animator animator;
   [SerializeField] GameObject visuals;
   
   [SerializeField] GameObject player;

   bool spottingPlayer = false;
   bool chasingPlayer = false;
   bool searchingPlayer = false;
   
   [Header("Chase Movement")]
   [SerializeField] float chaseSpeed = 5f;
   [SerializeField] float chaseAccel = 5f;


   void Start()
   {
      player = FindObjectOfType<PlayerMovement>().gameObject;
   }
   
   private void OnEnable()
   {
      if(spotCoroutine != null) {StopCoroutine(spotCoroutine);}
      spotCoroutine = StartCoroutine(SpotPlayer());
   }

   

   private void FixedUpdate()
   {
      ChasePlayer();
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
      if (!spottingPlayer && !searchingPlayer)
      {
         rb.velocity = new Vector2(MoveToPlayerLerp() * chaseSpeed, rb.velocity.y);
         animator.Play("Run");
         animator.speed = 1f;
      }
      else if (searchingPlayer)
      {
         animator.Play("Idle");
         animator.speed = 1f;
      }
   }

   float moveAxisToPlayer;
   float MoveToPlayerLerp()
   {
      float directionToPlayer = Mathf.Sign(player.transform.position.x - transform.position.x);
      
      moveAxisToPlayer = Mathf.MoveTowards(moveAxisToPlayer, directionToPlayer, Time.deltaTime * chaseAccel);
      
      return moveAxisToPlayer;
   }

   
}




















