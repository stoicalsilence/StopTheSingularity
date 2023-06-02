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

    [Header("Crouching")]
    public float crouchSpeed;
    public float crouchYScale;
    private float startYScale;

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
    public enum MovementState
    {
        walking, sprinting, crouching, air
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
    }

    private void FixedUpdate()
    {
        movePlayer();
        
    }

    private void myInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;

            jump();

            Invoke(nameof(resetJump), jumpCooldown);
        }

        if (Input.GetKeyDown(crouchkey))
        {
            transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
            rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        }

        if (Input.GetKeyUp(crouchkey))
        {
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
        if (Input.GetKeyDown(jumpKey))
        {
            if (isSticking)
            {
                UnstickFromWall();
                if (!grounded)
                {
                    jump();
                }
            }
            else if (readyToJump && grounded)
            {
                readyToJump = false;
                jump();
                Invoke(nameof(resetJump), jumpCooldown);
            }
        }
    }

    private void stateHandler()
    {
        if (Input.GetKey(crouchkey))
        {
            state = MovementState.crouching;
            moveSpeed = crouchSpeed;
        }

        // Mode - Sprinting
        if (grounded && Input.GetKey(sprintKey))
        {
            state = MovementState.sprinting;
            moveSpeed = sprintSpeed;
        }

        // Mode - Walking
        else if (grounded)
        {
            state = MovementState.walking;
            moveSpeed = walkSpeed;
        }

        // Mode - Air
        else
        {
            state = MovementState.air;
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
        }

        if (grounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        }

        else if (!grounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
        }

        rb.useGravity = !OnSlope();
    }

    private void speedControl()
    {
        if (OnSlope() && !exitingSlope)
        {
            if (rb.velocity.magnitude > moveSpeed)
            {
                rb.velocity = rb.velocity.normalized * moveSpeed;
            }
        }
        else
        {
            Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            if (flatVel.magnitude > moveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * moveSpeed;
                rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
            }
        }
    }

    private void jump()
    {
        exitingSlope = true;
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        player.painAudio.PlayOneShot(player.jumpSound);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        player.spawnJumpParticles();
        ScreenShake.Shake(0.05f, 0.05f);
    }
    private void resetJump()
    {
        readyToJump = true;
        exitingSlope = false;
        ScreenShake.Shake(0.05f, 0.05f);
    }

    private bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
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
            //rb.constraints = RigidbodyConstraints.FreezeAll;
            canStick = false;
            readyToJump = true;
            // Create a Fixed Joint component and attach it to the player and the surface
            FixedJoint joint = gameObject.AddComponent<FixedJoint>();
            joint.connectedBody = hit.rigidbody;

            // Set the anchor and connectedAnchor positions to maintain the current position relative to the surface
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
        //rb.constraints = RigidbodyConstraints.None;
        canStick = true;
        Destroy(GetComponent<FixedJoint>());
    }
}