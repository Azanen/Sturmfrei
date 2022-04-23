using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DustDevilAscenseur : MonoBehaviour
{
    private PlayerCollisionSphere quiMonte;
    private Rigidbody quiMonteRigid;
    private GameObject colliderPlayer;
    public PlayerMovement player;
    public Animator anim;
    public GameObject pono;

    private void Start()
    {
        pono = GameObject.Find("PonoPrefab#03");
        if (pono!=null)
        {
            anim = pono.GetComponentInChildren<Animator>();
        }
        player = pono.GetComponent<PlayerMovement>();
    }

    //donne une velocite verticale
    private void OnTriggerStay(Collider other)
    {
        colliderPlayer = other.gameObject;
        if (colliderPlayer.tag == "Player")
        {
            quiMonte = colliderPlayer.GetComponent<PlayerCollisionSphere>();
            quiMonteRigid = quiMonte.GetComponent<Rigidbody>();
            player = quiMonte.PlayerMov;

            if (player.States == PlayerMovement.WorldState.InAir || player.States == PlayerMovement.WorldState.Grounded)
            {
                quiMonteRigid.velocity = Vector3.up * 8f;
                player.inDustDevil = true;
                anim.SetBool("InDustDevil", true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            quiMonteRigid.useGravity = true;
            player.inDustDevil = false;
            anim.SetBool("InDustDevil", false);
        }
    }
}
