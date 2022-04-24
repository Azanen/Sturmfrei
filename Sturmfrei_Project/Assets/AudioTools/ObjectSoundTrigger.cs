using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSoundTrigger : MonoBehaviour
{
    public string eventName = "default";

    void Awake()
    {
        AkSoundEngine.RegisterGameObj(gameObject);
    }

    public void PlaySound()
    {
        Debug.Log("SFX");
        AkSoundEngine.PostEvent(eventName, gameObject);

    }
}
