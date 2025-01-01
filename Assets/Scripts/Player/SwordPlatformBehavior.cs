using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordPlatformBehavior : MonoBehaviour
{
    [SerializeField] private GameObject swordProjectile;
   

    
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
