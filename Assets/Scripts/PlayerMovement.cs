using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 10f;
    public float sprintSpeed = 20f;
    public float jumpForce = 7f;
    public bool is3DView = false;  // Set to true when in 3D view, false in 2D view

    [Header("Mouse Settings for 3D")]
    public float mouseSensitivity = 100f;
    private float xRotation = 0f;

    private Rigidbody rb;
    private bool isGrounded = true;
    private bool facingRight = true;

    [Header("Cinemachine Cameras")]
    public CinemachineVirtualCamera camera2D;
    public CinemachineVirtualCamera camera3D;

    [Header("Animation Settings")]
    public Animator animator;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Start with the 2D camera active
        ToggleCameraView(is3DView);

        if (is3DView)
        {
            // Lock and hide the cursor for 3D view
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    private void Update()
    {
        // Toggle between 2D and 3D view on Tab key press
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            is3DView = !is3DView;
            ToggleCameraView(is3DView);
        }

        if (is3DView)
        {
            Move3D();
            RotateWithMouse();
        }
        else
        {
            Move2D();
        }

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            Jump();
        }

        UpdateAnimation();
    }

    private void Move2D()
    {
        float moveX = Input.GetAxis("Horizontal");
        float speed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed;

        Vector3 move = new Vector3(moveX, 0, 0) * speed * Time.deltaTime;
        rb.MovePosition(transform.position + move);

        // Face player in the direction of movement
        if (moveX > 0)
        {
            transform.rotation = Quaternion.Euler(0, 90, 0);
            facingRight = true;
        }
        else if (moveX < 0)
        {
            transform.rotation = Quaternion.Euler(0, -90, 0);
            facingRight = false;
        }
    }

    private void Move3D()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        float speed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed;

        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        rb.MovePosition(transform.position + move * speed * Time.deltaTime);
    }

    private void RotateWithMouse()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        Camera.main.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    private void Jump()
    {
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        isGrounded = false;
        animator.SetTrigger("Jump");
    }

    private void ToggleCameraView(bool switchTo3D)
    {
        if (switchTo3D)
        {
            // Activate 3D Camera
            camera3D.Priority = 10;
            camera2D.Priority = 0;

            // Lock and hide the cursor for 3D view
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            // Activate 2D Camera
            camera2D.Priority = 10;
            camera3D.Priority = 0;

            // Reset player rotation to face forward along the X-axis in 2D
            transform.rotation = Quaternion.Euler(0, 90, 0);

            // Reset 2D camera position and rotation
            camera2D.transform.position = new Vector3(transform.position.x, transform.position.y, camera2D.transform.position.z);
            camera2D.transform.rotation = Quaternion.Euler(0, 0, 0); // Faces the player along the X-axis

            // Unlock and show the cursor for 2D view
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    private void UpdateAnimation()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = is3DView ? Input.GetAxis("Vertical") : 0;
        float speed = Mathf.Abs(horizontalInput) + Mathf.Abs(verticalInput);

        animator.SetFloat("Speed", speed);
        animator.SetBool("IsGrounded", isGrounded);
        animator.SetBool("Is3DView", is3DView);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    public float GetFacing()
    {
        if (facingRight) return 1; else return -1;
    }
}
