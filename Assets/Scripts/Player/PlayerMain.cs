using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMain : MonoBehaviour
{
    [SerializeField] PlayerMovement playerMovement;
    [SerializeField] PlayerActions playerActions;

    // Update is called once per frame
    void Update()
    {
        StateControl();
    }

    void StateControl()
    {
        if (playerActions.attacking)
        {
            playerMovement.enabled = false;
        }
        else
        {
            playerMovement.enabled = true;
        }
    }
}
