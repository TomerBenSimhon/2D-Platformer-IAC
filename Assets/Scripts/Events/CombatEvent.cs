using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CombatEvent : MonoBehaviour
{
   [SerializeField] Tilemap doorTilemap;
   
   [SerializeField] EnemyMain[] enemies;
   [SerializeField] Transform[] tilesToBreak;

   private void Update()
   {
      if (AreEnemiesDead() && !isCoroutine)
      {
         isCoroutine = true;
         StartCoroutine(BreakTiles());
      }
   }

   bool AreEnemiesDead()
   {
      foreach (EnemyMain enemy in enemies)
      {
         if (enemy.currentState == EnemyState.Dead)
         {
            continue;
         }
         else
         {
            return false;
         }
      }
      
      return true;
   }

   bool isCoroutine = false;
   IEnumerator BreakTiles()
   {
      yield return new WaitForSeconds(2f);
      AudioManager.Instance.PlayEnviromentSFX("Door_Break", 0.4f, 0.9f, 1.1f);

      foreach (Transform transform in tilesToBreak)
      {
         Vector3Int cell = doorTilemap.WorldToCell(transform.position);
         ParticleSystem effect = transform.gameObject.GetComponent<ParticleSystem>();
         
         if (doorTilemap.HasTile(cell))
         {
            doorTilemap.SetTile(cell, null); // Removes the tile at the specified position
            effect.Play();
            Debug.Log("Tile broken at: " + cell);
         }
         else
         {
            Debug.Log("No tile to break at: " + cell);
         }
      }
      
      Destroy(gameObject, 5f);
   }
}
