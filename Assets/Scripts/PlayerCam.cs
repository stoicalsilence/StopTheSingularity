using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCam : MonoBehaviour
{
    public float sensitivity;
    public Transform orientation;
    public Transform cameraHolder;
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
        sensitivity = PlayerPrefs.GetFloat("Sensitivity", 300);
    }

    // Update is called once per frame
    void Update()
    {
        if (!FindObjectOfType<Player>().menu.activeInHierarchy)
        {
            float mouseX = Input.GetAxisRaw("Mouse X") * Time.fixedDeltaTime * sensitivity;
            float mouseY = Input.GetAxisRaw("Mouse Y") * Time.fixedDeltaTime * sensitivity;

            yRotation += mouseX;
            xRotation -= mouseY;
        }

        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        if (playerMovement.isCrouching && playerMovement.slideSpeed > 0)
        {
            cameraHolder.rotation = Quaternion.Euler(xRotation, yRotation, tiltAngle);
        }
        else
        {
            cameraHolder.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        }
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);
    }

    public void DoFov(float endValue)
    {
        GetComponent<Camera>().DOFieldOfView(endValue, 0.25f);
    }

    public void DoTilt(float zTilt)
    {
        transform.DOLocalRotate(new Vector3(0, 0, zTilt), 0.25f);
    }
}
