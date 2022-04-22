using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectCollision : MonoBehaviour
{
    public float bottomOffset;
    public float frontOffset;
    public float auDessusOffset;
    public float collisionRadiusFloor;
    public float collisionRadiusWall;
    public float collisionRadiusDessus;
    public LayerMask GroundLayer;
    public LayerMask WallLayer;
    public float WallDistance;
    private PlayerMovement playMove;
    public bool checkGround2;

    public void Start()
    {
        playMove = GetComponent<PlayerMovement>();
    }

    public bool CheckGround()
    {
        Vector3 Pos = transform.position + (-transform.up * bottomOffset);
        Collider[] hitColliders = Physics.OverlapSphere(Pos, collisionRadiusFloor, GroundLayer);
        if (hitColliders.Length > 0)
        {
            return true;
        }
        return false;
    }

    public bool CheckWall()
    {
        Vector3 Pos2 = transform.position + (transform.forward * frontOffset);
        Collider[] hitColliders = Physics.OverlapSphere(Pos2, collisionRadiusWall, WallLayer);

        if (hitColliders.Length > 0 && !playMove.isDashing)
        {
            return true;
        }
        return false;
    }
    public bool CheckGround2()
    {
        Vector3 Pos3 = transform.position + (transform.forward * auDessusOffset);
        Collider[] hitColliders = Physics.OverlapSphere(Pos3, collisionRadiusFloor, GroundLayer);
        if (hitColliders.Length > 0)
        {
            checkGround2 = true;
            return true;
        }
        checkGround2 = false;
        return false;
    }
    void OnDrawGizmosSelected()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.yellow;
        Vector3 Pos = transform.position + (-transform.up * bottomOffset);
        Gizmos.DrawSphere(Pos, collisionRadiusFloor);
        Gizmos.color = Color.red;
        Vector3 Pos2 = transform.position + (transform.forward * frontOffset);
        Gizmos.DrawSphere(Pos2, collisionRadiusWall);
        Gizmos.color = Color.blue;
        Vector3 Pos3 = transform.position + (transform.forward * auDessusOffset);
        Gizmos.DrawSphere(Pos3, collisionRadiusDessus);
    }
}
