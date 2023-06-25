using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplingGun : MonoBehaviour
{
    private LineRenderer lr;
    private Vector3 grapplePoint;
    public LayerMask whatIsGrappleable;
    public Transform gunTip, playerCamera, player;
    private float maxDistance = 100f;
    private SpringJoint joint;

    public AudioSource audioSource;
    public AudioClip grappleSound;

    // Adjust these values to control the swinging effect
    public float swingForce = 10f;
    public float maxSwingAngle = 90f;

    void Awake()
    {
        lr = GetComponent<LineRenderer>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            StartGrapple();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            StopGrapple();
        }
    }

    // Called after Update
    void LateUpdate()
    {
        DrawRope();
    }

    void StartGrapple()
    {
        RaycastHit hit;
        if (Physics.Raycast(playerCamera.position, playerCamera.forward, out hit, maxDistance, whatIsGrappleable))
        {
            grapplePoint = hit.point;
            joint = player.gameObject.AddComponent<SpringJoint>();
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedAnchor = grapplePoint;

            float distanceFromPoint = Vector3.Distance(player.position, grapplePoint);

            joint.maxDistance = distanceFromPoint * 0.9f; 
            joint.minDistance = distanceFromPoint * 0.1f; 

            joint.spring = 10f; // Increase the spring value
            joint.damper = 15f; // Increase the damper value
            joint.massScale = 4.5f;

            lr.positionCount = 2;
            currentGrapplePosition = gunTip.position;

            audioSource.PlayOneShot(grappleSound);
        }
    }

    public void StopGrapple()
    {
        if (joint == null)
        {
            return;
        }
        lr.positionCount = 0;
        Destroy(joint);
    }

    private Vector3 currentGrapplePosition;

    void DrawRope()
    {
        if (!joint) return;

        currentGrapplePosition = Vector3.Lerp(currentGrapplePosition, grapplePoint, Time.deltaTime * 8f);

        lr.SetPosition(0, gunTip.position);
        lr.SetPosition(1, currentGrapplePosition);

        // Calculate the direction from the player to the grapple point
        Vector3 grappleDirection = (grapplePoint - player.position).normalized;

        // Calculate the angle between the player's forward direction and the grapple direction
        float angle = Vector3.Angle(player.forward, grappleDirection);

        // Apply swing force based on the angle
        if (angle <= maxSwingAngle)
        {
            // Adjust the swing force calculation
            Vector3 swingForceVector = Quaternion.AngleAxis(angle, Vector3.Cross(player.forward, grappleDirection)) * player.forward * swingForce;
            player.GetComponent<Rigidbody>().AddForce(swingForceVector, ForceMode.Acceleration);
        }
    }

    public bool IsGrappling()
    {
        return joint != null;
    }

    public Vector3 GetGrapplePoint()
    {
        return grapplePoint;
    }
}
