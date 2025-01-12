using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.TextCore.Text;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class SwordProjectileBehavior : MonoBehaviour
{
   GameObject player;
   PlayerActions playerActions;
   Rigidbody2D rb;
   [SerializeField] Collider2D myCollider;
   
   [Header("Throw")]
   [SerializeField] float maxDistance = 20f; 
   [SerializeField] float retrivingAccel = 0f;
   [SerializeField] float moveSpeed = 50f;
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
        player = FindObjectOfType<PlayerMain>().gameObject;
        playerActions = player.GetComponent<PlayerActions>();
        rb = GetComponent<Rigidbody2D>();
        
        ricochetVelocity = Vector2.zero;
        
        ricochetLOS = LayerMask.GetMask("Ground", "Wall");
        
        hitEnemies.Clear();
        
        
        MoveToMouse();
    }

    
    void Update()
    {
        RetrieveToPlayer();
        WallChecking();
        
    }

    void LateUpdate()
    {
        GroundChecking();
    }

    private void OnDestroy()
    {
        Destroy(hitSparks.gameObject);
    }


    void MoveToMouse()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;

        Vector2 direction = mousePos - transform.position;
        if (!playerActions.groundDetected)
        {
            direction = (mousePos - transform.position);
        }
        else
        {
            if (direction.y < 0)
            {
                direction = new Vector2(direction.x, 0);
            }
            else
            {
                direction = mousePos - transform.position;
            }
        }
        
        
        
        rb.velocity = direction.normalized * moveSpeed;
        
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

    List<GameObject> hitEnemies = new List<GameObject>();
    GameObject currenEnemyToRicochet;
    LayerMask ricochetLOS;

    private List<GameObject> enemiesInScene = new List<GameObject>();
    
    private void OnTriggerStay2D(Collider2D other)
    {
        if (isRetriving && other.CompareTag("Player"))
        {
            player.GetComponent<PlayerMain>().SwordVisualsEnabled(true);
            
            Destroy(gameObject);
        }

        if (other.CompareTag("Enemy"))
        {
            if(hitEnemies.Contains(other.transform.parent.parent.gameObject)) {return;}
            
            if(moveToNextEnemy !=null) {StopCoroutine(moveToNextEnemy);}
            
            hitEnemies.Add(other.transform.parent.parent.gameObject);
            
            EnemyMain enemyMain = other.transform.parent.parent.GetComponent<EnemyMain>();
            
            Vector2 normal = (myCollider.transform.position - other.transform.position).normalized;
            Vector2 reflectedVelocity = (rb.velocity - 2 * Vector2.Dot(rb.velocity, normal) * normal);
            
            ricochetVelocity = new Vector2(Random.Range(reflectedVelocity.x * 0.75f, reflectedVelocity.x * 1.5f),
                                            Random.Range(reflectedVelocity.y, reflectedVelocity.y * 2f));
            
            float angle = Vector2.Angle(normal, Vector2.right);
            
            hitSparks.transform.position = other.transform.position;
            PlayHitSparks(angle - 30f, 0);
            GameManager.Instance.HitStop(0.08f);

            if (enemyMain.currentState == EnemyState.Stun)
            {
                StartCoroutine(ReStunEnemy(enemyMain));
            }
            else
            {
                enemyMain.currentState = EnemyState.Stun; 
            }

            if (CanRicochetToEnemy())
            {
                isRetriving = false;
                hasTouchedGround = true;
                moveToNextEnemy = StartCoroutine(MoveToNextEnemy());
            }
            else
            {
                isRetriving = true;
            }
        }
    }
    
    bool CanRicochetToEnemy()
    {
        enemiesInScene = FindObjectsOfType<EnemyMain>().Select(enemy => enemy.gameObject).ToList();

        foreach (GameObject enemy in enemiesInScene)
        {
            float distanceToEnemy = Vector2.Distance(transform.position, enemy.transform.position);
            Vector2 directionToEnemy = (enemy.transform.position - transform.position).normalized;
            EnemyMain enemyMain = enemy.GetComponent<EnemyMain>();


            if (hitEnemies.Count >= 3)
            {
                Debug.Log("we hit 3 already");
            }
            else if (enemyMain.currentState == EnemyState.Dead)
            {
                Debug.Log("he is dead");
            }
            else if (hitEnemies.Contains(enemy))
            {
                Debug.Log("we already hit him");
            }
            else if (distanceToEnemy > 10f)
            {
                Debug.Log("there is one but he is too far");
            }
            else if(Physics2D.Raycast(transform.position, directionToEnemy, distanceToEnemy, ricochetLOS))
            {
                Debug.Log("there is one but not in LOS");
            }
            else
            {
                Debug.Log("We found one!!!");
                currenEnemyToRicochet = enemy;
                return true;
            }

        }
        return false; 
      
    }

    
    private Coroutine moveToNextEnemy;
    IEnumerator MoveToNextEnemy()
    {
        float speed = 0;
        while (!isRetriving)
        {
            Vector2 direction = (currenEnemyToRicochet.transform.position - transform.position).normalized;
            rb.velocity = direction * speed + ricochetVelocity/2;
            
            speed += retrivingAccel* 2 * Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator ReStunEnemy(EnemyMain enemyMain)
    {
        enemyMain.currentState = EnemyState.Shocked;
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

    bool hasTouchedGround;
    Vector2 ricochetVelocity;
    RaycastHit2D groundHit;
    void GroundChecking()
    {
        groundHit = Physics2D.Raycast(myCollider.transform.position, Vector2.down, 0.5f, LayerMask.GetMask("Ground"));

        if (groundHit && !isRetriving && !hasTouchedGround)
        {
            hasTouchedGround = true;
            if (Mathf.Abs(rb.velocity.y) < 10)
            {
                ricochetVelocity = new Vector2(Random.Range(0.75f * rb.velocity.x, 1.5f * rb.velocity.x),-Random.Range(2f * rb.velocity.y, 4f * rb.velocity.y)); 
            }
            else
            {
                ricochetVelocity = new Vector2(Random.Range(0.75f * rb.velocity.x, 1.5f * rb.velocity.x),-Random.Range(rb.velocity.y, 2f * rb.velocity.y)); 
            }
            isRetriving = true;
            
            if (rb.velocity.y < 0) {PlayHitSparks(30f, 0);}
            else {PlayHitSparks(-120f, 0);}
            
        }
    }

    void PlayHitSparks(float rotation, float xPos)
    {
        hitSparks.transform.rotation = Quaternion.Euler(0, 0, rotation);
        hitSparks.transform.position += new Vector3(xPos, 0, 0);
        
        hitSparks.Play();
        hitSparks.transform.parent = null;
        
        //destroys onDestroy
    }

    
}
