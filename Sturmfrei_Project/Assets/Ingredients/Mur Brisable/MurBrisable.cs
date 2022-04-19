using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MurBrisable : MonoBehaviour
{
    public bool isDashing;
    public GameObject MurBrise;
    public GameObject MurSolide;
    //public AudioSource bedingbedang;
    public PlayerCollisionSphere playMov; 
    List<GameObject> childObjects = new List<GameObject>();


private void Start()
    {
        //bedingbedang = this.gameObject.GetComponent<AudioSource>();
        MurSolide = this.gameObject.transform.GetChild(0).gameObject;
        MurBrise = this.gameObject.transform.GetChild(1).gameObject;
        MurBrise.SetActive(false);
        foreach (Transform child in MurBrise.transform)
        {
            childObjects.Add(child.gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        playMov = other.gameObject.GetComponent<PlayerCollisionSphere>();
        if (other.gameObject.tag == "Player" && other != null)
        {
            isDashing = playMov.PlayerMov.isDashing;

            if (isDashing == true)
            {
                StartCoroutine(Brise());
            }
        }
    }

    //Not sure we need to put this in a enumerator, talk to Oli
    IEnumerator Brise()
    {
        this.gameObject.GetComponent<ObjectSoundTrigger>().PlaySound();
        MurSolide.SetActive(false);
        MurBrise.SetActive(true); 
        foreach(GameObject childObjects in childObjects)
        {
            int flyup = Random.Range(5, 15);
            childObjects.GetComponent<Rigidbody>().AddForce(new Vector3 (0,flyup,0), ForceMode.Impulse);
            Debug.Log(flyup.ToString());
           // childObjects.GetComponent<Material>().
        }
        //bedingbedang.Play();
        yield return new WaitForSeconds(1.3f);
        Destroy(MurBrise, 7f);
    }


}
