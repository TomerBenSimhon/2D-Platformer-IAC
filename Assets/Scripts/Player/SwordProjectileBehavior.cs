using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordProjectileBehavior : MonoBehaviour
{
   GameObject player;
   Rigidbody2D rb;

   [SerializeField] private float moveSpeed = 50f;
    void Start()
    {
        player = FindObjectOfType<PlayerMovement>().gameObject;
        rb = GetComponent<Rigidbody2D>();
        
        MoveToMouse();
    }

    
    void Update()
    {
        RetriveToPlayer();
    }

    void MoveToMouse()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        
        Vector2 direction = (mousePos - transform.position).normalized;
        
        rb.velocity = direction * moveSpeed;
        
    }

    public bool isRetriving { get; protected set; }
    [SerializeField] float maxDistance = 20f; 
    [SerializeField] float retrivingAccel = 0f;
    float retrivingSpeed;
    
    void RetriveToPlayer()
    {
        if (DistanceFromPlayer().magnitude > maxDistance && !isRetriving)
        {
            isRetriving = true;
            rb.velocity = Vector2.zero;
        }

        
        if (isRetriving)
        {
            rb.velocity = DistanceFromPlayer().normalized * retrivingSpeed;
            retrivingSpeed += retrivingAccel * Time.deltaTime;

        }
        
    }
    
    Vector2 DistanceFromPlayer()
    {
        Vector2 distance = player.transform.position - transform.position;
        
        return distance;
        
    }
    
   

  
}
