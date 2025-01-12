using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class DealDamage : MonoBehaviour
{
   [SerializeField] int damage;
   [SerializeField] CinemachineImpulseSource impulseSource;

   void OnTriggerEnter2D(Collider2D other)
   {
      if (other.CompareTag("Player"))
      {
         other.transform.parent.parent.GetComponent<Health>().TakeDamage(damage);
         CameraManager.Instance.CameraShake(impulseSource);
      }
   }
}
