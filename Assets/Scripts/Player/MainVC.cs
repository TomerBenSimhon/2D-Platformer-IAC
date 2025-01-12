using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class MainVC : MonoBehaviour
{
    [SerializeField] CinemachineVirtualCamera vcam;
    CinemachineFramingTransposer transposer;
    GameObject player;
    Rigidbody2D playerRB;
    
    [Header("xOffset")]
    [SerializeField] float xOffsetTarget;
    [SerializeField] private float lerpSmoothing;
    
    [Header("YDamping")]
    [SerializeField] float defaultYDamping;
    void Start()
    {
        player = FindObjectOfType<PlayerMain>().gameObject;
        playerRB = player.GetComponent<Rigidbody2D>();
        
        transposer = vcam.GetCinemachineComponent<CinemachineFramingTransposer>();
    }

    
    void Update()
    {
        OffsetLerp();
        YDamp();
    }

    private float xOffset;
    private float xOffsetVelocity;
    void OffsetLerp()
    {
        xOffset = Mathf.SmoothDamp(
            xOffset, 
            player.transform.localScale.x * xOffsetTarget, 
            ref xOffsetVelocity, // SmoothDamp tracks velocity internally
            lerpSmoothing * Time.deltaTime
        );        
        
        
        transposer.m_TrackedObjectOffset = new Vector3(xOffset, 0, 0);
    }

    
    private float yDampVelocity;
    void YDamp()
    {
        if (playerRB.velocity.y < -1)
        {
            transposer.m_YDamping = Mathf.SmoothDamp(transposer.m_YDamping, 0.2f, ref yDampVelocity, 225f * Time.deltaTime);
        }
        else
        {
            transposer.m_YDamping = defaultYDamping;
        }
    }
}
