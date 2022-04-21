using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    DeathZone deathZone;
    public PlayerRespawn player;
    private void Start()
    {
        //deathZone = GameObject.Find ("DeathZone").GetComponent<DeathZone>();
    }
    void OnTriggerEnter (Collider other)
    {
        player = other.GetComponent<PlayerCollisionSphere>().PlayerMov.gameObject.GetComponent<PlayerRespawn>(); ;
        if (other.transform.gameObject.tag == "Player")
        {
            //deathZone.respawnPoint = other.GetComponent<PlayerRespawn>().respawnPoint;
            player.respawnPoint = this.transform;
        }
    }
}
