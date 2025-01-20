using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEditor;
using UnityEngine.UI;

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
   
   
   [SerializeField] Texture2D cursorTexture;
   private Vector2 cursorHotspot;

   void Start()
   {
      cursorHotspot = new Vector2(cursorTexture.width / 2, cursorTexture.height / 2);

      Cursor.SetCursor(cursorTexture, cursorHotspot, CursorMode.Auto);
   }

   void Update()
   {
      if (Input.GetKeyDown(KeyCode.R))
      {
         SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
      }

      if (Input.GetKeyDown(KeyCode.Escape))
      {
         EditorApplication.isPaused = !EditorApplication.isPaused;
      }
   }

   public void RestartScene()
   {
      if (restartCoroutine != null)
      {
         StopCoroutine(restartCoroutine);
      }

      restartCoroutine = StartCoroutine(RestartSceneRoutine());
   }

   
   Coroutine restartCoroutine;
   IEnumerator RestartSceneRoutine()
   {
      if (EventManager.Instance != null)
      {
         EventManager.Instance.StopDisplayEvent();
         EventManager.Instance.DisableEvents();
      }
      
      Time.timeScale = 1;

      StartCoroutine(FillBar(Slider.Direction.TopToBottom));
      yield return new WaitForSecondsRealtime(fillDuration);
      
      yield return SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
      yield return new WaitForSecondsRealtime(0.2f);
      
      StartCoroutine(UnFillBar(Slider.Direction.TopToBottom));
      
   }

   public void NextScene(Vector2 startPos)
   {
      if (nextSceneRoutine != null)
      {
         StopCoroutine(nextSceneRoutine);
      }
      nextSceneRoutine = StartCoroutine(NextSceneRoutine(startPos));
   }

   
   Coroutine nextSceneRoutine;
   IEnumerator NextSceneRoutine(Vector2 startPos)
   {
      yield return new WaitForSecondsRealtime(0.2f);

      StartCoroutine(FillBar(Slider.Direction.RightToLeft));
      yield return new WaitForSecondsRealtime(fillDuration);
      
      yield return SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
      yield return new WaitForSecondsRealtime(0.2f);
      
      StartCoroutine(UnFillBar(Slider.Direction.LeftToRight));
   }
   

   #region Checkpoint Manager

   public Vector2 currentCheckpoint = Vector2.zero;

   #endregion

   #region Effects

   public void HitStop(float time)
   {
      StartCoroutine(HitStopCoroutine(time));
   }

   public void HitStopScale(float time, float hitStopTime)
   {
      if (hitStopScaleCoroutine != null) { StopCoroutine(hitStopScaleCoroutine); }
      hitStopScaleCoroutine = StartCoroutine(HitStopScaleCoroutine(time, hitStopTime));
   }

   IEnumerator HitStopCoroutine(float time)
   {
      Time.timeScale = 0;
      yield return new WaitForSecondsRealtime(time);
      Time.timeScale = 1;
   }

   Coroutine hitStopScaleCoroutine;
   IEnumerator HitStopScaleCoroutine(float scaleTime, float hitStopTime)
   {
      Time.timeScale = 0;
      
      yield return new WaitForSecondsRealtime(hitStopTime);
      
      float elapsedTime = 0;

      while (elapsedTime < scaleTime)
      {
         elapsedTime += Time.unscaledDeltaTime;
         
         Time.timeScale = elapsedTime / scaleTime < 1 ? elapsedTime / scaleTime : Time.timeScale;
         
         yield return null;
      }
      
      Time.timeScale = 1;
   }

   #endregion

   #region Black Bar Animation

   [SerializeField] Slider blackBar;
   [SerializeField] float fillDuration = 0.5f;

   IEnumerator FillBar(Slider.Direction direction)
   {
      blackBar.value = 0;
      blackBar.direction = direction;
      
      float elapsedTime = 0;

      while (elapsedTime < fillDuration)
      {
         elapsedTime += Time.unscaledDeltaTime;
         blackBar.value = Mathf.Lerp(0, 1, elapsedTime / fillDuration);
         yield return null;
      }
      
      blackBar.value = 1;
   }

   IEnumerator UnFillBar(Slider.Direction direction)
   {
      blackBar.value = 0;
      blackBar.direction = direction;
      
      float elapsedTime = 0;

      while (elapsedTime < fillDuration)
      {
         elapsedTime += Time.unscaledDeltaTime;
         blackBar.value = Mathf.Lerp(1, 0, elapsedTime / fillDuration);
         yield return null;
      }
      
      blackBar.value = 0;
   }
   

   #endregion
   
}
