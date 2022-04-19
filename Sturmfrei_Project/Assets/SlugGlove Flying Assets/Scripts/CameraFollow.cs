using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    //public bool FollowTargetRotation;
    [Header("FollowSpeed")]
    public float FollowRotSpeed = 0.5f;
    public float FollowRotSpeedFlying = 10f;
    public float GravityFollowSpeed = 0.1f;
    private Vector3 LookDirection;

   
    public enum WorldState
    {
        Grounded, //on ground
        Flying, //trying to fly
        Dead,
    }

    private WorldState States;
    public Transform target;
    public Transform YPivot;

    private Transform pivot;
    private Transform FollowRotationPivot;
    public Transform camTransform;
    private Camera CamUnit;

    private Vector3 LookAtPos;
    [Header("Mouse Speeds")]
    public float MouseSpeed = 3; //how quickly the camera rotates 
    public float turnSmoothing = 0.1f; //smoothing applied to this
    public float minAngle = -35; //the min amount the camera can tilt downwards
    public float maxAngle = 35; //the max amount the camera can tilt upwards
    public float LookDirectionSpeed = 2f; //how fast the camera corrects to the players upwards direction (for when flying upside down

    [Header("Auto Inputs")]
    public float SecondsBeforeCameraReplaceBehindPlayer; //how long before the camera will correct to be behind the player
    private float AutoXInput;
    public float AutoInputCorrectionSpeed = 2f; //how fast the camera auto corrects on the ground
    public float AutoInputCorrectionSpeedFlying = 2f; //how fast the camera auto corrects when flying

    public float DistanceFromPlayer; //how close the camera is to the player 
    private float curZ;

    float smoothX;
    float smoothXvelocity;
    float smoothY;
    float smoothYvelocity;
    private float tiltAngle;

    float delta;
    public static CameraFollow singleton;

    [Header("Fov")]
    public float NormalFov; //the normal fov
    public float MaxFov; //the fov when the player is moving faster

    public float MinVelocity;
    public float MaxVelocity;
    private float FovLerp;

    public bool isGrappled;
    public bool isDead;
    public bool lookWall = false;
    public GameObject wallObject;
    private CameraFollowTarget camFolTar;

    public PlayerMovement player;

    //setup objects
    void Awake()
    {
        transform.parent = null;

        pivot = camTransform.parent;
        LookAtPos = target.position;
        curZ = -DistanceFromPlayer;

        tiltAngle = 10f;
        AutoXInput = 0f;

        LookDirection = transform.forward;

        CamUnit = GetComponentInChildren<Camera>();

        //Ajouter par Philippe
        Cursor.lockState = CursorLockMode.Locked;

        camFolTar = this.gameObject.GetComponent<CameraFollowTarget>();
    }

    private void FixedUpdate()
    {
        delta = Time.deltaTime;

        if (!target)
        {
            return;
        }

        if (!isGrappled && !isDead && !lookWall)
        {
            Tick(delta);
        }
        else if (!lookWall)
        {
            //LookAtPos = target.position;
            this.transform.LookAt(target.GetComponentInParent<PlayerMovement>().transform.position);
                
        }
        else if (lookWall)
        {
            Vector3 direction = wallObject.transform.position - transform.position;
            Quaternion toRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, 1 * Time.deltaTime);
            //this.transform.LookAt(wallObject.transform.position);
        }

    }

    /*private void Update()
    {
        if (isGrappled)
        {
            GrappleCam();
        }
    }*/

    public void Tick(float d)
    {
        float h = Input.GetAxis("Mouse X");
        //float v = Input.GetAxis("Mouse Y");
        float v = Input.GetAxis("Mouse Y") * player.manager.ReturnCamInverted();

        //reset auto input timer
        if (h != 0 || v != 0) 
        {

            if(AutoXInput < SecondsBeforeCameraReplaceBehindPlayer)
                AutoXInput = SecondsBeforeCameraReplaceBehindPlayer;
        }

        HandleRotation(d, v, h, MouseSpeed);
        handlePivotPosition();

        //look at player
        
        
        LookAtPos = target.position;

        //Vector3 WorldPos = transform.up;

        //get speed based on if we are flying
        float FolSpd = FollowRotSpeed;
        if (States == WorldState.Flying)
            FolSpd = FollowRotSpeedFlying;
        else if (States == WorldState.Dead)
            FolSpd = FollowRotSpeed * 0.2f;

        Vector3 LerpDir = Vector3.Lerp(transform.up, target.up, d * FolSpd);
        transform.rotation = Quaternion.FromToRotation(transform.up, LerpDir) * transform.rotation;

        if (AutoXInput < 0 && States != WorldState.Grounded)//ajout du check of flight
        {
            float CorSpd = AutoInputCorrectionSpeed;

            if (States != WorldState.Grounded)
                CorSpd = AutoInputCorrectionSpeedFlying;
            else if (States == WorldState.Dead)
                CorSpd = FollowRotSpeed * 0.2f;

            tiltAngle = Mathf.Lerp(tiltAngle, 10, CorSpd * d);
            transform.rotation = Quaternion.Slerp(transform.rotation, target.rotation, CorSpd * d);
        }
        else
        {
            AutoXInput -= d;
            //Stabilise camera 
            //transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 0);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 0), Time.deltaTime * 1.5f);
        }
    }

    public void HandleFov(float d, float Velocity)
    {
        float FAmt = NormalFov;
        //lerp to the velocity
        float LerpAmt = Velocity / MaxVelocity;
        FAmt = Mathf.Lerp(NormalFov, MaxFov, LerpAmt);

        FovLerp = Mathf.Lerp(FovLerp, FAmt, d * 8f);

        CamUnit.fieldOfView = FovLerp;
    }

    void handlePivotPosition()
    {
        float targetZ = -DistanceFromPlayer;

        curZ = Mathf.Lerp(curZ, targetZ, delta * 10f);

        Vector3 tp = Vector3.zero;
        tp.z = curZ;
        camTransform.localPosition = tp;
    }

    void HandleRotation(float d, float v, float h, float speed)
    {
        if (turnSmoothing > 0)
        {
            smoothY = Mathf.SmoothDamp(smoothY, v, ref smoothYvelocity, turnSmoothing);
            smoothX = Mathf.SmoothDamp(smoothX, h, ref smoothXvelocity, turnSmoothing);
        }
        else
        {
            smoothX = h;
            smoothY = v;
        }

        tiltAngle -= smoothY * speed;
        tiltAngle = Mathf.Clamp(tiltAngle, minAngle, maxAngle);
        pivot.localRotation = Quaternion.Euler(tiltAngle, 0, 0);

        if (smoothX != 0)
        {
            transform.RotateAround(transform.position, transform.up, ((smoothX * speed) * 30f) * d);
        }
    }

    public void SetFlyingState(int Dex)
    {
        if (Dex == 1)
        {
            States = WorldState.Flying;
        }
        else if (Dex == 0)
        {
            States = WorldState.Grounded;
        }
        else
        {
            States = WorldState.Dead;
        }
    }
    


}