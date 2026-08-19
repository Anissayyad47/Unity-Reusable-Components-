using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public enum PlayerState { Idle, Move, Jump }
    public PlayerState currentState = PlayerState.Idle;
    [Header("Reference")]
    [SerializeField] private Animator animator;
    [SerializeField] private CharacterController controller;

    [Header("Player Attributes ")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float jumpHeight = 3f;

    [Header("Acting Forces")]
    [SerializeField] private float gravity = -30f;

    private UserInputAction action;
    
    private Vector2 moveInput;
    private Vector3 moveDirection;
    private float verticalVelocity;
//  Booleans -------------------------------------------------------------------
    private bool isGround=false;
    public bool isRunning=false;

    private void Awake()
    {
        action = new UserInputAction();
        if(controller==null)controller = GetComponent<CharacterController>();
        if(animator==null)animator = GetComponent<Animator>();
    }
    private void OnEnable()
    {
        action.Enable();
        action.Player.Move.performed += InputKeyMovePressed;
        action.Player.Move.canceled += InputKeyMoveCancelled;
        action.Player.Jump.performed += InputKeySpacePressed;
    }

    private void OnDisable()
    {
        action.Player.Move.performed -= InputKeyMovePressed;
        action.Player.Move.canceled -= InputKeyMoveCancelled;
        action.Player.Jump.performed -= InputKeySpacePressed;
        action.Disable();
    }
    private void InputKeyMovePressed(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
        isRunning=true;
        if(currentState != PlayerState.Jump)
        {
            animator.SetBool("isWalk",true);
            ChangeState(PlayerState.Move);
        }
    }
    private void InputKeyMoveCancelled(InputAction.CallbackContext context)
    {
        moveInput = Vector2.zero;
        moveDirection = Vector3.zero;
        isRunning=false;
        if(currentState != PlayerState.Jump)
        {
            animator.SetBool("isWalk",false);
            ChangeState(PlayerState.Idle);
        }
    }
    private void InputKeySpacePressed(InputAction.CallbackContext context)
    {
        if (!isGround)
            return;
        ChangeState(PlayerState.Jump);
        animator.SetBool("isWalk",false);
        animator.SetTrigger("Jump");
        verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
    }
    private void Update()
    {
        GroundCheck();
        HandleStates();
        ApplyGravity();
        ControllerMove();
    }
    private void ChangeState(PlayerState newState)
    {
        if (currentState == newState)
            return;

        currentState = newState;
    }
    private void HandleStates()
    {
        switch (currentState)
        {
            case PlayerState.Idle:
                HandleStateIdle();
                break;
            case PlayerState.Move:
                HandleStateMove();
                break;
            case PlayerState.Jump:
                HandleStateJump();
                break;
        }

    }
    private void ControllerMove()
    {
        Vector3 finalMovement = moveDirection * moveSpeed + Vector3.up * verticalVelocity;
        controller.Move(finalMovement * Time.deltaTime);
    }
    private void HandleStateIdle()
    {
        moveDirection = Vector3.zero;
    }
    private void HandleStateMove()
    {
        moveDirection.x=moveInput.x;
        moveDirection.z=moveInput.y;
        RotatePlayer();
    }
    private void HandleStateJump()
    {
        moveDirection.x=moveInput.x;
        moveDirection.z=moveInput.y;
        RotatePlayer();
    }
    private void ApplyGravity()
    {
        if (isGround && verticalVelocity < 0)
        {
            verticalVelocity = -2f;
        }
        verticalVelocity += gravity * Time.deltaTime;
    }
    private void GroundCheck()
    {
        isGround = controller.isGrounded;
        if(isGround && currentState == PlayerState.Jump && verticalVelocity < 0)
        {
            if (isRunning)
            {
                animator.SetBool("isWalk",true);
                ChangeState(PlayerState.Move);
            }else
            {
                animator.SetBool("isWalk",false);
                ChangeState(PlayerState.Idle);
            }
        }
    }
    private void RotatePlayer()
    {
        if (moveDirection.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }
    }
}
