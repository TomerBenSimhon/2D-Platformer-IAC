using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
   public static CameraManager Instance;
   
   [Header("Main Camera")]
   [SerializeField] CinemachineVirtualCamera mainVCam;
   private void Awake()
   {
      if (Instance == null)
      {
         Instance = this;
      }
   }
   

   public void CameraShake(CinemachineImpulseSource impulseSource)
   {
      impulseSource.GenerateImpulse();
   }
}
