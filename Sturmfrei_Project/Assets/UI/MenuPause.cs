using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class MenuPause : MonoBehaviour
{
    public static bool isPaused = false;
    public GameObject menuPauseUI;
    public GameObject menuOptionsUI;
    public GameObject menuControlsUI;
    public GameObject player;
    public string menuPrincipal = "MenuPrincipal";

    public GameObject pauseFirstButton;
    public GameObject optionsFirstButton;
    public GameObject controlsFirstButton;

    /*public GameObject contourInvertFly;
    private bool toggleBoutFly;
    public GameObject contourInvertCam;
    private bool toggleBoutCam;
    public GameObject SaveSys;
    */

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
       // ToggleCam();
       // ToggleFly();
    }
    void Update()
    {
        if (Input.GetButtonDown("Pause"))
        {
            if (isPaused == true)
            {
                Reprendre();
            }
            else
            {
                SurPause();
            }
        }
    }

    public void Reprendre() //fonction pour le bouton "reprendre" aussi
    {
        menuPauseUI.SetActive(false);
        menuControlsUI.SetActive(false);
        menuOptionsUI.SetActive(false);

        Time.timeScale = 1f;
        isPaused = false;
        Cursor.lockState = CursorLockMode.Locked;
        player.GetComponent<PlayerMovement>().enabled = true;
    }

    void SurPause()
    {
        menuPauseUI.SetActive(true);
        Time.timeScale = 0f; //met le jeu sur pause
        isPaused = true;
        EventSystem.current.SetSelectedGameObject(null);    //clear les selection
        EventSystem.current.SetSelectedGameObject(pauseFirstButton); //cree nouvelle selection (pour manette)
        Cursor.lockState = CursorLockMode.None; //fait reapparaitre le curseur
        player.GetComponent<PlayerMovement>().enabled = false; //fait qu'on ne peut pas "dash" durant le menu
    }

    public void LoadMenu()
    {
        //Debug.Log("loadingMenuPrincipal");
        SceneManager.LoadScene(menuPrincipal);
        Time.timeScale = 1f;
    }

    public void Quitter()
    {
        //Debug.Log("Quitte le jeu");
        Application.Quit();
    }

    public void OpenOption()
    {
        menuOptionsUI.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null);    //clear les selection
        EventSystem.current.SetSelectedGameObject(optionsFirstButton); //cree nouvelle selection (pour manette)
    }

    public void CloseOption()
    {
        menuOptionsUI.SetActive(false);
        EventSystem.current.SetSelectedGameObject(null);    //clear les selection
        EventSystem.current.SetSelectedGameObject(pauseFirstButton); //cree nouvelle selection (pour manette)
    }

    public void OpenControls()
    {
        menuControlsUI.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null);    //clear les selection
        EventSystem.current.SetSelectedGameObject(controlsFirstButton); //cree nouvelle selection (pour manette)
    }

    public void CloseControls()
    {
        menuControlsUI.SetActive(false);
        EventSystem.current.SetSelectedGameObject(null);    //clear les selection
        EventSystem.current.SetSelectedGameObject(pauseFirstButton); //cree nouvelle selection (pour manette)
    }
    /*
    public void ToggleCam()
    {
        if (SaveSys.GetComponent<Save_System_Proto>().ReturnCamInverted() > 0) 
        {
            toggleBoutCam = true;
            contourInvertCam.SetActive(toggleBoutCam);
        }
        else { toggleBoutCam = false; contourInvertCam.SetActive(toggleBoutCam);}
    }

    public void ToggleFly()
    {
        if (SaveSys.GetComponent<Save_System_Proto>().ReturnFlyingInverted() > 0) 
        { 
            toggleBoutFly = true;
            contourInvertCam.SetActive(toggleBoutFly);
        }
        else { toggleBoutFly = false; contourInvertFly.SetActive(toggleBoutFly);}
    }*/
}
