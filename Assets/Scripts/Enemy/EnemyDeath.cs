using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyDeath : MonoBehaviour, IEnemyState
{
   public EnemyState StateId => EnemyState.Dead;
   public void Enter() => enabled = true;
   public void Exit() => enabled = false;
   
   [SerializeField] GameObject[] objectsDestroyOnDeath;
   private EnemyMain enemyMain;

   private void Awake()
   {
      enemyMain = GetComponent<EnemyMain>();
   }

   private void OnEnable()
   {
      Die();
      AudioManager.Instance.PlayEnemySFX("Enemy_Killed", 0.6f, 0.8f, 1.2f);
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
