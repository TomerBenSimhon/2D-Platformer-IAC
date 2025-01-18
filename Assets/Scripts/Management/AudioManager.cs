using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class AudioManager : MonoBehaviour
{
   [SerializeField] AudioSource playerSfxSource;
   [SerializeField] AudioSource enemySfxSource;
   [SerializeField] AudioSource environmentSfxSource;
   [SerializeField] AudioSource musicSource;
   
   
   public static AudioManager Instance;

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

   public void PlayPlayerSFX(string sfxName, float volume, float minPitch, float maxPitch)
   {
      playerSfxSource.pitch = Random.Range(minPitch, maxPitch);
      playerSfxSource.volume = volume * 2f;
      
      
      AudioClip clip = Resources.Load<AudioClip>("SFX/" + sfxName);
      playerSfxSource.PlayOneShot(clip);
   }

   public void PlayEnemySFX(string sfxName, float volume, float minPitch, float maxPitch)
   {
      enemySfxSource.pitch = Random.Range(minPitch, maxPitch);
      enemySfxSource.volume = volume * 2f;
      
      
      AudioClip clip = Resources.Load<AudioClip>("SFX/" + sfxName);
      enemySfxSource.PlayOneShot(clip);
   }
   
   public void PlayEnviromentSFX(string sfxName, float volume, float minPitch, float maxPitch)
   {
      environmentSfxSource.pitch = Random.Range(minPitch, maxPitch);
      environmentSfxSource.volume = volume * 2f;
      
      
      AudioClip clip = Resources.Load<AudioClip>("SFX/" + sfxName);
      environmentSfxSource.PlayOneShot(clip);
   }
   public void LowerMusicOnDeath()
   {
      if(lowerMusicCoroutine != null) { StopCoroutine(lowerMusicCoroutine); }
      
      lowerMusicCoroutine = StartCoroutine(LowerMusicOnDeathRoutine());
   }

   Coroutine lowerMusicCoroutine;
   IEnumerator LowerMusicOnDeathRoutine()
   {
      float elapsedTime = 0f;
      float volume = musicSource.volume;

      while (elapsedTime < 0.25f)
      {
         musicSource.volume = Mathf.Lerp(volume, 0f, elapsedTime / 0.25f);
         elapsedTime += Time.deltaTime;
         yield return null;
      }
      
      yield return new WaitForSeconds(1.5f);
      elapsedTime = 0f;
      
      while (elapsedTime < 0.25f)
      {
         musicSource.volume = Mathf.Lerp(0, volume, elapsedTime / 0.25f);
         elapsedTime += Time.deltaTime;
         yield return null;
      }
   }
}
