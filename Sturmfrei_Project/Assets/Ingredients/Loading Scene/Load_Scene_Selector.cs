using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class Load_Scene_Selector : MonoBehaviour
{
    // list des scene en string
    public UnityEvent Test;
    public PlayerMovement player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            player.StartFadeWhite();
            StartCoroutine(LoadWithFadeToWhite());
        }
    }

    IEnumerator LoadWithFadeToWhite()
    {
        yield return new WaitForSeconds(2);
        Test.Invoke();
    }
}
