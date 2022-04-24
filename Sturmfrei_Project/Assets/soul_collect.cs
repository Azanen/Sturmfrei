using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class soul_collect : MonoBehaviour
{
    //public AudioSource collectsound;
    private PlayerCollisionSphere playCol;
    private PlayerMovement playerMov;

    public GameObject fullSoul;
    public GameObject soulTransparent;
    //private PonchoColourChange ponchoColor;

   private void OnTriggerEnter(Collider other)
    {
        playCol = other.GetComponent<PlayerCollisionSphere>();
        playerMov = playCol.PlayerMov;
        //ponchoColor = playerMov.gameObject.GetComponent<PonchoColourChange>();
        //ponchoColor.GoldPoncho();
        //collectsound.Play();
        if (fullSoul != null)
        {
            StartCoroutine(detroy());
        }
    }

    private IEnumerator detroy()
    {
            this.gameObject.GetComponent<ObjectSoundTrigger>().PlaySound();
            yield return new WaitForSeconds(0.1f);
        //Destroy(fullSoul);
        fullSoul.SetActive(false);
            yield return new WaitForSeconds(5f);
            soulTransparent.SetActive(true);
    }
}


