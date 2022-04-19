using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Save_Data_Proto 
{
    public int number;
    public bool InvertedCam;
    public bool InvertedFlying;

    public Save_Data_Proto(int data_proto, bool invertedCam, bool invertedFlying)
    {
        number = Save_System_Proto.number;
        InvertedCam = Save_System_Proto.InvertedCam;
        InvertedFlying = Save_System_Proto.InvertedFlying;
    }
}
