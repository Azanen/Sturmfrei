using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main_Menu : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.None; //fait reapparaitre le curseur

    }

public void RemoveCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;

    }
}
