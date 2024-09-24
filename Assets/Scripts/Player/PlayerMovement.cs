using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Player player;
    private CharacterController characterController;

    [Header("Movement Info")]
    [SerializeField] private Vector3 movementDir;
    private float movementSpeed;
    [SerializeField] private float walkSpeed;
    [SerializeField] private float runSpeed;
    [SerializeField] private float turnSpeed;
    [SerializeField] private float gravityScale = 9.81f;
    [SerializeField] private Vector3 lookingDir;


    [Header("Animation Info")]
    private Animator animator;
    [SerializeField] private bool isRunning;

    private float verticalVelocity;
    public Vector2 moveInput { get; private set; }

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        animator = transform.GetComponentInChildren<Animator>();
        player = GetComponent<Player>();

        movementSpeed = walkSpeed;
        AssignInputsEvent();
    }

    private void Update()
    {
        HandleMovement();
        HandleAimTowardMouse();
        AnimationController();
    }

    private void AnimationController()
    {
        float xVelocity = Vector3.Dot(movementDir.normalized, transform.right);
        float zVelocity = Vector3.Dot(movementDir.normalized, transform.forward);

        animator.SetFloat("xVelocity", xVelocity, .1f, Time.deltaTime);
        animator.SetFloat("zVelocity", zVelocity, .1f, Time.deltaTime);

        bool playRunAnimation = isRunning && movementDir.magnitude > 0;
        animator.SetBool("isRunning", playRunAnimation);
    }

    private void HandleAimTowardMouse()
    {
        lookingDir = player.playerAim.GetHitInfo().point - transform.position;
        lookingDir.y = 0f;
        lookingDir.Normalize();

        Quaternion desiredRotation = Quaternion.LookRotation(lookingDir);
        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, turnSpeed * Time.deltaTime);

        //transform.forward = lookingDir;
    }

    private void HandleMovement()
    {
        movementDir = new Vector3(moveInput.x, 0, moveInput.y);
        ApplyGravity();
        if (movementDir.magnitude > 0)
        {
            characterController.Move(movementDir * movementSpeed * Time.deltaTime);
        }
    }

    private void ApplyGravity()
    {
        if (characterController.isGrounded == false)
        {
            verticalVelocity -= gravityScale * Time.deltaTime;
            movementDir.y = verticalVelocity;
        }
        else
        {
            verticalVelocity = -.5f;
        }
    }

    private void AssignInputsEvent()
    {
        PlayerInputs playerInputs = player.playerInputs;

        playerInputs.Player.Movement.performed += context => moveInput = context.ReadValue<Vector2>();
        playerInputs.Player.Movement.canceled += context => moveInput = Vector2.zero;

        

        playerInputs.Player.Run.performed += context =>
        {
            isRunning = true;
            movementSpeed = runSpeed;
        };

        playerInputs.Player.Run.canceled += context =>
        {
            isRunning = false;
            movementSpeed = walkSpeed;
        };
    }
}
