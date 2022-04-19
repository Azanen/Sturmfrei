using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindSturmfrei : MonoBehaviour
{
    public Animator sturmfreiAnim;
    public GameObject sturmfrei;
    public GameObject pono;
    public Transform poncho1;
    public Transform poncho2;
    private bool collecte = false;
    public Event_Custom customEvent;

    private void Awake()
    {
        sturmfrei = this.gameObject.transform.GetChild(0).GetChild(0).gameObject;
        sturmfreiAnim = sturmfrei.GetComponent<Animator>();
        pono = GameObject.Find("PonoPrefab#03");
        //poncho1 = gameObject.transform.Find("ponchoMain");
        //poncho2 = gameObject.transform.Find("poncho2");
        customEvent = this.GetComponent<Event_Custom>();

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" && collecte == false)
        {

            StartCoroutine(Collecte(other));
            other.GetComponent<PlayerCollisionSphere>().PlayerMov.StartFadeWhite();
        }
    }

    IEnumerator Collecte(Collider other)
    {
        this.gameObject.GetComponent<AudioSource>().Play();
        StopMovingPono(other.GetComponent<PlayerCollisionSphere>().PlayerMov);
        collecte = true;
        yield return new WaitForSeconds(3.5f);
        sturmfreiAnim.SetBool("Collecte", true);
        yield return new WaitForSeconds(1.9f);
        sturmfrei.SetActive(false);

        // Around here for the cinematic I guess 


        customEvent.RaiseIsland.Invoke();
        customEvent.ChangeIsland.Invoke();
        yield return new WaitForSeconds(1.2f);
        poncho1.GetComponent<SkinnedMeshRenderer>().enabled = true;
        poncho2.GetComponent<SkinnedMeshRenderer>().enabled = true;
        other.GetComponent<PlayerCollisionSphere>().PlayerMov.RemoveFadeWhite();
        yield return new WaitForSeconds(1.0f);

        customEvent.LookAtFlyingControl.Invoke();

    }

    public void StopMovingPono(PlayerMovement player)
    {
        player.ActSpeed = 0;
        player.Anim.SetFloat("Moving", 0);
        player.Rigid.transform.position = player.transform.position;
        player.Rigid.velocity = Vector3.zero;
        player.enabled = false;
    }
}
