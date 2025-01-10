using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DealDamage : MonoBehaviour
{
   [SerializeField] int damage;

   void OnTriggerEnter2D(Collider2D other)
   {
      if (other.CompareTag("Player"))
      {
         other.transform.parent.parent.GetComponent<Health>().TakeDamage(damage);
      }
   }
}
