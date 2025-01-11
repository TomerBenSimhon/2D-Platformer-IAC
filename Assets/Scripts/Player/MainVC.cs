using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class MainVC : MonoBehaviour
{
    [SerializeField] CinemachineVirtualCamera vcam;
    CinemachineFramingTransposer transposer;
    GameObject player;
    
    [Header("xOffset")]
    [SerializeField] float xOffset;

    [SerializeField] private float lerpSpeed;
    void Start()
    {
        player = FindObjectOfType<PlayerMain>().gameObject;
        transposer = vcam.GetCinemachineComponent<CinemachineFramingTransposer>();
    }

    
    void Update()
    {
        OffsetLerp();
    }

    
    void OffsetLerp()
    {
        xOffset = Mathf.MoveTowards(xOffset, player.transform.localScale.x * 2f, Time.deltaTime * 10f);
        
        transposer.m_TrackedObjectOffset = new Vector3(xOffset, 0, 0);
    }
}
