using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
   public static GameManager Instance;
   TextMeshProUGUI eventText;

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
   
   void Start()
   {
      eventText = FindObjectOfType<Canvas>().transform.Find("EventText").GetComponent<TextMeshProUGUI>();
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
      SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
   }

   #region Checkpoint Manager

   public Vector2 currentCheckpoint = Vector2.zero;

   #endregion
   
}
