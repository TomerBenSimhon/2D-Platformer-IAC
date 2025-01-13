using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
   public static GameManager Instance;

   void Awake()
   {
      if (Instance != null && Instance != this)
      {
         Destroy(gameObject);
         return;
      }
      
      Instance = this;
      DontDestroyOnLoad(this);
   }
   

   void Update()
   {
      if (Input.GetKeyDown(KeyCode.R))
      {
         RestartScene();
      }
   }

   public void RestartScene()
   {
      EventManager.Instance.StopDisplayEvent();
      Time.timeScale = 1;
      SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
   }

   #region Checkpoint Manager

   public Vector2 currentCheckpoint = Vector2.zero;

   #endregion

   #region Effects

   public void HitStop(float time)
   {
      StartCoroutine(HitStopCoroutine(time));
   }

   public void HitStopScale(float time)
   {
      if (hitStopScaleCoroutine != null) { StopCoroutine(hitStopScaleCoroutine); }
      hitStopScaleCoroutine = StartCoroutine(HitStopScaleCoroutine(time));
   }

   IEnumerator HitStopCoroutine(float time)
   {
      Time.timeScale = 0;
      yield return new WaitForSecondsRealtime(time);
      Time.timeScale = 1;
   }

   Coroutine hitStopScaleCoroutine;
   IEnumerator HitStopScaleCoroutine(float time)
   {
      Time.timeScale = 0;
      float elapsedTime = 0;

      while (elapsedTime < time)
      {
         elapsedTime += Time.unscaledDeltaTime;
         
         Time.timeScale = elapsedTime / time < 1 ? elapsedTime / time : Time.timeScale;
         
         yield return null;
      }
      
      Time.timeScale = 1;
   }

   #endregion
   
}
