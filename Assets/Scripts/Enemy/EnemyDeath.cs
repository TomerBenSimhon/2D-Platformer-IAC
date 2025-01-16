using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

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
      AudioManager.Instance.PlaySFX("Enemy_Killed", 0.6f, Random.Range(0.9f, 1.1f));
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
