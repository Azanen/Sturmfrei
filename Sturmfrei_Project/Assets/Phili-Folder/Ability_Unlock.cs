using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Ability_Unlock : MonoBehaviour
{
    //public UnityEvent Unlock_Time;
    public GameObject ObjetARegarder;
    public GameObject CameraPosition;
    public float tempsDeLookAt;

    private CameraFollowTarget camFolTar;
    private CameraFollow camFol;
    private PlayerMovement player;

    private void Start()
    {
        //tempsDeLookAt = 3;
        camFolTar = GameObject.Find("CameraFollow").GetComponent<CameraFollowTarget>();
        player = GameObject.Find("PonoPrefab#03").GetComponent<PlayerMovement>();
        camFol = camFolTar.gameObject.GetComponent<CameraFollow>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && ObjetARegarder != null)
        {
            camFol.wallObject = ObjetARegarder;
            StartCoroutine(RegardeObjet());
        }
    }

    public IEnumerator RegardeObjet()
    {
        
        camFolTar.lookingAtWall = true;
        camFolTar.Cam_Pos_Unlock = CameraPosition.transform.position;
        // Stop player control
        player.enabled = false ;
        //stop animation
        yield return new WaitForSeconds(tempsDeLookAt);
        // give back player control
        player.enabled = true;
        //Return cam
        camFolTar.lookingAtWall = false;
        Destroy(this.gameObject);
    }
}
