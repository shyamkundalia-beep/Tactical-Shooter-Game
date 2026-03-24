using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MouseLook : MonoBehaviour
{
    [Header("Min and Max Camera View")]
    private const float YMin = -50f;
    private const float YMax = 50f;


    [Header("Camera View")]
    public Transform lookAt;
    public Transform player;


    [Header("Camera Position")]
    public float CameraDistance = 10f;
    private float currentX = 0.0f;
    private float currentY = 0.0f;
    public float CameraSensitivity = 4f;
    
    public FloatingJoystick floatingjoystick;

    private void LateUpdate()
    {
        currentX +=  floatingjoystick.Horizontal * CameraSensitivity * Time.deltaTime;
        currentY -= floatingjoystick.Vertical * CameraSensitivity * Time.deltaTime;

        currentY = Mathf.Clamp(currentY, YMin, YMax);

        Vector3 Direction = new Vector3(0, 0, -CameraDistance);

        Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);

        transform.position = lookAt.position + rotation * Direction;

        transform.LookAt(lookAt.position);
    }
}