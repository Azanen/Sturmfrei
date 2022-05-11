using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PonchoEmissionChange : MonoBehaviour
{
    public Texture EmissionZero;
    public Texture EmissionOne;
    public Texture EmissionTwo;
    public Texture EmissionThree;
    public GameObject poncho;
    public Material ponchoMat;

    public UnityEvent ponchoSwitchStart;
    public UnityEvent ponchoSwitch;

    private void Awake()
    {
        //ponchoMat = poncho.GetComponent<Renderer>().material;
        ponchoSwitchStart.Invoke();
    }

    public void PonchoSwitchTriggger()
    {
        ponchoSwitch.Invoke();
    }

    public void PonchoZero()
    {
        ponchoMat.SetTexture("_EmissionMap", EmissionZero);
    }
    public void PonchoOne()
    {
        ponchoMat.SetTexture("_EmissionMap", EmissionOne);
    }
    public void PonchoTwo()
    {
        ponchoMat.SetTexture("_EmissionMap", EmissionTwo);
    }
    public void PonchoThree()
    {
        ponchoMat.SetTexture("_EmissionMap", EmissionThree);
    }

}
