using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Return_to_Hub : MonoBehaviour
{
    private  PlayerCollisionSphere colliderPlayer;
    public CameraFollowTarget camFolTar;

    private void Start()
    {
        camFolTar = GameObject.Find("CameraFollow").GetComponent<CameraFollowTarget>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            this.gameObject.GetComponent<ObjectSoundTrigger>().PlaySound();
            colliderPlayer = other.GetComponent<PlayerCollisionSphere>();
            colliderPlayer.PlayerMov.StartFadeWhite();
            //stop the cam moving
            camFolTar.ReturnHub = true;
        }
    }
}
