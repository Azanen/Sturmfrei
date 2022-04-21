using System.Collections;
using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    private PlayerMovement playMove;

    public Transform respawnPoint;
    private GameObject deathZone;

    //Start is called before the first frame update
    void Start()
    {
        //deathZone = GameObject.Find("/DeathZone");
        playMove = GetComponent<PlayerMovement>();
        respawnPoint = transform;

    }

    //Update is called once per frame
    void Update()
    {
        //deathZone.transform.position = new Vector3(transform.position.x, deathZone.transform.position.y, transform.position.z);
    }

    private void OnTriggerEnter(Collider fallingCollision)
    {
        if (fallingCollision.transform.gameObject.tag == "DeathZone")
        {
            fallingCollision.transform.position = respawnPoint.position;
            playMove.amDead = false;
        }
    }

}









