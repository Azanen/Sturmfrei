using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class Load_Scene_Selector : MonoBehaviour
{
    public UnityEvent Test;
    public PlayerMovement player;
    private void Start()
    {
        player = GameObject.Find("PonoPrefab#03").GetComponent<PlayerMovement>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            StartCoroutine(LoadWithFadeToWhite());
            player.StartFadeWhite();
        }
    }

    IEnumerator LoadWithFadeToWhite()
    {
        yield return new WaitForSeconds(2);
        Test.Invoke();
    }
}
