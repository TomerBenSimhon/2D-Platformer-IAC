using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordPlatformBehavior : MonoBehaviour
{
    [SerializeField] private GameObject swordProjectile;
    [SerializeField] LayerMask wallLayer;

    Collider2D myCollider;
    private void Start()
    {
        myCollider = GetComponentInChildren<Collider2D>();
        StartCoroutine(AdjustPosition());
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


    bool isTouchingWall;
    IEnumerator AdjustPosition()
    {
        isTouchingWall = true;

        if (transform.localScale.x > 0)
        {
            while (isTouchingWall)
            {
                isTouchingWall = Physics2D.OverlapArea(myCollider.bounds.min, myCollider.bounds.max, wallLayer);
                transform.position += Vector3.left * Time.deltaTime;
            }

            yield return null;
        }
        else if (transform.localScale.x < 0)
        {
            while (isTouchingWall)
            {
                isTouchingWall = Physics2D.OverlapArea(myCollider.bounds.min, myCollider.bounds.max, wallLayer);
                transform.position += Vector3.right * Time.deltaTime;
            }
            yield return null;
        }
        yield return null;
    }
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
}
