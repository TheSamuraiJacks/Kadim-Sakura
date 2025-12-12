using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrControllerRoot : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 1f;
    public float sprintSpeed = 3f;
    public float gravity = -9.8f;
    public float jumpHeight = 1f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    [SerializeField] private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;

    public Animator animator;
    [SerializeField] private Rigidbody rb;

    [SerializeField] float mouseSensitivity = 500f;
    float xRotation = 0;

    public Transform followTarget;
    public float rotPower = 10f;
    public bool isMoving;
    private void Update()
    {
        GroundCheck();
        MovePlayer();

        // Zýplama
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            Jump();
        }

    }
    void GroundCheck()
    {
        // E?er yer ile temas varsa a?a?? do?ru h?z s?f?rlan?r
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;
    }

    void MovePlayer()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        if (x != 0 || z != 0)
        {
            
            isMoving = true;
            Rotation();
        }
        else
        {
            isMoving = false;
        }


        // Koþma
        animator.SetBool("isRunning", Input.GetKey(KeyCode.LeftShift) ? true : false);

        animator.SetFloat("XSpeed", x);
        animator.SetFloat("ZSpeed", z);


        if (Physics.Raycast(groundCheck.position, Vector3.down, out RaycastHit hit, groundDistance, groundMask))
        {
            float distance = hit.distance;
            animator.SetFloat("GroundDistance", distance);
        }

        // Yerçekimi
        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
    }

    public void Jump()
    {

        if (isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            animator.SetTrigger("isJumped");

            if (AudioManager.instance != null)
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
        transform.Rotate(Vector3.up * mouseX);
        //followTarget ayarý
        followTarget.transform.rotation = Quaternion.Lerp(followTarget.transform.rotation, transform.rotation, Time.deltaTime * rotPower);
    }
}
