using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SwordPlatformBehavior : MonoBehaviour
{
    [SerializeField] private GameObject swordProjectile;
    [SerializeField] LayerMask wallLayer;
    
    [SerializeField] ParticleSystem hitSparks;

    Collider2D myCollider;
    
    GameObject player;
    private void Start()
    {
        myCollider = GetComponentInChildren<Collider2D>();
        player = FindObjectOfType<PlayerMain>().gameObject;
        
        hitSparks.Play();
        hitSparks.transform.parent = null;
        Destroy(hitSparks.gameObject, 1f);
        
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire2"))
        {
            AudioManager.Instance.PlayPlayerSFX("Sword_Throw", 0.6f, 0.8f, 1f);
            
            GameObject instant = Instantiate(swordProjectile, transform.position, Quaternion.identity);
       
            instant.GetComponent<SwordProjectileBehavior>().isRetriving = true;
            Destroy(gameObject);
        }

        if (Vector2.Distance(player.transform.position, transform.position) > 18f)
        {
            AudioManager.Instance.PlayPlayerSFX("Sword_Throw", 0.6f, 0.8f, 1f);
            
            GameObject instant = Instantiate(swordProjectile, transform.position, Quaternion.identity);
       
            instant.GetComponent<SwordProjectileBehavior>().isRetriving = true;
            Destroy(gameObject);
        }
    }


 
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
}
