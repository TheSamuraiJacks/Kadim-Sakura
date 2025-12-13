using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamFollowTarget : MonoBehaviour
{
    public GameObject target;
    [SerializeField] float mouseSensitivity = 500f;

    float xRotation = 0;
    float yRotation = 0;

    public float rotPower = 8.0f;
    void Start()
    {
        // Mouse'u kilitle
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    
    void LateUpdate()
    {
        // Yön
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        //Sað-Sol Rotasyon
        yRotation += mouseX;
        //Yukarý-Aþaðý Rotasyon
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        Quaternion targetRot = Quaternion.Euler(xRotation, yRotation, 0);
        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRot, Time.deltaTime * rotPower);

        //Hareket
        if(target != null)
            if (transform.position != target.transform.position)
                transform.position = new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z);
    }
}
