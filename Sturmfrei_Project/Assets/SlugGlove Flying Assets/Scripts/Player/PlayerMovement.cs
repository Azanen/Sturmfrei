using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class PlayerMovement : MonoBehaviour
{
    public enum WorldState
    {
        Grounded, //on ground
        InAir, //in the air 
        Flying, //trying to fly
        Stunned, //on a wall
        Static,
        Grappling, 
    }

    [Header("Ability Unlock")]
    public bool upwardDashAbility = false;
    public bool frontDashAbility = false;
    public bool purificationAbility = false;
    public bool ponchoToFly = false;


    
    public WorldState States;
    [HideInInspector]
    public WorldState PreviousState;
    [HideInInspector]
    public Transform Cam; //reference to our camera
    private Transform CamY; //reference to our camera axis
    [HideInInspector]
    public CameraFollow CamFol; //reference to our camera script
    private PlayerVisuals Visuals; //script for handling visual effects
    private Vector3 CheckPointPos; //where we respawn

    private DetectCollision Colli; //collision detection
    [HideInInspector]
    public Rigidbody Rigid; //rigidbody 
    [HideInInspector]
    public Animator Anim; //animator
    private InputHandle InputHand; //script for handling our inputs
    float delta;

    [Header("Physics")]
    public float HandleReturnSpeed; //how quickly our handle on our character is returned to normal after a force is added (such as jumping
    private float ActGravAmt; //the actual gravity that is applied to our character
    public LayerMask GroundLayers; //what layers the ground can be
    private float FloorTimer; //how long we are on the floor
    private bool OnGround;  //the bool for if we are on ground (this is used for the animator
    private float ActionAirTimer; //the air timer counting our current actions performed in air

    [Header("Stats")]
    public float MaxWalkSpeed; //max speed for basic movement
    public float MaxFlyingAndWalkingSpeed; //max possible speed
    private float ActAccel; //our actual acceleration
    public float WalkAcceleration; //how quickly we build speed
    public float MovementAcceleration;    //how quickly we adjust to new speeds
    public float WalkSlowDownAcceleration; //how quickly we slow down
    public float WalkTurnSpeed; //how quickly we turn on the ground
    private float FlownAdjustmentLerp; //if we have flown this will be reset at 0, and effect turn speed on the ground
    
    //[HideInInspector]
    [Header("SPEED")]
    public float ActSpeed; //our actual speed
    private Vector3 movepos, targetDir, DownwardDirection; //where to move to

    [Header("Falling")]
    public float AirAcceleration;  //how quickly we adjust to new speeds when in air
    public float turnSpeedInAir;
    public float FallingDirectionSpeed; //how quickly we will return to a normal direction

   [Header("Flying")]
    public float FlyingDirectionSpeed; //how much influence our direction relative to the camera will influence our flying
    public float FlyingRotationSpeed; //how fast we turn in air overall
    public float FlyingUpDownSpeed; //how fast we rotate up and down
    public float FlyingLeftRightSpeed;  //how fast we rotate left and right
    public float FlyingAcceleration; //how much we accelerate to max speed
    public float FlyingDecelleration; //how quickly we slow down when flying
    public float MaxFlyingSpeed; //our max flying speed
    public float FlyingMinSpeed; //our flying slow down speed
    public float FlyingAdjustmentSpeed; //how quickly our velocity adjusts to the flying speed
    private float FlyingAdjustmentLerp; //the lerp for our adjustment amount

    [Header("Flying Physics")]
    public float FlyingGravityAmt; //how much gravity will pull us down when flying
    public float GlideGravityAmt; //how much gravity affects us when just gliding
    public float FlyingGravBuildSpeed; //how much our gravity is lerped when stopping flying
    public float FlyingAccelerationDownward; //how much velocity we gain for flying downwards
    public float FlyingDeccelerationUpward; //how much velocity we lose for flying upwards
    public float FlyingDownAngleBeforeAcceleration; //how much we fly down before a boost
    public float FlyingUpBeforeSlowDown; //how much we fly up before a boost;
    public float GlideTime; //how long we glide for when not flying before we start to fall (forward !?)

    [Header("Jumps")]
    public float JumpAmt; //how much we jump upwards 
    private bool HasJumped; //if we have pressed jump
    public float GroundedTimerBeforeJump; //how long we have to be on the floor before an action can be made
                                          //public float JumpForwardAmount; //how much our regular jumps move us forward

    [Header("Wall Impact")]
    public float SpeedLimitBeforeCrash; //how fast we have to be going to crash
    public float StunPushBack;  //how much we are pushed back
    public float StunnedTime; //how long we are stunned for
    private float StunTimer; //the in use stun timer

    [Header("Visuals")]
    private Transform HipsPos; //where our characters hips are
    private bool MirrorAnim; //if anims should be mirroed
    private float XAnimFloat; //the float for our wing turning
    private float RunTimer; //animation ctrl for running
    private float FlyingTimer; //the time before the animation stops flying

    //Variables de philippe
    private bool wingON;
    private bool flyTest;
    private bool wingSwitchCooldown;

    [Header("Dash Metriques")]
    public int frontDashSpeed = 50;
    public int upwardDashSpeed = 50;
    public int secondStaminaCooldown = 5;
    public bool canDashFront=true;
    public bool canDashUp=true;
    private float elapsedTime = 0;
    public float progress = 0;
    public float dashTime = 0.5f;
    private float tempoSpeed;
    private float tempoFixedSpeed;
    public bool isDashing = false;
    public float veloY;
    public bool isFalling;
    private bool coroutRunning;


    [Header("Custom")]
    public ParticleSystem aether;
    private PlayerRespawn playerResp;
    private grapple_gun grappleGunz;
    public bool isTainted;
    public float TaintedTimer;
    private float timeElapsedDashZoom;
    private float lerpDuration;
    public bool inDustDevil;
    public bool dashLock = false;
    public float downTime, upTime, pressTime = 0;
    public float countDown = 2.0f;
    public bool ready = false;
    public bool amDead = false;
    private Image Fade;
    private Image FadeWhite;
    public bool usingTriggerAxis = false;


    // Start is called before the first frame update
    void Awake()
    {
        States = WorldState.Static;

        InputHand = GetComponent<InputHandle>();
        Colli = GetComponent<DetectCollision>();
        Visuals = GetComponent<PlayerVisuals>();
        HipsPos = Visuals.HipsPos;

        Cam = GameObject.FindGameObjectWithTag("MainCamera").transform;
        CamY = Cam.transform.parent.parent.transform;
        CamFol = Cam.GetComponentInParent<CameraFollow>();
        playerResp = this.GetComponent<PlayerRespawn>();
        grappleGunz = this.GetComponent<grapple_gun>();
        aether = this.transform.Find("Aether").gameObject.GetComponent<ParticleSystem>();
        CheckPointPos = transform.position;

        //setup this characters stats
        SetupCharacter();
        SetupValue();

        wingON = false;
        wingSwitchCooldown = true;
        timeElapsedDashZoom = 0;
        lerpDuration = 10;

        progress = 1;

        GameObject canvas = GameObject.Find("InGameUI").GetComponentInChildren<Canvas>().gameObject;
        if (canvas != null)
        {
            Fade = canvas.transform.Find("Fade").GetComponent<Image>();
            FadeWhite = canvas.transform.Find("FadeWhite").GetComponent<Image>();
        }
    }

    private void Update()   
    {
        veloY = Rigid.velocity.y;
        //cannot function when dead
        if (States == WorldState.Static)
            return;

        AnimCtrl();

        transform.position = Rigid.position;

        if (States == WorldState.Grounded)
        {
            if (FloorTimer > 0)
                return;

            //check for ground
            bool Ground = Colli.CheckGround();

            if (!Ground)
            {
                SetInAir();
                return;
            }

            if (InputHand.Jump)
            {
                SetInAir();
                if (!HasJumped)
                {                   
                    if(Anim)
                    {
                        MirrorAnim = !MirrorAnim;
                        Anim.SetBool("Mirror", MirrorAnim);
                        Anim.SetTrigger("Jump");
                    }

                    Visuals.Jump();

                    float AddAmt = Mathf.Clamp((ActSpeed * 0.5f), -10, 16);

                    StopCoroutine(JumpUp(JumpAmt + AddAmt));
                    StartCoroutine(JumpUp(JumpAmt + AddAmt));
                    return;
                }
            }
            isFalling = false;
        }
        else if (States == WorldState.InAir)
        {
            if (InputHand.Dashing)
            {
                UpwardDash();
            }


            //check for ground version Phil
            bool Ground = false;
            if (Colli.CheckGround() /*|| Colli.CheckGround2()*/) // was removed because of wall slide
            {
                Ground = true;
            }
            if (Ground)
            {
                Anim.SetBool("isPlonging", false);
                SetGrounded();
                return;
            }

        }
        else if(States == WorldState.Flying)
        {
            MinimalChangeSpeed();

            if (ActionAirTimer > 0)
                return;

            //check wall collision for a crash, if this unit can crash
            bool WallHit = Colli.CheckWall();

            //if we have hit a wall
            if (WallHit)
            {
                if(ActSpeed < SpeedLimitBeforeCrash)
                {
                    SetInAir();
                }
                if(ActSpeed > SpeedLimitBeforeCrash)
                {
                    Stunned(-transform.forward);
                    return;
                }
  
            }

            //check for ground if we are not holding the flying button
            if (!InputHand.Fly)
            {
                bool Ground = Colli.CheckGround();

                if (Ground && !isDashing)
                {
                    SetGrounded();
                    return;
                }
            }

            if (InputHand.Dashing)
            {
                FrontalDash();
            }
            //Fixed speed while in 1 sec dash
            isFalling = false;
        }
        else if(States == WorldState.Grappling)
        {
            if (grappleGunz.magnetic_script.horizontalGrapple)
            {
                this.transform.position = new Vector3(this.transform.position.x, grappleGunz.magneticBall.transform.position.y, this.transform.position.z);
            }
            else if (grappleGunz.magnetic_script.verticalGrappleZ)
            {
                this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, grappleGunz.magneticBall.transform.position.z);
            }
            else if (grappleGunz.magnetic_script.verticalGrappleX)
            {
                this.transform.position = new Vector3(grappleGunz.magneticBall.transform.position.x, this.transform.position.y, this.transform.position.z);
            }
            Rigid.transform.position = this.transform.position;
            Vector3 testingForward = (this.transform.position - grappleGunz.magneticBall.transform.position);

            //Forward = opposé de magnetic
            this.transform.forward = new Vector3 (testingForward.x, testingForward.y, testingForward.z);

        }

        WingSwitch();

        // Reset mecanic
        if (Input.GetButtonDown("Save") && ready == false)
        {
            downTime = Time.time;
            pressTime = downTime + countDown;
            ready = true;
        }
        if (Input.GetButtonUp("Save"))
        {
            ready = false;
        }
        if (Time.time >= pressTime && ready == true)
        {
            ready = false;
            ResetPlayer();
        }
    }

    // Update is called once per frame
    void FixedUpdate()  //world movement
    {
        //tick deltatime
        delta = Time.deltaTime;

        //get velocity to feed to the camera
        float CamVel = 0;
        if (Rigid != null)
            CamVel = Rigid.velocity.magnitude;
        //change the cameras fov based on speed
        CamFol.HandleFov(delta, CamVel);

        //cannot function when dead
        if (States == WorldState.Static)
        {
            //turn off air sound
            Visuals.WindAudioSetting(delta * 3f, 0f);

            return;
        }
        //tick any fixed camera controls
        FixedAnimCtrl(delta);

        //control our direction slightly when falling
        float _xMov = InputHand.Horizontal;
        float _zMov = InputHand.Vertical;

        //get our direction of input based on camera position
        Vector3 screenMovementForward = CamY.transform.forward;
        Vector3 screenMovementRight = CamY.transform.right;
        Vector3 screenMovementUp = CamY.transform.up;

        Vector3 h = screenMovementRight * _xMov;
        Vector3 v = screenMovementForward * _zMov;

        Vector3 moveDirection = (v + h).normalized;

        if (States == WorldState.Grounded)
        {
            //turn off wind audio
            if (Visuals.WindLerpAmt > 0)
                Visuals.WindAudioSetting(delta * 3f, 0f);

            float LSpeed = MaxWalkSpeed;
            float Accel = WalkAcceleration;
            float MoveAccel = MovementAcceleration;

            //reduce floor timer
            if (FloorTimer > 0)
                FloorTimer -= delta;

           if (InputHand.Horizontal == 0 && InputHand.Vertical == 0)
            {
                //we are not moving, lerp to a walk speed
                LSpeed = 0f;
                Accel = WalkSlowDownAcceleration;
            }
           //lerp our current speed
            if (ActSpeed > LSpeed - 0.5f || ActSpeed < LSpeed + 0.5f)
                LerpSpeed(delta, LSpeed, Accel);
            //move our character
            MoveSelf(delta, ActSpeed, MoveAccel, moveDirection);
        }
        else if (States == WorldState.InAir)
        {
            Anim.SetBool("Flying", false);

            //reduce air timer 
            if (ActionAirTimer > 0)
                ActionAirTimer -= delta;

            //falling effect
            Visuals.FallEffectCheck(delta);

            //falling audio
            Visuals.WindAudioSetting(delta, Rigid.velocity.magnitude);

            //slow our flying control if we were not (PB not sure what is this)
            if (FlyingAdjustmentLerp > -.1)
                FlyingAdjustmentLerp -= delta * (FlyingAdjustmentSpeed * 0.5f);

            float speedValueTempo;
            if(moveDirection == Vector3.zero)
            {
                speedValueTempo = (PreviousState== WorldState.Flying)?5f:0;
            }
            else
            {
                speedValueTempo= 5f;
            }

            if(PreviousState == WorldState.Flying && !inDustDevil)
            {
                   Anim.SetBool("isPlonging", true);
                   FallingCtrl_Plonge(delta, speedValueTempo, AirAcceleration, moveDirection);
            }
            else
            {
                FallingCtrl(delta, speedValueTempo, AirAcceleration, moveDirection);
                if (inDustDevil)
                { 
                    PreviousState = WorldState.InAir;
                    Anim.SetBool("Flying", false);
                }
            }
        }
        else if (States == WorldState.Flying)
        {
            Anim.SetBool("isPlonging", false);

            //falling effect
            Visuals.FallEffectCheck(delta);

            //falling audio
            Visuals.WindAudioSetting(delta, Rigid.velocity.magnitude);

            //Philippe was here and it took me 2h -_-
            if (InputHand.Horizontal == 0 && transform.rotation.z !=0 )
            {
                Vector3 eulerRotation = transform.rotation.eulerAngles;
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(eulerRotation.x, eulerRotation.y, 0), Time.deltaTime * 1.5f);
            }


            //lerp controls
            if (FlyingAdjustmentLerp < 1.1)
                FlyingAdjustmentLerp += delta * FlyingAdjustmentSpeed;

            //lerp speed
            float YAmt = Rigid.velocity.y;
            float FlyAccel = FlyingAcceleration * FlyingAdjustmentLerp;
            float Spd = MaxFlyingSpeed;
            if (!InputHand.Fly)  //we are not holding fly, slow down
            {
                Spd = FlyingMinSpeed; 
                if(ActSpeed > FlyingMinSpeed)
                    FlyAccel = FlyingDecelleration * FlyingAdjustmentLerp;
            }


            HandleVelocity(delta, Spd, FlyAccel, YAmt);

            //flying controls
            FlyingCtrl(delta, ActSpeed, _xMov, _zMov);

        }
        else if (States == WorldState.Stunned)
        {
            //reduce stun timer
            if (StunTimer > 0)
            {
                StunTimer -= delta;

                if (StunTimer > StunnedTime * 0.5f)
                    return;
            }


            //switch to air
            bool Ground = Colli.CheckGround();

            //reduce ground timer
            if (Ground)
            {
                if (Anim)
                    Anim.SetBool("Stunned", false);

                //get up from ground
                if (StunTimer <= 0)
                {
                    SetGrounded();
                    return;
                }
            }
            
            // Pour ne pas rester coincer dans un dust devil
            if(inDustDevil)
            {
                if (Anim)
                    Anim.SetBool("Stunned", false);
                SetInAir();
            }
            
            //lerp mesh slower when not on ground
            RotateSelf(DownwardDirection, delta, 8f);
            RotateMesh(delta, transform.forward, WalkTurnSpeed);

            //push backwards while we fall
            Vector3 FallDir = -transform.forward * 4f;
            FallDir.y = Rigid.velocity.y;
            Rigid.velocity = Vector3.Lerp(Rigid.velocity, FallDir, delta * 2f);

            //falling audio
            Visuals.WindAudioSetting(delta, Rigid.velocity.magnitude);
        }

    }
    //for when we return to the ground
    public void SetGrounded()
    {
        
        CamFol.MouseSpeed = 7;
        CamFol.minAngle = -10;
        CamFol.maxAngle = 75;
        Visuals.Landing();

        //reset wind animation
        Visuals.SetFallingEffects(1.6f);

        //reset flying animation 
        FlyingTimer = 0;
        //reset flying adjustment
        FlyingAdjustmentLerp = 0;

        //reset physics and jumps
        DownwardDirection = Vector3.up;
        PreviousState = States;
        States = WorldState.Grounded; //on ground
        OnGround = true;

        //camera reset flying state
        CamFol.SetFlyingState(0);

        //turn on gravity
        Rigid.useGravity = true;
    }
    //for when we are set in the air (for falling
    public void SetInAir()
    {
        CamFol.MouseSpeed = 3;
        CamFol.minAngle =-25;
        CamFol.maxAngle =45;

        OnGround = false;
        FloorTimer = GroundedTimerBeforeJump;
        //ActionAirTimer = 0.2f;
        PreviousState = States;
        States = WorldState.InAir;

        //camera reset flying state
        CamFol.SetFlyingState(0);

        //turn off gravity
        Rigid.useGravity = true;

    }
    //for when we start to fly
    public void SetFlying()
    {
        CamFol.MouseSpeed = 4;
        CamFol.minAngle = -25;
        CamFol.maxAngle = 45;
        //PB added
        //OnGround = false;
        PreviousState = States;
        States = WorldState.Flying;

        //set animation 
        FlyingTimer = GlideTime;

        ActGravAmt = 0f; //our gravity is returned to the flying amount

        FlownAdjustmentLerp = -1;

        //camera set flying state
        CamFol.SetFlyingState(1);

        //turn on gravity
        Rigid.useGravity = false; // PB
    }
    //stun our character
    public void Stunned(Vector3 PushDirection)
    {
        if (Anim)
            Anim.SetBool("Stunned", true);

        StunTimer = StunnedTime;
        Anim.SetBool("Flying", false);

        //set physics
        ActSpeed = 0f;
        Rigid.velocity = Vector3.zero;
        Rigid.AddForce(PushDirection * StunPushBack, ForceMode.Impulse);
        DownwardDirection = Vector3.up;
        PreviousState = States;
        States = WorldState.Stunned;

        //turn on gravity
        Rigid.useGravity = true;
    }
    public void SetGrappling()
    {
        PreviousState = States;
        States = WorldState.Grappling;
        //turn on gravity
        Rigid.useGravity = false;
    }

    #region Fonctions de controles
    void AnimCtrl()
    {
        //setup the location of any velocity based animations from our hip position 
        Transform RelPos = this.transform;

        //find animations based on our hip position (for flying velocity animations
        if (HipsPos)
            RelPos = HipsPos;
        //get movement amounts in each direction
        Vector3 RelVel = RelPos.transform.InverseTransformDirection(Rigid.velocity);
        Anim.SetFloat("forwardMove", RelVel.z);
        Anim.SetFloat("sideMove", RelVel.x);
        Anim.SetFloat("upwardsMove", RelVel.y);
        //our rigidbody y amount (for upwards or downwards velocity animations
        Anim.SetFloat("YVel", Rigid.velocity.y);

        //set movement animator
        RunTimer = new Vector3(Rigid.velocity.x, 0, Rigid.velocity.z).magnitude;
        Anim.SetFloat("Moving", RunTimer);

        //set our grounded and flying animations
        Anim.SetBool("OnGround", OnGround);

    }

    void FixedAnimCtrl(float D) //animations involving a timer
    {
        //setup the xinput animation for tilting our wings left and right
        float LAMT = InputHand.Horizontal;
        XAnimFloat = Mathf.Lerp(XAnimFloat, LAMT, D * 4f);
        Anim.SetFloat("XInput", XAnimFloat);
    } 

    IEnumerator JumpUp(float UpwardsAmt)
    {
        
        HasJumped = true;
        //kill velocity
        Rigid.velocity = Vector3.zero;
        //set to in air as we will be 
        SetInAir();
        //add force upwards
        if (UpwardsAmt != 0)
            Rigid.AddForce((Vector3.up * UpwardsAmt), ForceMode.Impulse);

        //remove any built up acceleration
        ActAccel = 0;
        //stop jump state
        yield return new WaitForSecondsRealtime(0.3f);
        HasJumped = false;
    }
    //lerp our speed over time
    void LerpSpeed(float d, float TargetSpeed, float Accel)
    {
        //if our speed is larger than our max speed, reduce it slowly 
        if (ActSpeed > MaxWalkSpeed)
        {
            ActSpeed = Mathf.Lerp(ActSpeed, TargetSpeed, d * Accel * 0.5f);
        }
        else
        {
            if (TargetSpeed > 0.5)
            {
                //influence by x and y input 
                float Degree = Vector3.Magnitude(new Vector3(InputHand.Horizontal, InputHand.Vertical, 0).normalized);
                ActSpeed = Mathf.Lerp(ActSpeed, TargetSpeed, (d * Accel) * Degree);
            }
            else
                ActSpeed = Mathf.Lerp(ActSpeed, TargetSpeed, d * Accel);


        }
        //clamp our speed
        ActSpeed = Mathf.Clamp(ActSpeed, 0, MaxFlyingAndWalkingSpeed);
    }

    //handle how our speed is increased or decreased when flying
    void HandleVelocity(float d, float TargetSpeed, float Accel, float YAmt)
    {
        if (ActSpeed > MaxFlyingSpeed) //we are over out max speed, slow down slower
            Accel = Accel * 0.8f;

        if (YAmt < FlyingDownAngleBeforeAcceleration) //we are flying down! boost speed
        {
            TargetSpeed = TargetSpeed + (FlyingAccelerationDownward * (YAmt * -0.5f));
        }
        else if (YAmt > FlyingUpBeforeSlowDown) //we are flying up! reduce speed
        {
            TargetSpeed = TargetSpeed - (FlyingDeccelerationUpward * YAmt);
            ActSpeed -= (FlyingDeccelerationUpward * YAmt) * d;
        }
        //clamp speed
        TargetSpeed = Mathf.Clamp(TargetSpeed, -MaxFlyingAndWalkingSpeed, MaxFlyingAndWalkingSpeed);
        //lerp speed
        ActSpeed = Mathf.Lerp(ActSpeed, TargetSpeed, Accel * d);
    }
    //boost our speed
    public void SpeedBoost(float Amt)
    {
        ActSpeed += Amt;
    }

    // On ground move
    void MoveSelf(float d, float Speed, float Accel, Vector3 moveDirection)
    {
        if (moveDirection == Vector3.zero)
        {
            targetDir = transform.forward;
        }
        else
        {
            targetDir = moveDirection;
        }

        if (targetDir == Vector3.zero)
        {
            targetDir = transform.forward;
        }
        //turn ctrl
        Quaternion lookDir = Quaternion.LookRotation(targetDir);
        //turn speed after flown is reduced
        if (FlownAdjustmentLerp < 1)
            FlownAdjustmentLerp += delta * 2f;
        //set our turn speed
        float TurnSpd = (WalkTurnSpeed + (ActSpeed * 0.1f)) * FlownAdjustmentLerp;
        TurnSpd = Mathf.Clamp(TurnSpd, 0, 6);

        //lerp mesh slower when not on ground
        RotateSelf(DownwardDirection, d, 8f);
        RotateMesh(d, targetDir, TurnSpd);

        //move character
        float Spd = Speed;
        Vector3 curVelocity = Rigid.velocity;
        Vector3 MovDirection = transform.forward;

        if (moveDirection == Vector3.zero) //if we are not pressing a move input we move towards velocity //or are crouching
        {
            Spd = Speed * 0.8f; //less speed is applied to our character
        }

        Vector3 targetVelocity = MovDirection * Spd;
        //accelerate our character
        ActAccel = Mathf.Lerp(ActAccel, Accel, HandleReturnSpeed * d);
        //lerp our movement direction
        Vector3 dir = Vector3.Lerp(curVelocity, targetVelocity, d * ActAccel);   
        dir.y = Rigid.velocity.y;
        //set our rigibody direction
        Rigid.velocity = dir;
    }

    // THis might not be usefull at all right now.
    void FallingCtrl(float d, float Speed, float Accel, Vector3 moveDirection)
    {
        if (moveDirection == Vector3.zero)
        {
            targetDir = transform.forward;
        }
        else
        {
            targetDir = moveDirection;
        }

        //turn ctrl
        Quaternion lookDir = Quaternion.LookRotation(targetDir);
        //turn speed after flown is reduced
        if (FlownAdjustmentLerp < 1)
            FlownAdjustmentLerp += delta * 2f;
        //set our turn speed
        float TurnSpd = (WalkTurnSpeed + (ActSpeed * 0.1f)) * FlownAdjustmentLerp;
        TurnSpd = Mathf.Clamp(TurnSpd, 0, 6);

        RotateMesh(d, targetDir, TurnSpd);


        //Best value of control for now
        Vector3 eulerRotation = transform.rotation.eulerAngles;
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0), Time.deltaTime * 4f);

        //move character
        float Spd = Speed;
        if (inDustDevil) // weird fix
        {
            if (moveDirection == Vector3.zero)
            {
                Spd = 0;
            }
            else
            {
                Spd = Speed;
            }
        }
        else
        {
            Spd = Speed;
        }
       Vector3 curVelocity = Rigid.velocity;
        //this part is weird
        Vector3 targetVelocity;
        targetVelocity = targetDir * Spd;

        //lerp our acceleration
        ActAccel = Mathf.Lerp(ActAccel, Accel, HandleReturnSpeed * d);
        //set rigid direction
        Vector3 dir = Vector3.Lerp(curVelocity, targetVelocity, d * ActAccel);
        dir.y = Rigid.velocity.y;
        Rigid.velocity = dir;
    }
    void FallingCtrl2(float d, float Speed, float Accel, Vector3 moveDirection)
    {

        //Best value of control for now
        Vector3 eulerRotation = transform.rotation.eulerAngles;
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0), Time.deltaTime * 2f);
        //move character
        float Spd = Speed;
         Vector3 curVelocity = Rigid.velocity;

         Vector3 targetVelocity = targetDir * Spd;
         //lerp our acceleration
         ActAccel = Mathf.Lerp(ActAccel, Accel, HandleReturnSpeed * d);
    }
    void FallingCtrl_Plonge(float d, float Speed, float Accel, Vector3 moveDirection)
    {
        if (moveDirection == Vector3.zero)
        {
            targetDir = transform.forward;
        }
        else
        {
            targetDir = moveDirection;
        }
        Quaternion lookDir = Quaternion.LookRotation(targetDir);
        //turn speed after flown is reduced
        if (FlownAdjustmentLerp < 1)
            FlownAdjustmentLerp += delta * 2f;
        //set our turn speed
        float TurnSpd = (WalkTurnSpeed + (ActSpeed * 0.1f)) * FlownAdjustmentLerp;
        TurnSpd = Mathf.Clamp(TurnSpd, 0, 6);

        float rotSpd = FlyingRotationSpeed;

        RotateToVelocity(d, rotSpd * 0.15f); // was 0.05

        //move character
        float Spd = Speed;
        if (inDustDevil) // weird fix
        {
            if (moveDirection == Vector3.zero)
            {
                Spd = 0;
            }
            else
            {
                Spd = Speed;
            }
        }
        else
        {
            Spd = Speed;
        }
        Vector3 curVelocity = Rigid.velocity;
        //this part is weird
        Vector3 targetVelocity;
        targetVelocity = targetDir * Spd;

        //lerp our acceleration
        ActAccel = Mathf.Lerp(ActAccel, Accel, HandleReturnSpeed * d);
        //set rigid direction
        Vector3 dir = Vector3.Lerp(curVelocity, targetVelocity, d * ActAccel);
        dir.y = Rigid.velocity.y;
        Rigid.velocity = dir;
    }
    void FlyingCtrl(float d, float Speed, float XMove, float ZMove)
    {
        //input direction 
        float InvertX = -1;
        float InvertY = -1;

        XMove = XMove * InvertX; //horizontal inputs
        ZMove = ZMove * InvertY; //vertical inputs

        //get direction to move character
        DownwardDirection = VehicleFlyingDownwardDirection(d, ZMove);
        Vector3 SideDir = VehicleFlyingSideDirection(d, XMove);
        //get our rotation and adjustment speeds
        float rotSpd = FlyingRotationSpeed;
        float FlyLerpSpd = FlyingAdjustmentSpeed * FlyingAdjustmentLerp;

        //lerp mesh slower when not on ground
        RotateSelf(DownwardDirection, d, rotSpd);
        RotateMesh(d, SideDir, rotSpd);

        if (FlyingTimer < GlideTime * 0.7f) //lerp to velocity if not flying
            RotateToVelocity(d, rotSpd * 0.05f);

        Vector3 targetVelocity = transform.forward * Speed;

            ActGravAmt = Mathf.Lerp(ActGravAmt, GlideGravityAmt, FlyingGravBuildSpeed * 0.5f * d);
 
        targetVelocity -= Vector3.up * ActGravAmt;
        //lerp velocity
        Vector3 dir = Vector3.Lerp(Rigid.velocity, targetVelocity, d * FlyLerpSpd);
        Rigid.velocity = dir;
    }
    
    Vector3 VehicleFlyingDownwardDirection(float d, float ZMove)
    {
        Vector3 VD = -transform.up;

        if (ZMove > 0.1) //upward tilt
        {
            VD = Vector3.Lerp(VD, -transform.forward, d * (FlyingUpDownSpeed * ZMove));
        }
        else if (ZMove < -.1) //downward tilt
        {
            VD = Vector3.Lerp(VD, transform.forward, d * (FlyingUpDownSpeed * (ZMove * -1)));
        }
        return VD;
    }

    Vector3 VehicleFlyingSideDirection(float d, float XMove)
    {
        Vector3 RollDir = transform.forward;

        //left right input
        if (XMove > 0.1)
        {
            RollDir = Vector3.Lerp(RollDir, -transform.right, d * (FlyingLeftRightSpeed * XMove));
        }
        else if (XMove < -.1)
        {
            RollDir = Vector3.Lerp(RollDir, transform.right, d * (FlyingLeftRightSpeed * (XMove * -1)));
        }
        return RollDir;
    }
    //rotate our upwards direction
    void RotateSelf(Vector3 Direction, float d, float GravitySpd)
    {
        Vector3 LerpDir = Vector3.Lerp(transform.up, Direction, d * GravitySpd);
        transform.rotation = Quaternion.FromToRotation(transform.up, LerpDir) * transform.rotation;
    }
    //rotate our left right direction
    void RotateMesh(float d, Vector3 LookDir, float spd)
    {
        Quaternion SlerpRot = Quaternion.LookRotation(LookDir, transform.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, SlerpRot, spd * d);
    }
    //rotate towards the velocity direction
    void RotateToVelocity(float d, float spd)
    {
        Vector3 TempoRotation = transform.rotation.eulerAngles;
        Quaternion SlerpRot = Quaternion.Euler(90, TempoRotation.y, TempoRotation.z);
        transform.rotation = Quaternion.Slerp(transform.rotation, SlerpRot, spd * d);
    }

    public void SetupCharacter()
    {
        //get components
        Anim = GetComponentInChildren<Animator>();
        Rigid = GetComponentInChildren<PlayerCollisionSphere>().GetComponentInChildren<Rigidbody>();
        //detatch rigidbody
        Rigid.transform.parent = null;
        //setup rigidbody link
        PlayerCollisionSphere LinkSet = Rigid.GetComponent<PlayerCollisionSphere>();
        LinkSet.Setup(this);
        SetInAir();
    }
    #endregion

    private IEnumerator Countdown() 
    {
        yield return new WaitForSeconds(0.1f);
        wingSwitchCooldown = true;
    }

    private void WingSwitch()
    {
        if (Input.GetButtonDown("SwitchWingOn") && wingSwitchCooldown && ponchoToFly)
        {
            if (States == WorldState.InAir)
            {
                wingSwitchCooldown = false;
                StartCoroutine(Countdown());
                SetFlying();
                Anim.SetBool("Flying", true);
                AnimCtrl();
                wingON = false;
                this.gameObject.GetComponent<WingSwitch>().Switch();
            }
        }
        if (Input.GetButtonDown("SwitchWingOn") && wingSwitchCooldown && ponchoToFly)
        {
            if (States == WorldState.Flying)
            {
                wingSwitchCooldown = false;
                StartCoroutine(Countdown());
                SetInAir();
                Anim.SetBool("Flying", false);
                AnimCtrl();
                wingON = true;
                this.gameObject.GetComponent<WingSwitch>().Switch();
            }
        }

        if (Input.GetAxis("TriggerSwitch") > 0 && wingSwitchCooldown)
        {
            if (States == WorldState.InAir)
            {
                usingTriggerAxis = true;
                wingSwitchCooldown = false;
                StartCoroutine(Countdown());
                SetFlying();
                Anim.SetBool("Flying", true);
                AnimCtrl();
                wingON = false;
                this.gameObject.GetComponent<WingSwitch>().Switch();
            }
        }
        if (Input.GetAxis("TriggerSwitch") <= 0 && wingSwitchCooldown && usingTriggerAxis)
        {
            if (States == WorldState.Flying)
            {
                usingTriggerAxis = false;
                wingSwitchCooldown = false;
                StartCoroutine(Countdown());
                SetInAir();
                Anim.SetBool("Flying", false);
                AnimCtrl();
                wingON = true;
                this.gameObject.GetComponent<WingSwitch>().Switch();

            }
        }
    }

    private void FrontalDash()
    {
        if (canDashUp && canDashFront && frontDashAbility && dashLock == false) 
        {
            Anim.SetTrigger("isDashing");
            tempoSpeed = ActSpeed;

            //Smaller collider 
            Rigid.GetComponent<SphereCollider>().radius = 0.3f;
            canDashFront = false;
            elapsedTime = 0;
            progress = 0;
            CreateAether();
            StartCoroutine(DashCountdown());
            StartCoroutine(DashResetSpeed());
        }
    }

    private void UpwardDash()
    {
        if (canDashUp && canDashFront && upwardDashAbility && dashLock == false)
        {
            SetInAir();
            Anim.SetTrigger("isDashing");
            transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
            Rigid.velocity = new Vector3(0, 0, 0);
            ActSpeed = 0;
            Rigid.AddForce((Vector3.up * upwardDashSpeed), ForceMode.Impulse);
            canDashUp = false;
            elapsedTime = 0;
            progress = 0;
            CreateAether();
            StartCoroutine(DashCountdown());
        }
    }
    public float progressTempo;
    private IEnumerator DashCountdown()
    {
        while(progress <= 1) 
        {
            elapsedTime += Time.unscaledDeltaTime;
            progress = elapsedTime / secondStaminaCooldown;
            float progressDash = progress * secondStaminaCooldown;
            progressTempo = progressDash;
            if (progressDash<dashTime)
            {
                if(!canDashUp)
                isDashing = true;
                if (!canDashFront)
                {
                    isDashing = true;
                    ActSpeed = frontDashSpeed;
                }
            }
            if (!canDashUp && progressDash > 1)// le 1 represente le temps que le joueur dash vers le haut, soit environ 1 sec
            {
                isDashing = false;
            }
            if (!canDashFront && progressDash > dashTime)
            {
                isDashing = false;
            }
            yield return null;
        }
        canDashFront = true;
        canDashUp = true;
        timeElapsedDashZoom = 0;
        isDashing = false;
    }
    private IEnumerator DashResetSpeed()
    {
        yield return new WaitForSeconds(dashTime);//0.5sec
        Rigid.GetComponent<SphereCollider>().radius = 0.6f;
        ActSpeed = tempoSpeed;
    }

    private void MinimalChangeSpeed()
    {
        FlyingMinSpeed = veloY > -0.9 ? 0 : 5;
    }

    private IEnumerator TetePremiere()
    {
        coroutRunning = true;
        yield return new WaitForSeconds(1.5f);
        isFalling = true;
        coroutRunning = false;
    }

    public void Tainted()
    {
        isTainted = true;
        dashLock = true;
        canDashFront = false;
        canDashUp = false;
    }

    //ajout Oli pour que player reste tainted tant que pono est dans toxic
    public void Detox()
    {
        StartCoroutine(NotTainted());
    }

    private IEnumerator NotTainted()
    {
        yield return new WaitForSeconds(TaintedTimer);
        isTainted = false;
        dashLock = false;
        canDashFront = true;
        canDashUp = true;
    }
    public void Purify()
    {
        canDashFront = true;
        canDashUp = true;
    }
    private void SetupValue()
    {
        MaxWalkSpeed = 7f;
        MaxFlyingAndWalkingSpeed = 25f;
        WalkAcceleration = 3f;
        MovementAcceleration = 20f;
        WalkSlowDownAcceleration = 4f;
        WalkTurnSpeed = 50f;
        FlownAdjustmentLerp = 1;
        FlyingDirectionSpeed = 2f; 
        FlyingRotationSpeed = 6f; 
        FlyingUpDownSpeed = 8f; 
        FlyingLeftRightSpeed = 8f;
        AirAcceleration = 10f; 
        turnSpeedInAir = 2f;
        FallingDirectionSpeed = 0.5f;
        FlyingAcceleration = 3f; 
        FlyingDecelleration = 0.3f; // was 1f, on the board also 1, than we tried 0.1f before 0.3f
        MaxFlyingSpeed = 25f; //was 30f, need to check if it's better together
        FlyingMinSpeed = 5; 
        FlyingAdjustmentSpeed = 75; // was 100
        FlyingAdjustmentLerp = 0;
        FlyingGravityAmt = 2f; 
        GlideGravityAmt = 3.5f; // was 4f before
        FlyingGravBuildSpeed = 3f; 
        FlyingAccelerationDownward = 10f; 
        FlyingDeccelerationUpward = 1f; 
        FlyingDownAngleBeforeAcceleration = 0f;  // was -2
        FlyingUpBeforeSlowDown = 1f; 
        GlideTime = 10f;
        JumpAmt = 30; 
        GroundedTimerBeforeJump = 0.2f;
        isFalling = false;
        coroutRunning = false;
        purificationAbility = false;
        upwardDashAbility = true;
        frontDashAbility = true;
        isTainted = false;
        TaintedTimer = 5;
        secondStaminaCooldown = 5;
        dashTime = 0.5f;
        inDustDevil = false;
        StunPushBack = 5;
}
    void CreateAether()
    {
        aether.Play();
    }

    public void ResetPlayer()
    {
        Rigid.transform.position = playerResp.respawnPoint;
        ActSpeed = 0;
        if (Anim)
            Anim.SetBool("Stunned", false);
        SetGrounded();
    }

    private float progressFadeBlack;
    private float elapsedTimeFadeBlack;
    public IEnumerator FadeToBlack()
    {
        while (progressFadeBlack < 1)
        {
            elapsedTimeFadeBlack += Time.unscaledDeltaTime;
            progressFadeBlack = elapsedTimeFadeBlack / 2;

            Color c = Fade.color;
            c.a = progressFadeBlack;
            Fade.color = c;
            yield return null;
        }
        ///OPTIONNEL
        Color finalBlack = Fade.color;
        finalBlack.a = 1;
        Fade.color = finalBlack;
    }

    public void StartFade()
    {
        if (Fade != null)
        {
            StartCoroutine(FadeToBlack());
        }
    }

    public void RemoveFade()
    {
        if (Fade != null)
        {
            Color blanc = Fade.color;
            blanc.a = 0;
            Fade.color = blanc;
            elapsedTimeFadeBlack = 0;
            progressFadeBlack = 0;
        }
    }

    private float progressFadeWhite;
    private float elapsedTimeFadeWhite;
    public IEnumerator FadeToWhite()
    {
        while (progressFadeWhite < 1)
        {
            elapsedTimeFadeWhite += Time.unscaledDeltaTime;
            progressFadeWhite = elapsedTimeFadeWhite / 2;

            Color c = FadeWhite.color;
            c.a = progressFadeWhite;
            FadeWhite.color = c;
            yield return null;
        }
        Color finalWhite = FadeWhite.color;
        finalWhite.a = 1;
        FadeWhite.color = finalWhite;
        yield return new WaitForSeconds(1);
        //SceneManager.LoadScene("Hub#02");
    }

    public void StartFadeWhite()
    {
        if (Fade != null)
        {
            StartCoroutine(FadeToWhite());
        }
    }
    public void RemoveFadeWhite()
    {
        if (Fade != null)
        {
            /*
            Color blanc = FadeWhite.color;
            blanc.a = 0;
            FadeWhite.color = blanc;*/
            elapsedTimeFadeWhite = 0;
            progressFadeWhite = 0;
           progressRemoveFadeWhite = 1;
             elapsedTimeRemoveFadeWhite = 1;
            StartCoroutine(RemoveFadeToWhite());
        }

        // try to remove with fadeout
    }

    private float progressRemoveFadeWhite = 1;
    private float elapsedTimeRemoveFadeWhite = 1;
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

    public void GotPonchoICanFlyNow()
    {
        ponchoToFly = true;
    }

    public void UnlockFrontalDash()
    {
    frontDashAbility = true;
    }
    public void UnlockUpwardDash()
    {
        upwardDashAbility = true;
    }

    public void UnlockPurification()
    {
        purificationAbility = true;
    }

}
