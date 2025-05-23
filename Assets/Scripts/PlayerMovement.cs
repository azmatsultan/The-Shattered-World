using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PlayerMovement : MonoBehaviour
{
    
    public enum MovementType
    {
        Keyboard,
        Controller
    };

    public MovementType movementType;
    
    [Header("Movement Settings")]
    public float walkSpeed = 10f;
    public float sprintSpeed = 20f;
    public float jumpForce = 7f;
    public bool is3DView = false;  // Set to true when in 3D view, false in 2D view

    [Header("Mouse Settings for 3D")]
    public float mouseSensitivity = 100f;
    private float xRotation = 0f;

    [Header("Joystick Settings for 3D")]
    public float joystickSensitivity = 100f;

    private Rigidbody rb;
    private bool isGrounded = true;
    private bool facingRight = true;

    [Header("Cinemachine Cameras")]
    public CinemachineVirtualCamera camera2D;
    public CinemachineVirtualCamera camera3D;

    [Header("Animation Settings")]
    public Animator animator;

    [Header("Audio Settings")]
    public AudioSource audioSource;  // The single AudioSource attached to the player
    public AudioClip moveClip;       // Movement sound clip
    

    public Transform respawnPoint;
    
    playerHealth thePlayerHealth;

    private void Awake()
    {
        thePlayerHealth = gameObject.GetComponent<playerHealth>();
    }

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

    private void FixedUpdate()
    {
        

        if (is3DView)
        {
            Move3D();
            RotateWithMouse();
        }
        else
        {
            Move2D();
        }


        UpdateMovementSound();
        UpdateAnimation();

        // Check if player has fallen below -0.95 in Y-axis
        if (transform.position.y < -0.95f)
        {
            RespawnPlayer();
        }
    }

    private void Update()
    {
        if (movementType == MovementType.Keyboard)
        {
            if (Input.GetButtonDown("Jump") && isGrounded)
            {
                Jump();
            }

            // Toggle between 2D and 3D view on Tab key press
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                is3DView = !is3DView;
                ToggleCameraView(is3DView);
            }
        }
        if (movementType == MovementType.Controller || GameManager.Instance.isMultiPlayer == false)
        {
            if (Input.GetButtonDown("JumpGamepad") && isGrounded)
            {
                Jump();
            }

            // Toggle between 2D and 3D view on Tab key press
            if (Input.GetButtonDown("CameraGamepad"))
            {
                is3DView = !is3DView;
                ToggleCameraView(is3DView);
            }
        }
    }

    private void Move2D()
    {
        float moveX = 0f;
        float speed = walkSpeed;

        if (!GameManager.Instance.isMultiPlayer || movementType == MovementType.Keyboard)
        {
            moveX += Input.GetAxis("Horizontal");  // Add keyboard input
            if (Input.GetKey(KeyCode.LeftShift)) speed = sprintSpeed;
        }

        if (!GameManager.Instance.isMultiPlayer || movementType == MovementType.Controller)
        {
            moveX += Input.GetAxis("HorizontalGamepad");  // Add controller input
            if (Input.GetAxisRaw("SprintGamepad") > 0.5f) speed = sprintSpeed;
        }

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
        float moveX = 0f;
        float moveZ = 0f;
        float speed = walkSpeed;

        if (!GameManager.Instance.isMultiPlayer || movementType == MovementType.Keyboard)
        {
            moveX += Input.GetAxis("Horizontal");  // Add keyboard input
            moveZ += Input.GetAxis("Vertical");
            if (Input.GetKey(KeyCode.LeftShift)) speed = sprintSpeed;
        }

        if (!GameManager.Instance.isMultiPlayer || movementType == MovementType.Controller)
        {
            moveX += Input.GetAxis("HorizontalGamepad");  // Add controller input
            moveZ += Input.GetAxis("VerticalGamepad");
            if (Input.GetAxisRaw("SprintGamepad") > 0.5f) speed = sprintSpeed;
        }

        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        rb.MovePosition(transform.position + move * speed * Time.deltaTime);
    }


    private void RotateWithMouse()
    {
        if (movementType == MovementType.Keyboard)
        {
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            Camera.main.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
            transform.Rotate(Vector3.up * mouseX);
        }
        if (movementType == MovementType.Controller || GameManager.Instance.isMultiPlayer == false)
        {
            float mouseX = Input.GetAxis("Gamepad X") * joystickSensitivity * Time.deltaTime;
            float mouseY = Input.GetAxis("Gamepad Y") * joystickSensitivity * Time.deltaTime;

            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            Camera.main.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
            transform.Rotate(Vector3.up * mouseX);
        }
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

    private void UpdateMovementSound()
    {
        float moveX = 0f;
        float moveZ = 0f;

        if (!GameManager.Instance.isMultiPlayer || movementType == MovementType.Keyboard)
        {
            moveX += Input.GetAxis("Horizontal");
            moveZ += is3DView ? Input.GetAxis("Vertical") : 0;
        }

        if (!GameManager.Instance.isMultiPlayer || movementType == MovementType.Controller)
        {
            moveX += Input.GetAxis("HorizontalGamepad");
            moveZ += is3DView ? Input.GetAxis("VerticalGamepad") : 0;
        }

        bool isMoving = (moveX != 0 || moveZ != 0) && isGrounded;

        if (isMoving)
        {
            if (!audioSource.isPlaying && moveClip != null)
            {
                audioSource.clip = moveClip;
                audioSource.loop = true;
                audioSource.Play();
            }
        }
        else
        {
            if (audioSource.isPlaying && audioSource.clip == moveClip)
            {
                audioSource.Stop();
            }
        }
    }


    private void UpdateAnimation()
    {
        float horizontalInput = 0f;
        float verticalInput = 0f;

        if (!GameManager.Instance.isMultiPlayer || movementType == MovementType.Keyboard)
        {
            horizontalInput += Input.GetAxis("Horizontal");
            verticalInput += is3DView ? Input.GetAxis("Vertical") : 0;
        }

        if (!GameManager.Instance.isMultiPlayer || movementType == MovementType.Controller)
        {
            horizontalInput += Input.GetAxis("HorizontalGamepad");
            verticalInput += is3DView ? Input.GetAxis("VerticalGamepad") : 0;
        }

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

    private void RespawnPlayer()
    {

        thePlayerHealth.addDamage(5);
        transform.position = respawnPoint.position; // Move player to respawn point
        rb.velocity = Vector3.zero; // Reset velocity to prevent continued falling
         
    }
}
