using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDeath : MonoBehaviour
{
   private EnemyMain enemyMain;
   
   [SerializeField] GameObject[] objectsDestroyOnDeath;

   private void Awake()
   {
      enemyMain = GetComponent<EnemyMain>();
   }

   private void OnEnable()
   {
      Die();
   }
   
   void Die()
   {
      
      Animator animator = GetComponent<Animator>();
      animator.Play("Death");
      animator.speed = 1f;
      
      enemyMain.enabled = false;
      
      foreach (GameObject obj in objectsDestroyOnDeath)
      {
         obj.SetActive(false);
      }

   }
}
