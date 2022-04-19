using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InteractionZone2 : MonoBehaviour
{
    public UnityEvent Event_Modular;
    public LayerMask checkPointZone;
    private Transform CPDetectionPoint; // CP = checkpoint
    private GameObject CPDetectionPointObject; // CP = checkpoint
    public Transform spawnPoint;
    public GameObject playerObject;
    public bool pressingLore;
    public GameObject shrine;
    public GameObject[] shrineRocks;
    public GameObject crystalToSpawn;
    public ParticleSystem aetherFX;
    public Color ShrineLight;
    public Color coul1;
    public Color coul2;
    public float intensity;
    private float t;

    public GameObject lightbeam;

    public bool shrineActive = false;

    //private GameObject controllerY;
    //private GameObject keyboardF;

    private void Start()
    {
        playerObject = GameObject.FindGameObjectWithTag("Player");
        CPDetectionPointObject = GameObject.Find("/"+ playerObject.name.ToString() + "/Bird_Centre/FloorCheck");
        CPDetectionPoint = CPDetectionPointObject.transform;
        //controllerY = this.transform.parent.transform.Find("YController").gameObject;
        //keyboardF = this.transform.parent.transform.Find("FKeyboard").gameObject;
        //lightbeam = this.transform.parent.transform.Find("LightBeam").gameObject;
       // controllerY.SetActive(false);
       // keyboardF.SetActive(false);
    }

    public void Update()
    {
        pressingLore = Input.GetButton("Save");

        if (shrineActive == true)
        {
            LightUp();
            intensity = (Mathf.Lerp(0, 5, t));
            t += 0.8f * Time.deltaTime;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            shrine.GetComponent<Animator>().SetBool("PonoProche", true);

            //PlayerMovement player = playerObject.GetComponent<PlayerMovement>();
            //ControleDeCheckPoint(player);
            /*
            if (pressingLore)
            {
                shrine.GetComponent<Animator>().SetTrigger("SetCheckpoint");
                Instantiate(crystalToSpawn, spawnPoint.position, spawnPoint.rotation);
                StartCoroutine(Aether());
                shrineActive = true;
            }*/
            if (!shrineActive)
            {
                shrine.GetComponent<Animator>().SetTrigger("SetCheckpoint");
                Instantiate(crystalToSpawn, spawnPoint.position, spawnPoint.rotation);
                StartCoroutine(Aether());
                StartCoroutine(RemoveLightBeam(lightbeam));
                if (!shrineActive) StartCoroutine(ActivateLook());
                shrineActive = true;
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            shrine.GetComponent<Animator>().SetBool("PonoProche", false);
             //RemoveControleDeCheckPoint();
        }
    }

    IEnumerator Aether()
    {
        this.gameObject.GetComponent<ObjectSoundTrigger>().PlaySound();
        aetherFX.Play();
        yield return new WaitForSeconds(2f);
        //aetherFX.Stop();
    }
    public void LightUp()
    {
        for (int i = 0; i < shrineRocks.Length; i++)
        {
            shrineRocks[i].GetComponent<Renderer>().material.EnableKeyword("_EMISSION");
            shrineRocks[i].GetComponent<Renderer>().material.SetColor("_EmissionColor", ShrineLight * intensity);
        }
        //shrineRocks.GetComponent<Renderer>().material.EnableKeyword("_EMISSION");
        //shrineRocks.GetComponent<Renderer>().material.SetColor("_EmissionColor", ShrineLight * (Mathf.Lerp(0, 1, 1f)));

        ShrineLight = Color.Lerp(coul1, coul2, Mathf.PingPong(Time.time, 3));
        
    }

    IEnumerator ActivateLook()
    {
        yield return new WaitForSeconds(3);
        Event_Modular.Invoke();
    }
    /*
    private void ControleDeCheckPoint(PlayerMovement player)
    {
        if (controllerY != null && keyboardF != null)
        {
            controllerY.SetActive(true);
            keyboardF.SetActive(true);

            controllerY.transform.LookAt(player.Cam);
            keyboardF.transform.LookAt(player.Cam);
        }
    }

    private void RemoveControleDeCheckPoint()
    {
        if (controllerY != null && keyboardF != null)
        {
            controllerY.SetActive(false);
            keyboardF.SetActive(false);
        }
    }
    */


    /*    private float progressRemoveFadeWhite = 1;
public IEnumerator RemoveFadeToWhite()
    {
        while (progressRemoveFadeWhite > 0)
        {
            elapsedTimeRemoveFadeWhite -= Time.unscaledDeltaTime;
            progressRemoveFadeWhite = elapsedTimeRemoveFadeWhite * 2;

            Color c = FadeWhite.color;
            c.a = progressRemoveFadeWhite;
            FadeWhite.color = c;
            yield return null;
        }
    }
    }*/
    public void ShrinkBeam(GameObject beam)
    {
        StartCoroutine(RemoveLightBeam(beam));
    }

    private float progressScale = 1;
    private float elapsedTimeScale = 1;
    IEnumerator RemoveLightBeam(GameObject beam1)
    {
        while (progressScale > 0)
        {
            elapsedTimeScale -= Time.unscaledDeltaTime/2;
            progressScale = elapsedTimeScale;

            Vector3 tempoScale = new Vector3(1, progressScale, 1);
            beam1.transform.localScale = tempoScale;
            yield return null;
        }

    }
}
