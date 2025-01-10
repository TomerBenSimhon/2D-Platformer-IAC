using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordPlatformBehavior : MonoBehaviour
{
    [SerializeField] private GameObject swordProjectile;
    [SerializeField] LayerMask wallLayer;
    
    [SerializeField] ParticleSystem hitSparks;

    Collider2D myCollider;
    private void Start()
    {
        myCollider = GetComponentInChildren<Collider2D>();
        
        
        hitSparks.Play();
        hitSparks.transform.parent = null;
        Destroy(hitSparks.gameObject, 1f);
        
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire2"))
        {
           GameObject instant = Instantiate(swordProjectile, transform.position, Quaternion.identity);
           
           instant.GetComponent<SwordProjectileBehavior>().isRetriving = true;
           Destroy(gameObject);
        }
    }


 
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
}
