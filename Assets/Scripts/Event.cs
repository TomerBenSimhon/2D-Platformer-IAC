using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Event : MonoBehaviour
{
   Collider2D eventCollider;
   
   [SerializeField] string[] messages;
   
   [SerializeField] bool destroyOnPlay = false;

   void Start()
   {
      eventCollider = GetComponent<Collider2D>();
      eventCollider.enabled = true;
   }

   void OnTriggerEnter2D(Collider2D other)
   {
      if (other.CompareTag("Player"))
      {
         EventManager.Instance.StartDisplayEvent(messages);
         
         if (destroyOnPlay) {eventCollider.enabled = false;}
      }
      
   }
}
