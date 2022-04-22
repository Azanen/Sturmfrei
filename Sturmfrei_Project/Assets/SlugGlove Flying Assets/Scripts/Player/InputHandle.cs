using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandle : MonoBehaviour
{
    private PlayerMovement Player;

    public float Horizontal;
    public float Vertical;
    public bool Jump;
    public bool JumpHold;
    public bool Accelerate;
    public bool Dashing;
    public bool Fly;

    private void Start()
    {
        Player = GetComponent<PlayerMovement>();
    }

    void Update()
    {
        Horizontal = Input.GetAxis("Horizontal");
        Vertical = Input.GetAxis("Vertical");

        Jump = Input.GetButtonDown("Jump");
        Dashing = Input.GetButtonDown("Dashing");
    }
}
