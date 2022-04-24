using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;


public class Main_Menu : MonoBehaviour
{
    public UnityEvent eventMain;
    public GameObject PremierBoutonMenu;
    public GameObject PremierBoutonOption;

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

    public void SelectMainMenu()
    {
        EventSystem.current.SetSelectedGameObject(null);    //clear les selection
        EventSystem.current.SetSelectedGameObject(PremierBoutonMenu);
    }

    public void SelectOptionMenu()
    {
        EventSystem.current.SetSelectedGameObject(null);    //clear les selection
        EventSystem.current.SetSelectedGameObject(PremierBoutonOption);
    }
}
