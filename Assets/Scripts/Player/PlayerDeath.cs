using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerDeath : MonoBehaviour
{
    PlayerMain playerMain;
    private Rigidbody2D rb;

    [SerializeField] Animator playerAnimator;
    [SerializeField] SpriteRenderer swordRenderer;
    [SerializeField] float deathForce;
    [SerializeField] float slowForce;
    [SerializeField] GameObject[] objectsDestroyOnDeath;

    private void Awake()
    {
        playerMain = GetComponent<PlayerMain>();
        rb = GetComponent<Rigidbody2D>();
        
    }

    private void OnEnable()
    {
        Die();
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    void Die()
    {
        playerMain.SwordVisualsEnabled(false);
          
        playerAnimator.Play("Death");
        playerAnimator.speed = 1f;

        StartCoroutine(DeathMovement());
        StartCoroutine(RestartSceneDelay());
        
        playerMain.enabled = false;
      
        foreach (GameObject obj in objectsDestroyOnDeath)
        {
            obj.SetActive(false);
        }
        
        AudioManager.Instance.PlayPlayerSFX("Player_Killed", 0.1f,1f, 1f);
        AudioManager.Instance.LowerMusicOnDeath();

    }
    
    IEnumerator DeathMovement()
    {
        rb.velocity = Vector2.up * deathForce;

        while (true)
        {
            rb.velocity = Vector2.MoveTowards(rb.velocity, Vector2.zero, slowForce * Time.deltaTime);
            yield return null;
        }
    }

    IEnumerator RestartSceneDelay()
    {
        yield return new WaitForSeconds(2f);
        GameManager.Instance.RestartScene();
    }
}
