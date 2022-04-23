using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RetourHub : MonoBehaviour
{
    public bool isDashing;
    public PlayerCollisionSphere playMov;
    public GameObject RetourDustDevil;
    public GameObject RetourCheck;

    private void Start()
    {
        RetourCheck.SetActive(false);
        RetourDustDevil.SetActive(false);
    }
    private void OnTriggerEnter(Collider other)
    {
        playMov = other.gameObject.GetComponent<PlayerCollisionSphere>();
        if (other.gameObject.tag == "Player" && other != null)
        {
            isDashing = playMov.PlayerMov.isDashing;

            if (isDashing == true)
            {
                StartCoroutine(TelepHub());
            }
        }
    }

    IEnumerator TelepHub()
    {
        yield return new WaitForSeconds(3f);
        RetourDustDevil.SetActive(true);
        yield return new WaitForSeconds(1f);
        RetourCheck.SetActive(true);
    }
}
