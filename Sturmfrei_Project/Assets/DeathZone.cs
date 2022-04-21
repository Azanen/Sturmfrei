using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathZone : MonoBehaviour
{
    public Transform respawnPoint;
    public Collider collider_Player;
    public CameraFollow camFol;
    //public Transform Death_Pos;
    void OnTriggerEnter(Collider other)
    {
        if (other.transform.gameObject.tag == "Player")
        {
            collider_Player = other;
            collider_Player.GetComponent<PlayerCollisionSphere>().PlayerMov.amDead = true;
            camFol = collider_Player.GetComponent<PlayerCollisionSphere>().PlayerMov.CamFol;
            camFol.isDead = collider_Player.GetComponent<PlayerCollisionSphere>().PlayerMov.amDead;
            //GameObject.Find("Pono#05").gameObject.GetComponent<EventsPonoAnim>().Fall_Respawn();
            collider_Player.GetComponent<PlayerCollisionSphere>().PlayerMov.gameObject.GetComponentInChildren<EventsPonoAnim>().Fall_Respawn();
            StartCoroutine(Dead_Respawn());
        }
    }

    private IEnumerator Dead_Respawn()
    {
        //Fade to black
        PlayerMovement player = collider_Player.GetComponent<PlayerCollisionSphere>().PlayerMov;
        player.StartFade();
        yield return new WaitForSeconds(2);
        respawnPoint = player.gameObject.GetComponent<PlayerRespawn>().respawnPoint;

        float yRotation = respawnPoint.rotation.y;
        Debug.Log("tryingSomething");
        player.gameObject.transform.rotation = Quaternion.Euler(gameObject.transform.rotation.x, yRotation+90, gameObject.transform.rotation.z);

        player.amDead = false;
        camFol.isDead = player.amDead;
        //Fade in instant
        collider_Player.transform.position = respawnPoint.position;
        yield return new WaitForSeconds(1f);
        player.RemoveFade();
    }
}

