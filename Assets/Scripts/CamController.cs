using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamController : MonoBehaviour
{
    [SerializeField] float mouseSensitivity = 500f;
    [SerializeField] Transform playerBody;

    float xRotation = 0;
    void Start()
    {
        // Mouse'u kilitle
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    void Update()
    {
        // Mouse input
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Yukarý-aþaðý rotasyon (kamera)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -85f, 85f);

        playerBody.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Saða-sola rotasyon (player)
        playerBody.Rotate(Vector3.up * mouseX);
    }
}
