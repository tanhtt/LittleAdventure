using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private PlayerInputs playerInputs;

    private CharacterController characterController;

    [Header("Movement Info")]
    [SerializeField] private Vector3 movementDir;
    private float movementSpeed;
    [SerializeField] private float walkSpeed;
    [SerializeField] private float runSpeed;
    [SerializeField] private float gravityScale = 9.81f;

    [Header("Aim Info")]
    [SerializeField] private Transform aimTransform;
    [SerializeField] private Vector3 lookingDir;
    [SerializeField] private LayerMask layerMask;

    [Header("Animation Info")]
    private Animator animator;
    [SerializeField] private bool isRunning;

    private float verticalVelocity;
    private Vector2 moveInput;
    private Vector2 aimInput;

    private void Awake()
    {
        AssignInputsEvent();
    }

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        animator = transform.GetComponentInChildren<Animator>();

        movementSpeed = walkSpeed;
    }

    private void Update()
    {
        HandleMovement();
        HandleAimTowardMouse();
        AnimationController();
    }

    private void Shoot()
    {
        animator.SetTrigger("Fire");
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
        Ray ray = Camera.main.ScreenPointToRay(aimInput);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask))
        {
            lookingDir = hit.point - transform.position;
            lookingDir.y = 0f;
            lookingDir.Normalize();

            transform.forward = lookingDir;

            aimTransform.position = new Vector3(hit.point.x, hit.point.y, hit.point.z);
        }
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

    #region Input action
    private void AssignInputsEvent()
    {
        playerInputs = new PlayerInputs();

        playerInputs.Player.Movement.performed += context => moveInput = context.ReadValue<Vector2>();
        playerInputs.Player.Movement.canceled += context => moveInput = Vector2.zero;

        playerInputs.Player.Aim.performed += context => aimInput = context.ReadValue<Vector2>();
        playerInputs.Player.Aim.canceled += context => aimInput = Vector2.zero;

        playerInputs.Player.Fire.performed += context => Shoot();

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

    private void OnEnable()
    {
        playerInputs.Enable();
    }

    private void OnDisable()
    {
        playerInputs.Disable();
    }

    #endregion
}
