using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MusiqueTriggers : MonoBehaviour
{
    public UnityEvent Start_Event;
    public UnityEvent Musique_Event;

    private void Start()
    {
        Start_Event.Invoke();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "player")
        {
            Musique_Event.Invoke();
        }
    }
}

