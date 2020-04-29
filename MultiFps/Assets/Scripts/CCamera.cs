using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CCamera : MonoBehaviour
{
    private Camera camera;

    private GameObject targetObj;
    public float offsetY;
    public float offsetForward;

    public float verticalLimit = 90;

    private float vertical = 0;
    private float horizontal = 0;

    public float growingCameraAmount = 1;
    private float growingCameraSpeed;

    void Start()
    {
        camera = GetComponent<Camera>();
        targetObj = GameObject.FindGameObjectWithTag("Head");

        growingCameraSpeed = growingCameraAmount * 50;
    }

    void Update()
    {
        if (targetObj == null || CGameManager.isOptionOpen)
            return;

        vertical -= Input.GetAxis("Mouse Y") * CGameManager.mouseSensitivity / 2;
        vertical = Mathf.Clamp(vertical, -verticalLimit, verticalLimit);

        horizontal += Input.GetAxis("Mouse X") * CGameManager.mouseSensitivity;
        transform.rotation = Quaternion.Euler(vertical, horizontal, 0);

        transform.position = targetObj.transform.position + (transform.forward * offsetForward) + (Vector3.up * offsetY);
    }
}
