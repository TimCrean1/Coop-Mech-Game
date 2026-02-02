using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using Unity.Netcode;

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
    [SerializeField][Range(0,4)] private float horizontalRotationRate = 2;
    [SerializeField][Range(0,4)] private float verticalRotationRate = 2;
    [SerializeField][Range(0,1)] private float lookClampMin = 0.25f;
    [SerializeField][Range(0,1)] private float lookClampMax = 0.75f;
    [SerializeField][Range(0,0.5f)] private float deadZoneSize = 0.02f;
    private float center = 0.5f;

    [Header("Player - Ground Check")]
    [SerializeField] private float groundCheckDistance = 0.1f;
    [SerializeField] private LayerMask environmentLayerMask;
    private bool wasGroundedLastFrame = false;
    [SerializeField] private bool isGrounded = false;

    [Header("Player - Shooting")]
    [SerializeField] private TeamWeaponManager weaponMgr;

    [Header("Camera")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float cameraPitch = 0f;
    [SerializeField] private Vector3 targetPoint;
    [SerializeField] private CinemachineImpulseSource impulseSource;
    [SerializeField][Range(0.01f, 3)] private float impulseRate;
    private float impulseTimer;

    #endregion

    #region Unity Functions

    private void Awake()
    {
        currentMaxSpeed = maxWalkSpeed;
    }

    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
        rigidbody.angularDamping = 8f;
        // StartupAnimation script sets this to true when start up animation is complete
        canMove = false;
    }

    private void FixedUpdate()
    {
        //if (!IsOwner) { return; }
        CheckIsGrounded();
        if (!canMove) { return; }
        MoveCharacter();
        CharacterLook();
        LimitVelocity(); //TODO: play with limit velocity tuning, especially for synced/unsynced movement
    }

    private void Update()
    {
        Cursor.visible = true;
        RotateCharacter();
        if (rigidbody.linearVelocity.sqrMagnitude > 0.1f)
        {
            impulseTimer += Time.deltaTime;
            if (impulseTimer >= impulseRate)
            {
                //impulseSource.GenerateImpulse();
                impulseTimer = 0f;
                //movementSFXManager.PlayFootstepSound();
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

        movementDirection = new Vector3(input.x, 0, input.y);
        if (movementDirection.sqrMagnitude > 1f)
            movementDirection.Normalize();
    }

    #endregion

    #region Movement

    // Handles character movement based on input direction and camera orientation
    protected override void MoveCharacter()
    {
        // If there is movement input
        if (movementDirection.x != 0 || movementDirection.z != 0)
        {
            // Forward/back movement relative to camera's forward direction
            if (movementDirection.z != 0)
            {
                Vector3 camForward = playerCamera.transform.forward;
                camForward.y = 0; // Ignore vertical component
                camForward.Normalize();

                Vector3 move = camForward * movementDirection.z;

                // Apply force for forward/back movement
                if (isGrounded)
                    rigidbody.AddForce(move * accelerationRate, ForceMode.Acceleration);
                else
                    rigidbody.AddForce(move * accelerationRate * airControlMultiplier, ForceMode.Acceleration);
            }
            // Sideways movement relative to camera's right direction
            if (movementDirection.x != 0)
            {
                Vector3 camRight = playerCamera.transform.right;
                camRight.y = 0; // Ignore vertical component
                camRight.Normalize();

                Vector3 move = camRight * movementDirection.x;

                // Apply force for sideways movement
                if (isGrounded)
                    rigidbody.AddForce(move * accelerationRate, ForceMode.Acceleration);
                else
                    rigidbody.AddForce(move * accelerationRate * airControlMultiplier, ForceMode.Acceleration);
            }
        }
        // If no movement input and character is grounded, apply deceleration
        else if (isGrounded && movementDirection.x == 0 && movementDirection.z == 0)
        {
            Vector3 horizontalVel = GetHorizontalRBVelocity();
            Vector3 dampingForce = -horizontalVel * decelerationRate;
            rigidbody.AddForce(dampingForce, ForceMode.Acceleration);
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

        if (Mathf.Abs(rigidbody.linearVelocity.y) > maxVerticalSpeed)
        {
            Vector3 counteract = Vector3.up * -Mathf.Sign(rigidbody.linearVelocity.y);
            float excessY = Mathf.Abs(rigidbody.linearVelocity.y) - maxVerticalSpeed;
            rigidbody.AddForce(counteract * excessY, ForceMode.VelocityChange);
        }
    }
    #endregion

    #region Rotation
    // Handles character look direction and camera pitch based on look input
    private void CharacterLook()
    {
        // Clamp look input to defined min/max values
        lookInput.x = Mathf.Clamp(lookInput.x, lookClampMin, lookClampMax);
        lookInput.y = Mathf.Clamp(lookInput.y, lookClampMin, lookClampMax);

        // Apply dead zone to look input to prevent jitter near center
        lookInput.x = Mathf.Abs(lookInput.x - 0.5f) < deadZoneSize ? 0.5f : lookInput.x;
        lookInput.y = Mathf.Abs(lookInput.y - 0.5f) < deadZoneSize ? 0.5f : lookInput.y;

        // Convert look input to screen position and create a ray from the camera
        Vector2 screenPos = new Vector2(Screen.width * lookInput.x, Screen.height * lookInput.y);
        Ray ray = playerCamera.ScreenPointToRay(screenPos);
        RaycastHit hit;

        // Raycast to determine the target point in the world
        if (Physics.Raycast(ray, out hit))
        {
            targetPoint = hit.point;
        }

        // Calculate horizontal direction from character to target point
        Vector3 direction = targetPoint - transform.position;
        direction.y = 0;

        // Calculate rotation rates based on look input
        float newHRotRate = horizontalRotationRate * Mathf.Abs(lookInput.x / Screen.width * 2f - 1);
        float newVRotRate = verticalRotationRate * Mathf.Abs(lookInput.y / Screen.height * 2f - 1);

        if (direction.sqrMagnitude > 0.1f)
        {
            // Smoothly rotate character horizontally towards target point
            Quaternion targetYaw = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetYaw, newHRotRate * Time.deltaTime);

            // Calculate pitch for camera and smoothly apply it
            Vector3 lookDir = targetPoint - playerCamera.transform.position;
            float pitch = Mathf.Atan2(lookDir.y, new Vector2(lookDir.x, lookDir.z).magnitude) * Mathf.Rad2Deg;
            cameraPitch = Mathf.Lerp(cameraPitch, pitch, newVRotRate * Time.deltaTime);
            playerCamera.transform.localRotation = Quaternion.Euler(-cameraPitch, 0, 0);
        }
    }
    #endregion

    #region Shooting
    public override void Shoot(float shootInput)
    {
        if (shootInput <= 0f) return;
        Debug.Log("Shooting!");
        weaponMgr.FireWeapons();
    }
    #endregion

    #region Jumping
    public override void Jump()
    {
        if (readyToJump && (isGrounded || currentJumps < maxJumps))
        {
            currentJumps++;
            float adjustedJumpForce = jumpForce - rigidbody.linearVelocity.y;
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
        if (rigidbody.linearVelocity.y > 0f)
        {
            rigidbody.AddForce(Vector3.down * (rigidbody.linearVelocity.y * 0.5f), ForceMode.VelocityChange);
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
        return Vector3.ProjectOnPlane(rigidbody.linearVelocity, Vector3.up);
    }
    public float GetDeadZoneSize()
    {
        return deadZoneSize;
    }

    public bool GetCanMove()
    {
        return canMove;
    }

    public void SetCanMove(bool In)
    {
        canMove = In;
    }

    #endregion

    #region Gizmos

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        // Draw the raycast from CharacterLook()
        if (playerCamera != null)
        {
            Vector2 screenPos = new Vector2(lookInput.x * Screen.width, lookInput.y * Screen.height);
            Ray ray = playerCamera.ScreenPointToRay(screenPos);
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(ray.origin, ray.direction * 100f);

            // Draw a sphere at the targetPoint if set
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(targetPoint, 0.1f);
        }  
        // Draw a raycast showing rigidbody velocity
        if (rigidbody != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawRay(transform.position, rigidbody.linearVelocity);
        }
        // Draw the deceleration raycast
        if (isGrounded && movementDirection.x == 0 && movementDirection.z == 0)
        {
            Vector3 horizontalVel = GetHorizontalRBVelocity();
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position, -horizontalVel.normalized * 2f);
        }
    }
#endif

#endregion

    public bool GetIsGrounded()
    {
        return isGrounded;
    }
}