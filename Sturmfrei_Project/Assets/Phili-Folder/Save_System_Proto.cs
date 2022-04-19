using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Save_System_Proto : MonoBehaviour
{
    //public Text Text_Centre;
    public static int number = 0;
    public static bool InvertedCam;
    public static bool InvertedFlying;


    private void Start()
    {
        //Text_Centre.text = "0";
        Load();

    }
    public void IncreaseNumber()
    {
       // number += 1;
        //Text_Centre.text = number.ToString();
    }
    public void ChangeInvertedCam()
    {
        InvertedCam = !InvertedCam;
        Save();
    }
    public void ChangeInvertedFlying()
    {
        InvertedFlying = !InvertedFlying;
        Save();
    }
    public void Save()
    {
        Save_System.Save(number, InvertedCam, InvertedFlying);
    }

    public void Load()
    {
        Save_Data_Proto data = Save_System.LoadData();
        number = data.number;
        InvertedCam = data.InvertedCam;
        InvertedFlying = data.InvertedFlying;
        //Text_Centre.text = number.ToString();
    }

    public int ReturnCamInverted()
    {
        int num = InvertedCam ? 1 : -1;
        Save();
        return num;
    }
    public int ReturnFlyingInverted()
    {
        int num = InvertedFlying ? 1 : -1;
        Save();
        return num;
    }
}
