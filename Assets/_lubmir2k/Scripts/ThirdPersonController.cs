using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ThirdPersonController : MonoBehaviour
{
    // Tracking of player's input
    private Vector2 moveDirection;

    // Calculations of the player's speed in fn. Move()
    public float maxForwardSpeed = 8;
    private float desiredSpeed;
    private float forwardSpeed;
    private const float groundAcceleration = 5;
    private const float groundDeceleration = 25f;

    // Turning
    public float turnSpeed = 100;

    private Animator _anim;

    /// Input event that returns Vector2 as "context" and assigns it to Vector2 moveDirection
    public void OnMove(InputAction.CallbackContext context)
    {
        moveDirection = context.ReadValue<Vector2>();
        // Debug.Log(moveDirection);
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

    void Start()
    {
        _anim = this.GetComponent<Animator>();
    }

    void Update()
    {
        Move(moveDirection);
    }
}
