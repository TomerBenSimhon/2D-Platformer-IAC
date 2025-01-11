using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class MainVC : MonoBehaviour
{
    [SerializeField] CinemachineVirtualCamera vcam;
    GameObject player;
    void Start()
    {
        player = FindObjectOfType<PlayerMain>().gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
