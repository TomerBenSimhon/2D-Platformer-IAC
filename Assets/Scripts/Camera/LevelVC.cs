using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class LevelVC : MonoBehaviour
{
  
  CinemachineVirtualCamera vcam;

  void Start()
  {
    vcam = GetComponent<CinemachineVirtualCamera>();
    vcam.enabled = false;
  }
  private void OnTriggerEnter2D(Collider2D other)
  {
    if (other.CompareTag("Player"))
    {
      vcam.enabled = true;
    }
  }

  void OnTriggerExit2D(Collider2D other)
  {
    if (other.CompareTag("Player"))
    {
      vcam.enabled = false;
    }
  }
}
