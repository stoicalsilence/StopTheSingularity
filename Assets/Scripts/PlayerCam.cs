using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCam : MonoBehaviour
{
    public float sensitivity;
    public Transform orientation;

    float xRotation;
    float yRotation;
    public float tiltAngle;
    private PlayerMovement playerMovement;
    // Start is called before the first frame update
    void Start()
    {
        playerMovement = FindObjectOfType<PlayerMovement>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.fixedDeltaTime * sensitivity;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.fixedDeltaTime * sensitivity;

        yRotation += mouseX;
        xRotation -= mouseY;

        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        if (playerMovement.isCrouching && playerMovement.slideSpeed > 0)
        {
            transform.rotation = Quaternion.Euler(xRotation, yRotation, tiltAngle);
        }
        else
        {
            transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        }
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);
    }
}
