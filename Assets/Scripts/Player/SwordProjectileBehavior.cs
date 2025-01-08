using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

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
   [SerializeField] private Collider2D wallCheckRight;
   [SerializeField] private Collider2D wallCheckLeft;
   [SerializeField] LayerMask walls;
   [SerializeField] private GameObject swordPlatform;

   [SerializeField] private float yOffset;
   
   
   
   
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

    
    //Collisions
    private void OnTriggerStay2D(Collider2D other)
    {
        if (isRetriving && other.CompareTag("Player"))
        {
            player.GetComponent<PlayerActions>().SwordVisuallsActive(true);
            Destroy(gameObject);
        }

        if (!isRetriving && other.CompareTag("Enemy"))
        {
            EnemyMain enemyMain = other.transform.parent.parent.GetComponent<EnemyMain>();

            if (enemyMain.currentState == EnemyState.Stun)
            {
                StartCoroutine(ReStuneEnemy(enemyMain));
            }
            else
            {
                enemyMain.currentState = EnemyState.Stun; 
            }
            
            isRetriving = true;
        }
    }

    IEnumerator ReStuneEnemy(EnemyMain enemyMain)
    {
        enemyMain.currentState = EnemyState.Chase;
        yield return null;
        enemyMain.currentState = EnemyState.Stun;
    }

    Vector2 DirectionToPlayer()
    {
        Vector2 distance = player.transform.position - transform.position;
        
        return distance;
        
    }

    Collider2D wallTouchingRight;
    Collider2D wallTouchingLeft;
    
    void WallChecking()
    {
        if (!isRetriving)
        {
            wallTouchingRight = Physics2D.OverlapArea(wallCheckRight.bounds.min, wallCheckRight.bounds.max, walls);
            wallTouchingLeft = Physics2D.OverlapArea(wallCheckLeft.bounds.min, wallCheckLeft.bounds.max, walls);

            if (wallTouchingRight && rb.velocity.x > 0)
            {
                GameObject instant = Instantiate(swordPlatform, new Vector2(transform.position.x,transform.position.y + yOffset), Quaternion.identity);
                instant.transform.localScale = new Vector3(1, 1, 1);
                Destroy(gameObject);
            }

            if (wallTouchingLeft && rb.velocity.x < 0)
            {
                GameObject instant = Instantiate(swordPlatform, new Vector2(transform.position.x,transform.position.y + yOffset), Quaternion.identity);
                instant.transform.localScale = new Vector3(-1, 1, 1);
                Destroy(gameObject);
            }
            
        }
    }

    
}
