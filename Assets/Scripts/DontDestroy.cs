using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroy : MonoBehaviour
{
   static DontDestroy Instance;

   void Awake()
   {
      if (Instance == null)
      {
         Instance = this;
         DontDestroyOnLoad(this);
      }
      else
      {
         Destroy(gameObject);
      }
   }
}
