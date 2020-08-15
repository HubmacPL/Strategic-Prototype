using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Camera speed settings")]
    public float cameraMoveSpeed = 10f;
    public float cameraRotateSpeed = 5.0f;
    public float cameraScroolSpeed = 30.0f;

    [Header("Camera angle range")]
    [Space(10)]
    [Range(20f, 90f)] public float cameraUpperRange = 55.0f;
    [Range(-10, 10)] public float cameraLowerRange = 0.0f;

    private float rotationX = 0f;
    private float rotationY = 0f;
    private bool cursorInMargin = false;
    private Vector2 screenSize;
    private int screenMargin = 10;
    private Vector3 mousePosition;


    private void Start()
    {
    }

    private void Update()
    {
        MoveCam();
    }
    void MoveCam()
    {
        mousePosition = Input.mousePosition;
        screenSize.x = Screen.width;
        screenSize.y = Screen.height;

        Vector3 forceVector = new Vector3(0, 0, 0);

        //Margin move
        if (mousePosition.x >= screenSize.x - screenMargin)
        {
            forceVector.x = 1;
            cursorInMargin = true;
        }
        else if (mousePosition.x <= screenMargin)
        {
            forceVector.x = -1;
            cursorInMargin = true;
        }
        else
        {
            forceVector.x = 0;
            cursorInMargin = false;
        }

        if(mousePosition.y >= screenSize.y - screenMargin)
        {
            forceVector.z = 1;
            cursorInMargin = true;
        }
        else if(mousePosition.y <= screenMargin)
        {
            forceVector.z = -1;
            cursorInMargin = true;
        }
        else
        {
            forceVector.z = 0;
            cursorInMargin = false;
        }

        //ScroolRool move
        if (!cursorInMargin)
        {
            forceVector.z = Input.GetAxis("Mouse ScrollWheel") * cameraScroolSpeed;
        }
        //Left Shift move
        if (!cursorInMargin && Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.Mouse2))
        {
            forceVector.x = Input.GetAxis("Mouse X") * cameraMoveSpeed * -1;
            forceVector.y = Input.GetAxis("Mouse Y") * cameraMoveSpeed * -1;
        }
        else if (!cursorInMargin && Input.GetKey(KeyCode.Mouse2)) //ScroolButton move (Rotate camera)
        {
            rotationY += Input.GetAxis("Mouse X") * cameraRotateSpeed;
            rotationX += Input.GetAxis("Mouse Y") * cameraRotateSpeed;

            rotationX = Mathf.Clamp(rotationX, cameraLowerRange, cameraUpperRange);

            transform.rotation = Quaternion.Euler(rotationX, rotationY, 0);

        }

        Vector3 cameraVector = transform.rotation * forceVector;

        transform.position = transform.position + cameraVector * cameraMoveSpeed * Time.deltaTime;
    }
}

