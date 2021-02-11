using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ThirdPersonPlayerController : MonoBehaviour
{
    // Tracking of player's input
    private Vector2 moveDirection;
    private float jumpDirection;

    // Calculations of the player's speed in fn. Move()
    public float maxForwardSpeed = 8;
    private float desiredSpeed;
    private float forwardSpeed;
    private const float groundAcceleration = 5;
    private const float groundDeceleration = 25f;

    // Turning
    public float turnSpeed = 100;

    // Jumping
    bool readyJump = false;

    // Jumping with physics
    bool onGround = true;
    float groundRayDistance = 2f;

    // Handles for needed components
    private Animator _anim;
    private Rigidbody _rb;

    /// Input event that returns Vector2 as "context" and assigns it to Vector2 moveDirection
    public void OnMove(InputAction.CallbackContext context)
    {
        moveDirection = context.ReadValue<Vector2>();
        // Debug.Log(moveDirection);
    }

    // Input event ...
    public void OnJump(InputAction.CallbackContext context)
    {
        jumpDirection = context.ReadValue<float>();
    }

    // Helper for calculation of acceleration or deceleration, returns 0 if not moving
    bool IsMoveInput
    {
        get
        {
            return !Mathf.Approximately(moveDirection.sqrMagnitude, 0f);
        }
    }


    // Move character using the Vector2 moveDirection provided by OnMove()
    void Move(Vector2 moveDirection)
    {
        // Store directions from the input
        float forwardDirection = moveDirection.y;
        float turnAmount = moveDirection.x;

        // Clamp the strength of the direction vector between 0..1
        if (moveDirection.sqrMagnitude > 1f)
        {
            moveDirection.Normalize();
        }

        // Calculate the desired forward or backwards speed
        desiredSpeed = moveDirection.magnitude * maxForwardSpeed * Mathf.Sign(forwardDirection);

        // Automatic slowing down based on the input
        float acceleration = IsMoveInput ? groundAcceleration : groundDeceleration;

        // Calculates forward speed
        forwardSpeed = Mathf.MoveTowards(forwardSpeed, desiredSpeed, acceleration * Time.deltaTime);

        // Feed forward speed to the Animator
        _anim.SetFloat("ForwardSpeed", forwardSpeed);

        // Rotate the character
        transform.Rotate(0, turnAmount * turnSpeed * Time.deltaTime, 0);
    }

    void Jump(float jumpDirection)
    {
        if (jumpDirection > 0 && onGround)
        { 
            readyJump = true;
            _anim.SetBool("ReadyJump", true);
        }
        else if(readyJump)
        {
            _anim.SetBool("ReadyJump", false);
            readyJump = false;
        }
    }

    void OnCollisionEnter(Collision col)
    {
        //_anim.SetBool("Land", true);
    }

    public void Land()
    {
        // Debug.Log("Character Landed");
        _anim.SetBool("Launch", false);
        _anim.SetBool("Land", false);
        onGround = true;
    }

    void Start()
    {
        _anim = this.GetComponent<Animator>();
        _rb = this.GetComponent<Rigidbody>();
    }

    void Update()
    {
        Move(moveDirection);
        Jump(jumpDirection);

        // Detect landing with ray - OnCollisionEnter detects landing only once character is grounded already
        RaycastHit hit;
        Ray ray = new Ray(transform.position + Vector3.up * groundRayDistance * 0.5f, -Vector3.up);
        if(Physics.Raycast(ray, out hit, groundRayDistance))
        {
            if (!onGround)
            {
                onGround = true;
                _anim.SetBool("Land", true);
            }
            else
            {
                onGround = false;
            }
        }
        Debug.DrawRay(transform.position + Vector3.up * groundRayDistance * 0.5f, -Vector3.up * groundRayDistance, Color.red);
    }
}
