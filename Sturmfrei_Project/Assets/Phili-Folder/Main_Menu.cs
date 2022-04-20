using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class Main_Menu : MonoBehaviour
{
    public UnityEvent eventMain;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.None; //fait reapparaitre le curseur
        eventMain.Invoke();
    }

public void RemoveCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;

    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
