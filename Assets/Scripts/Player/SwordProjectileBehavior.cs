using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class SwordProjectileBehavior : MonoBehaviour
{
   GameObject player;
   Rigidbody2D rb;
   [SerializeField] Collider2D myCollider;
   
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
   
   [Header("Effect")]
   [SerializeField] ParticleSystem hitSparks;
   
   
   
    void Start()
    {
        player = FindObjectOfType<PlayerMovement>().gameObject;
        rb = GetComponent<Rigidbody2D>();
        
        ricochetVelocity = Vector2.zero;
        hitEnemy = false;
        
        MoveToMouse();
    }

    
    void Update()
    {
        RetrieveToPlayer();
        WallChecking();
        GroundChecking();
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
            rb.velocity = DirectionToPlayer().normalized * retrivingSpeed + ricochetVelocity/2;
            retrivingSpeed += retrivingAccel * Time.deltaTime;

        }
        
    }

    
    //Collisions

    bool hitEnemy;
    private void OnTriggerStay2D(Collider2D other)
    {
        if (isRetriving && other.CompareTag("Player"))
        {
            player.GetComponent<PlayerMain>().SwordVisualsEnabled(true);
            Destroy(gameObject);
        }

        if (!hitEnemy && other.CompareTag("Enemy"))
        {
            hitEnemy = true;
            
            EnemyMain enemyMain = other.transform.parent.parent.GetComponent<EnemyMain>();
            
            Vector2 normal = (myCollider.transform.position - other.transform.position).normalized;
            Vector2 reflectedVelocity = (rb.velocity - 2 * Vector2.Dot(rb.velocity, normal) * normal);
            
            ricochetVelocity = new Vector2(Random.Range(reflectedVelocity.x * 0.75f, reflectedVelocity.x * 1.5f),
                                            Random.Range(reflectedVelocity.y, reflectedVelocity.y * 2f));

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

            Tilemap wallTilemap;

            if (wallTouchingRight && rb.velocity.x > 0)
            {
                
                wallTilemap = wallTouchingRight.GetComponent<Tilemap>();
                Vector3 cellPos = wallTilemap.WorldToCell(transform.position);
                
                GameObject instant = Instantiate(swordPlatform, new Vector2(cellPos.x + 0.3f,transform.position.y + yOffset), Quaternion.identity);
                instant.transform.localScale = new Vector3(1, 1, 1);
                
                Destroy(gameObject);
            }

            if (wallTouchingLeft && rb.velocity.x < 0)
            {
                
                wallTilemap = wallTouchingLeft.GetComponent<Tilemap>();
                Vector3 cellPos = wallTilemap.WorldToCell(transform.position + Vector3.right);
                
                GameObject instant = Instantiate(swordPlatform, new Vector2(cellPos.x - 0.3f,transform.position.y + yOffset), Quaternion.identity);
                instant.transform.localScale = new Vector3(-1, 1, 1);
                
                Destroy(gameObject);
            }
            
        }
    }

    
    Vector2 ricochetVelocity;
    RaycastHit2D groundHit;
    void GroundChecking()
    {
        groundHit = Physics2D.Raycast(myCollider.transform.position, Vector2.down, 0.5f, LayerMask.GetMask("Ground"));

        if (groundHit && !isRetriving)
        {
            PlayHitSparks(30f, 0);
            
            if (Mathf.Abs(rb.velocity.y) < 10)
            {
                ricochetVelocity = new Vector2(Random.Range(0.75f * rb.velocity.x, 1.5f * rb.velocity.x),-Random.Range(2f * rb.velocity.y, 4f * rb.velocity.y)); 
            }
            else
            {
                ricochetVelocity = new Vector2(Random.Range(0.75f * rb.velocity.x, 1.5f * rb.velocity.x),-Random.Range(rb.velocity.y, 2f * rb.velocity.y)); 
            }
            isRetriving = true;
        }
    }

    void PlayHitSparks(float rotation, float xPos)
    {
        hitSparks.transform.rotation = Quaternion.Euler(0, 0, rotation);
        hitSparks.transform.position += new Vector3(xPos, 0, 0);
        
        hitSparks.Play();
        hitSparks.transform.parent = null;
        Destroy(hitSparks.gameObject, 1f);
    }

    
}
