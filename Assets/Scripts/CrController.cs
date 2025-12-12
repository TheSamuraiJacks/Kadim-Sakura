using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 6f;
    public float sprintSpeed = 10f;
    public float gravity = -20f;
    public float jumpHeight = 1.5f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    [SerializeField] private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;

    public Animator animator;
    [SerializeField]private Rigidbody rb;

    [SerializeField] float mouseSensitivity = 500f;
    float xRotation = 0;
    private void Update()
    {
        GroundCheck();
        MovePlayer();
        Rotation();
        // Z�plama
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            Jump();
        }
    }

    void GroundCheck()
    {
        // E�er yer ile temas varsa a�a�� do�ru h�z s�f�rlan�r
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;
    }

    void MovePlayer()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        // Y�n vekt�r�: ileri-geri + sa�-sol
        Vector3 move = transform.right * x + transform.forward * z;

        // Ko�ma
        float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed;
        animator.SetBool("isRunning", Input.GetKey(KeyCode.LeftShift) ? true : false);

        controller.Move(move * currentSpeed * Time.deltaTime);
        animator.SetFloat("XSpeed", x);
        animator.SetFloat("ZSpeed", z);
        

        if (Physics.Raycast(groundCheck.position, Vector3.down, out RaycastHit hit, groundDistance, groundMask))
        {
            float distance = hit.distance;
            animator.SetFloat("GroundDistance", distance);
        }

        // Yer�ekimi
        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
    }

    public void Jump()
    {

        if (isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            animator.SetTrigger("isJumped");
            
            if(AudioManager.instance != null)
            {
                AudioManager.instance.Play("Jump");
            }
        }
    }

    public void Rotation()
    {
        // Mouse input
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;

        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        //transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
        transform.Rotate(Vector3.up * mouseX);
    }
}