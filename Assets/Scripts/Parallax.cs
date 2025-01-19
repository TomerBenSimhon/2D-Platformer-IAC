using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Parallax : MonoBehaviour
{
    private float startPosX;
    private float startPosY;
    
    
    [SerializeField] GameObject cam;
    [SerializeField] float parallaxEffectX;
    [SerializeField] float parallaxEffectY;
    void Start()
    {
        startPosX = transform.position.x;
        startPosY = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        float distanceX = cam.transform.position.x * parallaxEffectX;
        float distanceY = cam.transform.position.y * parallaxEffectY;
        
        transform.position = new Vector3(startPosX + distanceX, startPosY + distanceY, transform.position.z);
    }
}
