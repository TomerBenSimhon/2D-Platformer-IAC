using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
   [SerializeField] AudioSource sfxSource;
   [SerializeField] AudioSource musicSource;
   
   
   public static AudioManager Instance;

   void Awake()
   {
      if (Instance == null)
      {
         Instance = this;
      }
   }

   public void PlaySFX(string sfxName, float volume, float pitch)
   {
      sfxSource.pitch = pitch;
      sfxSource.volume = volume;
      
      
      AudioClip clip = Resources.Load<AudioClip>("SFX/" + sfxName);
      sfxSource.PlayOneShot(clip);
   }
}
