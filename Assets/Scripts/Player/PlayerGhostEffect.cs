using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlayerGhostEffect : MonoBehaviour
{
   [SerializeField] SpriteRenderer spriteRenderer1;
   [SerializeField] SpriteRenderer spriteRenderer2;
   [SerializeField] Light2D light;
   
   SpriteRenderer playerSpriteRenderer;
   SpriteRenderer swordSpriteRenderer;
   GameObject player;
   
   [SerializeField] float ghostDuration;

   void Start()
   {
      player = FindObjectOfType<PlayerMain>().gameObject;
      playerSpriteRenderer = player.GetComponentInChildren<SpriteRenderer>();
      swordSpriteRenderer = player.GetComponentsInChildren<SpriteRenderer>()[1];
      
      StartCoroutine(GhostEffect());
   }

   IEnumerator GhostEffect()
   {
      spriteRenderer1.sprite = playerSpriteRenderer.sprite;
      spriteRenderer2.sprite = swordSpriteRenderer.sprite;
      transform.localScale = player.transform.localScale;
      
      Color spriteColor = spriteRenderer1.color;
      spriteColor.a = 0.33f;
      spriteRenderer1.color = spriteColor;
      spriteRenderer2.color = spriteColor;
      
      float elapsedTime = 0;

      while (elapsedTime < ghostDuration / 2)
      {
         yield return new WaitForSeconds(ghostDuration / 2);

         while (elapsedTime < ghostDuration / 2)
         {
            elapsedTime += Time.deltaTime;
            spriteColor.a = (1 - elapsedTime / (ghostDuration / 2)) / 3;
            light.intensity = Mathf.Lerp(light.intensity, 0, elapsedTime / ghostDuration / 2);
            
            spriteRenderer1.color = spriteColor;
            spriteRenderer2.color = spriteColor;
            yield return null;
         }
      }
      
      Destroy(gameObject);
   }
}
