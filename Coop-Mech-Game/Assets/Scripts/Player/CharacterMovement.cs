using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class CharacterMovement : BaseMovement
{
    #region Variables

    [Header("Character - Ground Movement")]
    [SerializeField] private bool canMove = true;
    [SerializeField] private float accelerationRate = 60f;
    [SerializeField] private float decelerationRate = 30f;
    [SerializeField] private float maxWalkSpeed = 4f;
    [SerializeField] private float maxVerticalSpeed = 25f;

    [Header("Character - Air Movement")]
    [SerializeField] private int maxJumps = 2;
    private int currentJumps = 0;
    [SerializeField] private float jumpCoolDown = 0.25f;
    [SerializeField] private float airControlMultiplier = 0.4f;
    private bool readyToJump = true;

    [Header("Player - Rotation")]
    [SerializeField] private float groundRotationRate = 90f; // degrees/sec
    [SerializeField] private float airRotationRate = 45f;
    [SerializeField] private float upDownRotationRate = 45f;

    [Header("Player - Ground Check")]
    [SerializeField] private float groundCheckDistance = 0.1f;
    [SerializeField] private LayerMask environmentLayerMask;
    private bool wasGroundedLastFrame = false;
    [SerializeField] private bool isGrounded = false;

    [Header("Camera")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private GameObject mainHandleBone;
    [SerializeField] private CinemachineImpulseSource impulseSource;
    [SerializeField][Range(0.01f, 3)] private float impulseRate;
    private float impulseTimer;

    [Header("Audio References")]
    [SerializeField] private MovementSFXManager movementSFXManager;


    #endregion

    #region Unity Functions

    private void Awake()
    {
        currentMaxSpeed = maxWalkSpeed;
    }

    private void FixedUpdate()
    {
        CheckIsGrounded();
        MoveCharacter();
        CharacterLook();
        LimitVelocity();
    }

    private void Update()
    {
        RotateCharacter();
        if (rigidbody.velocity.sqrMagnitude > 0.1f)
        {
            impulseTimer += Time.deltaTime;
            if (impulseTimer >= impulseRate)
            {
                impulseSource.GenerateImpulse();
                impulseTimer = 0f;
                movementSFXManager.PlayFootstepSound();
            }
        }
        else
        {
            impulseTimer = 0f;
        }
    }

    #endregion

    #region Input

    public override void SetMovementInput(Vector2 input)
    {
        base.SetMovementInput(input);

        // movementDirection.x = rotation input (A/D)
        // movementDirection.z = forward/back input (W/S)
        movementDirection = new Vector3(input.x, 0, input.y);
        if (movementDirection.sqrMagnitude > 1f)
            movementDirection.Normalize();
    }

    public override void SetLookInput(float input)
    {
        base.SetLookInput(input);

    }

    #endregion

    #region Movement

    protected override void MoveCharacter()
    {
        if (!canMove) return;

        // Forward/back movement relative to camera forward
        if (movementDirection.z != 0)
        {
            Vector3 camForward = playerCamera.transform.forward;
            camForward.y = 0;
            camForward.Normalize();

            Vector3 move = camForward * movementDirection.z;

            if (isGrounded)
                rigidbody.AddForce(move * accelerationRate, ForceMode.Acceleration); //TODO: i think here is mech rig, don't use physics?
            else
                rigidbody.AddForce(move * accelerationRate * airControlMultiplier, ForceMode.Acceleration);
        }
        else if (isGrounded)
        {
            // Deceleration when no input
            Vector3 horizontalVel = GetHorizontalRBVelocity();
            if (horizontalVel.magnitude > 0.5f)
            {
                Vector3 counteract = -horizontalVel.normalized;
                rigidbody.AddForce(counteract * decelerationRate, ForceMode.Acceleration);
            }
        }

        if(movementDirection.z == 0)
        {
            rigidbody.velocity = Vector3.zero;
        }
    }

    [SerializeField] private float cameraPitch = 0f;

    private void CharacterLook()
    {
        if (lookInput != 0)
        {
            cameraPitch -= lookInput * upDownRotationRate * Time.deltaTime;
            cameraPitch = Mathf.Clamp(cameraPitch, -60f, 60f);

            // Only modify pitch (local x-rotation)
            Vector3 euler = playerCamera.transform.localEulerAngles;
            // playerCamera.transform.localEulerAngles = new Vector3(cameraPitch, euler.y, euler.z);
            mainHandleBone.transform.localEulerAngles = new Vector3(cameraPitch, euler.y, euler.z);


            //TODO: call lose event in game state and apply ragdoll
        }
    }

    protected override void RotateCharacter()
    {
        if (movementDirection.x != 0)
        {
            float rotationSpeed = isGrounded ? groundRotationRate : airRotationRate;
            characterModel.Rotate(Vector3.up, movementDirection.x * rotationSpeed * Time.deltaTime);
        }
    }

    private void LimitVelocity()
    {
        Vector3 horizontalVel = GetHorizontalRBVelocity();
        if (horizontalVel.magnitude > currentMaxSpeed)
        {
            Vector3 counteract = -horizontalVel.normalized;
            float excess = horizontalVel.magnitude - currentMaxSpeed;
            rigidbody.AddForce(counteract * excess, ForceMode.VelocityChange);
        }

        if (Mathf.Abs(rigidbody.velocity.y) > maxVerticalSpeed)
        {
            Vector3 counteract = Vector3.up * -Mathf.Sign(rigidbody.velocity.y);
            float excessY = Mathf.Abs(rigidbody.velocity.y) - maxVerticalSpeed;
            rigidbody.AddForce(counteract * excessY, ForceMode.VelocityChange);
        }
    }

    public override void Jump()
    {
        if (readyToJump && (isGrounded || currentJumps < maxJumps))
        {
            currentJumps++;
            float adjustedJumpForce = jumpForce - rigidbody.velocity.y;
            rigidbody.AddForce(Vector3.up * adjustedJumpForce, ForceMode.VelocityChange);
            readyToJump = false;
            StartCoroutine(JumpCooldownCoroutine());
        }
    }

    private IEnumerator JumpCooldownCoroutine()
    {
        yield return new WaitForSeconds(jumpCoolDown);
        readyToJump = true;
    }

    public override void CancelJump()
    {
        if (rigidbody.velocity.y > 0f)
        {
            rigidbody.AddForce(Vector3.down * (rigidbody.velocity.y * 0.5f), ForceMode.VelocityChange);
        }
    }

    #endregion

    #region Ground Check

    private void CheckIsGrounded()
    {
        wasGroundedLastFrame = isGrounded;

        isGrounded = Physics.SphereCast(transform.position, capsuleCollider.radius + 0.1f, Vector3.down,
            out RaycastHit hit, groundCheckDistance, environmentLayerMask);

        if (!isGrounded)
        {
            Vector3 p1 = transform.position + Vector3.down * (capsuleCollider.radius + 0.1f);
            Collider[] colliders = Physics.OverlapSphere(p1, capsuleCollider.radius + groundCheckDistance, environmentLayerMask);
            isGrounded = colliders.Length > 0;
        }

        if (!wasGroundedLastFrame && isGrounded)
            currentJumps = 0;
        else if (wasGroundedLastFrame && !isGrounded && currentJumps == 0)
            currentJumps = 1;
    }

    #endregion

    #region Helper Functions

    private Vector3 GetHorizontalRBVelocity()
    {
        return Vector3.ProjectOnPlane(rigidbody.velocity, Vector3.up);
    }

    #endregion

    #region Gizmos

    private void OnDrawGizmos()
    {
        Gizmos.color = isGrounded ? Color.green : Color.red;
        Vector3 p1 = transform.position + Vector3.down * groundCheckDistance;
        Gizmos.DrawWireSphere(p1, capsuleCollider.radius + 0.1f);
    }

    #endregion

    public bool GetIsGrounded()
    {
        return isGrounded;
    }
}