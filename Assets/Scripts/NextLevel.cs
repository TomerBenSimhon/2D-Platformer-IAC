using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextLevel : MonoBehaviour
{
   [SerializeField] Vector2 startPosition;
   [SerializeField] String musicName;
   [SerializeField] float musicVolume;
   void OnTriggerEnter2D(Collider2D other)
   {
      GameManager.Instance.NextScene(startPosition);
      AudioManager.Instance.SetMusic(musicName, musicVolume);
   }
}
