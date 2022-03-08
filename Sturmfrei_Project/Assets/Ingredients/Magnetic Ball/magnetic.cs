using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class magnetic : MonoBehaviour
{
    private PlayerCollisionSphere playerRigid;
    private PlayerMovement playa;
    private grapple_gun grappleScript;
    public PonchoColourChange sturm;
    public bool isInsideMe = false;

    
    private void OnTriggerEnter(Collider other)
    {
        playerRigid = other.GetComponent<PlayerCollisionSphere>();
        playa = playerRigid.PlayerMov;
        grappleScript = playa.gameObject.GetComponent<grapple_gun>();
        grappleScript.magneticBall = this.gameObject.transform.parent.gameObject;
        //sturm = GameObject.FindGameObjectWithTag("Player").gameObject.GetComponent<PonchoColourChange>();
        sturm.GoldPoncho();
        isInsideMe = true;
    }
    private void OnTriggerExit(Collider other)
    {
        sturm.NoMoreGold();
        isInsideMe = false;
        grappleScript.StopGrapple(); // devrait �tre impossible
    }

    private void Update()
    {
        if (isInsideMe)
        {
            if (Input.GetMouseButtonDown(0))
            {
                grappleScript.StartGrapple();
            }
            else if (Input.GetMouseButtonUp(0))
            {
                grappleScript.StopGrapple();
            }
        }

    }
}
