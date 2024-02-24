using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public PlayerCam playerCam;

    [Header("Movement")]
    private float moveSpeed;
    public float walkSpeed;
    public float sprintSpeed;

    public float groundDrag;

    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump;
    public float wallrunSpeed;

    [Header("Crouching")]
    public float crouchSpeed;
    public float crouchYScale;
    public bool isCrouching;
    private float startYScale;
    public float maxSlideSpeed;
    public float slideSpeed;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode crouchkey = KeyCode.LeftControl;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    public bool grounded;

    [Header("Slope Handling")]
    public float maxSlopeAngle;
    private RaycastHit slopeHit;
    private bool exitingSlope;

    public Transform orientation;

    public float horizontalInput;
    public float verticalInput;
    Vector3 moveDirection;
    public Rigidbody rb;

    public Transform playerVisual;

    public MovementState state;

    public Player player;
    public bool isSticking;
    public bool canStick;

    public AudioSource footstepFX;
    public AudioClip[] footsteps;
    private float lastFootstepTime;

    public bool wallrunning;

    public enum MovementState
    {
        walking, sprinting, wallrunning, crouching, air
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        readyToJump = true;

        startYScale = transform.localScale.y;
    }

    // Update is called once per frame
    void Update()
    {

        if (((state == MovementState.walking || state == MovementState.sprinting) && rb.velocity.magnitude > 2) || state == MovementState.crouching && slideSpeed < 0.4f && rb.velocity.magnitude > 1 || state == MovementState.wallrunning && rb.velocity.magnitude > 2)
        {
            float footstepInterval = 3f / rb.velocity.magnitude;  // Inversely proportional interval
            float timeSinceLastFootstep = Time.time - lastFootstepTime;

            if (timeSinceLastFootstep >= footstepInterval)
            {
                AudioClip footstepSound = footsteps[Random.Range(0, footsteps.Length)];
                footstepFX.PlayOneShot(footstepSound);

                lastFootstepTime = Time.time;  // Update the last footstep time
            }
        }

        transform.rotation = Quaternion.Euler(0f, playerCam.transform.rotation.eulerAngles.y, 0f);

        if (state != MovementState.crouching)
        {
            grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);
        }
        else
        {
            grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f, whatIsGround);
        }

        myInput();
        speedControl();
        stateHandler();

        if (grounded)
        {
            rb.drag = groundDrag;
        }
        else
        {
            rb.drag = 0;
        }

        if (!isCrouching )
        {
            slideSpeed = rb.velocity.magnitude * 60;
        }
        if(slideSpeed > maxSlideSpeed)
        {
            slideSpeed = maxSlideSpeed;
        }

        if (isCrouching)
        {
            if (grounded)
            {
                if (slideSpeed > 0)
                {
                    player.playSlideSound();
                }
                else
                {
                    player.muteSlideSound();
                }
            }
        }
    }

    private void FixedUpdate()
    {
        movePlayer();
        if (wallrunning && moveSpeed > 0.1f)
        {
            moveSpeed -= Time.deltaTime * 400;
        }

        if (rb.velocity.magnitude > 0.5f)
        {
            if (isCrouching)
            {
                if (OnSlope() && GetSlopeMoveDirection().y == 0 || OnSlope() && GetSlopeMoveDirection().y < 0) // Check if the player is moving downward
                {
                    slideSpeed += (Time.deltaTime * 120) * 2; // Increase slideSpeed
                }
                Vector3 slideDirection = rb.velocity.normalized; // Slide in the direction of movement
                if (OnSlope())
                {
                    slideDirection.y = -1;
                }
                else
                {
                    slideDirection.y = 0;
                }
                rb.AddForce(slideDirection * slideSpeed);
                if (slideSpeed > 0)
                {
                    slideSpeed -= Time.deltaTime * 120;
                }
            }
        }
    }

    private void myInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;

            jump();

            Invoke(nameof(resetJump), jumpCooldown);
        }

        if (Input.GetKeyDown(crouchkey) && !wallrunning)
        {
            // slideSpeed = maxSlideSpeed;
            if (rb.velocity.magnitude > 2)
            {
                player.playStartSlideSound();
            }
            isCrouching = true;
            transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
            rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        }

        if (Input.GetKeyUp(crouchkey))
        {
            player.muteSlideSound();
            isCrouching = false;
            transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
        }

        if (player.icePickEquipped)
        {
            if (Input.GetMouseButtonDown(0) && canStick)
            {
                StickToWall();
            }

            if (Input.GetMouseButtonDown(1) && isSticking)
            {
                UnstickFromWall();
            }
        }
        if (Input.GetKeyDown(jumpKey) && isSticking)
        {
            
                UnstickFromWall();
                if (!grounded)
                {
                    jump();
                }
        }
    }

    private void stateHandler()
    {
        

        // Mode - Sprinting
        //if (grounded && Input.GetKey(sprintKey) && !isCrouching)
        //{
        //    state = MovementState.sprinting;
        //    moveSpeed = sprintSpeed;
        //}

        // Mode - Walking
        if (grounded && !isCrouching)
        {
            state = MovementState.walking;
            if (moveSpeed < 6.9f)
            {
                moveSpeed = walkSpeed;
            }
        }

        // Mode - Air
        else if (!isCrouching)
        {
            state = MovementState.air;
        }
        else if (Input.GetKey(crouchkey) && grounded)
        {
            state = MovementState.crouching;
            if (moveSpeed > 3.5f)
            {
                moveSpeed = (crouchSpeed * slideSpeed) / 240;
            }
            else
            {
                moveSpeed = 3.5f;
            }
        }

        // Mode - Wallrunning
        if (wallrunning)
        {
            state = MovementState.wallrunning;
            
                moveSpeed = wallrunSpeed;
            
        }
    }

    private void movePlayer()
    {
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        if (OnSlope())
        {
            rb.AddForce(GetSlopeMoveDirection() * moveSpeed * 20f, ForceMode.Force);
            if (rb.velocity.y > 0)
            {
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);
            }
            if (rb.velocity.y < 0f && (horizontalInput > 0 || verticalInput > 0))
            {
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);
            }

        }

        if (grounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        }

        else if (!grounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
        }

        if(!wallrunning) rb.useGravity = !OnSlope();
    }

   private void speedControl()
{

    float currentSpeed = rb.velocity.magnitude;

    if (OnSlope() && !exitingSlope)
    {
        if (currentSpeed > moveSpeed)
        {
            rb.velocity = rb.velocity.normalized * moveSpeed;
        }
    }
    else
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        if ((flatVel.magnitude > moveSpeed) && (state == MovementState.air || state == MovementState.crouching))
        {
            Vector3 limitedVel = Vector3.Lerp(flatVel.normalized * moveSpeed, flatVel, Time.deltaTime * 2.5f);
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

    if (!isCrouching && state != MovementState.sprinting)
    {
        moveSpeed = Mathf.Lerp(moveSpeed, walkSpeed, Time.deltaTime * 2.5f);
    }

    if (rb.velocity.magnitude > 8 && wallrunning)
    {
        rb.velocity = Vector3.Lerp(rb.velocity.normalized * 8, rb.velocity, Time.deltaTime * 2.5f);
    }
}

    private void jump()
    {
        //slideSpeed = slideSpeed / 2;
        exitingSlope = true;
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        player.painAudio.PlayOneShot(player.jumpSound);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        player.spawnJumpParticles();
    }
    public void playJumpSound()
    {
        player.painAudio.PlayOneShot(player.jumpSound);
    }
    public void playWallRunStartSound()
    {
        player.painAudio.PlayOneShot(player.wallRunStartSound);
    }
    private void resetJump()
    {
        readyToJump = true;
        exitingSlope = false;
        //slideSpeed = maxSlideSpeed;
    }

    private bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }

        return false;
    }

    private Vector3 GetSlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized;
    }

    private void StickToWall()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, playerCam.transform.forward, out hit, 0.95f))
        {
            player.playIcePickStickSound();
            isSticking = true;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            canStick = false;
            readyToJump = true;
            
            FixedJoint joint = gameObject.AddComponent<FixedJoint>();
            joint.connectedBody = hit.rigidbody;

            joint.anchor = transform.InverseTransformPoint(hit.point);
            joint.connectedAnchor = hit.transform.InverseTransformPoint(hit.point);

            GameObject parts = Instantiate(player.icePickParticles, hit.point, Quaternion.identity);
            Destroy(parts, 3f);
        }
    }

    public void UnstickFromWall()
    {
        float pitchRange = 0.1f;
        player.icePickSound.pitch = Random.Range(1f - pitchRange, 1f + pitchRange);
        player.icePickSound.PlayOneShot(player.icePickUnStick);
        isSticking = false;
        canStick = true;
        Destroy(GetComponent<FixedJoint>());
    }
}