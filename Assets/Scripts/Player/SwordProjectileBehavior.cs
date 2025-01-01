using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordProjectileBehavior : MonoBehaviour
{
   GameObject player;
   Rigidbody2D rb;
   
   [Header("Throw")]
   [SerializeField] float maxDistance = 20f; 
   [SerializeField] float retrivingAccel = 0f;
   [SerializeField] private float moveSpeed = 50f;
   public bool isRetriving;
   
   [Header("Platform")]
   [SerializeField] private Collider2D wallCheck;
   [SerializeField] LayerMask walls;
   [SerializeField] private GameObject swordPlatform;
    void Start()
    {
        player = FindObjectOfType<PlayerMovement>().gameObject;
        rb = GetComponent<Rigidbody2D>();
        
        MoveToMouse();
    }

    
    void Update()
    {
        RetrieveToPlayer();
        WallChecking();
    }

    void MoveToMouse()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        
        Vector2 direction = (mousePos - transform.position).normalized;
        
        rb.velocity = direction * moveSpeed;
        
    }

    
    
    float retrivingSpeed;
    
    void RetrieveToPlayer()
    {
        if (DirectionToPlayer().magnitude > maxDistance && !isRetriving)
        {
            isRetriving = true;
            rb.velocity = Vector2.zero;
        }

        
        if (isRetriving)
        {
            rb.velocity = DirectionToPlayer().normalized * retrivingSpeed;
            retrivingSpeed += retrivingAccel * Time.deltaTime;

        }
        
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (isRetriving && other.CompareTag("Player"))
        {
            GameObject playerSwordVisualls = GameObject.FindWithTag("SwordVisualls");
        }
    }

    Vector2 DirectionToPlayer()
    {
        Vector2 distance = player.transform.position - transform.position;
        
        return distance;
        
    }

    private bool wallTouching;
    
    void WallChecking()
    {
        if (!isRetriving)
        {
            wallTouching = Physics2D.OverlapArea(wallCheck.bounds.min, wallCheck.bounds.max, walls);

            if (wallTouching)
            {
                GameObject instant = Instantiate(swordPlatform, transform.position, Quaternion.identity);
                instant.transform.localScale = new Vector3(Mathf.Sign(rb.velocity.x), 1, 1);
                Destroy(gameObject);
            }
            
        }
    }
    
   

  
}
