using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Event : MonoBehaviour
{
   Collider2D eventCollider;
   
   [SerializeField] string[] messages;
   
   [SerializeField] bool destroyOnPlay = false;
   [SerializeField] bool isFreezeTime = false;

   void Start()
   {
      eventCollider = GetComponent<Collider2D>();
      eventCollider.enabled = true;
   }

   void OnTriggerEnter2D(Collider2D other)
   {
      if (other.CompareTag("Player"))
      {
         EventManager.Instance.StartDisplayEvent(messages, isFreezeTime);
         
         if (destroyOnPlay) {eventCollider.enabled = false;}
      }
      
   }
}
