using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Events;

public class CharacterMovement : BaseMovement
{
    #region Variables
    [Header("Walking")]
    [SerializeField] private bool canMove = true;
    public bool isStartingUp;
    [SerializeField] private float accelerationRate = 60f;
    [SerializeField] private float decelerationRate = 30f;
    [SerializeField] private float maxWalkSpeed = 4f;
    [SerializeField] private bool weirdRotate = false;
    [SerializeField] private bool limitingMotion = true;
    [SerializeField][Range(0.01f, 1)] private float limitVelocityStrength = 0.1f;

    [Header("Jumping")]
    [SerializeField] private float maxVerticalSpeed = 25f;
    [SerializeField] private int maxJumps = 1;
    private int currentJumps = 0;
    [SerializeField] private float jumpCoolDown = 0.25f;
    [SerializeField] private float airControlMultiplier = 0.4f;
    private bool canJump = true;

    [Header("Dashing")]
    [SerializeField] private bool canDash = true;
    [SerializeField] private float dashSpeed = 50f;
    [SerializeField][Range(0.1f, 10)] private float dashCooldown = 2;
    [SerializeField][Range(0.1f, 10)] private float dashRecharge = 2;
    [SerializeField][Range(0.1f, 10)] private float dashLength = 0.2f;
    [SerializeField][Range(1,5)] private int maxDashes = 2;
    private int currentDashes = 0;
    private bool currentlyRechargingDash = false;
    private bool currentlyDashing = false;

    [Header("Rotation")]
    [SerializeField][Range(0,10)] private float horizontalRotationRate = 2;
    [SerializeField][Range(0,10)] private float verticalRotationRate = 2;
    [SerializeField][Range(0,1)] private float lookClampMin = 0.25f;
    [SerializeField][Range(0,1)] private float lookClampMax = 0.75f;
    [SerializeField][Range(0,0.5f)] private float deadZoneSize = 0.02f;

    [Header("Ground Check")]
    [SerializeField] private float groundCheckDistance = 0.1f;
    [SerializeField] private LayerMask environmentLayerMask;
    private bool wasGroundedLastFrame = false;
    [SerializeField] private bool isGrounded = false;

    [Header("Shooting")]
    [SerializeField] private TeamWeaponManager weaponMgr;

    [Header("Camera")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float cameraPitch = 0f;
    [SerializeField] private Vector3 targetPoint;
    [SerializeField] private CinemachineImpulseSource movementImpulseSource;
    [SerializeField][Range(0.01f, 3)] private float impulseRate;
    private float impulseTimer;

    [Header("Misc")]
    private bool isBeingKnockedBack = false;

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

        GameManager.Instance.OnRoundEnd.AddListener(SetCanMoveFalse);
        ShopManager.Instance.OnShopEnd.AddListener(SetCanMoveTrue);
    }

    private void FixedUpdate()
    {
        //if (!IsOwner) { return; }
        CheckIsGrounded();
        if (!canMove) { return; }
        MoveCharacter();
        CharacterLook();

        if (limitingMotion && !currentlyDashing && !isBeingKnockedBack)
            LimitVelocity(); //TODO: play with limit velocity tuning, especially for synced/unsynced movement

        if (!currentlyRechargingDash && currentDashes > 0)
        {
            currentlyRechargingDash = true;
            StartCoroutine(DashRechargeCoroutine());
        }
    }

    private void Update()
    {
        Cursor.visible = true;
        // RotateCharacter();
        if (rigidbody.linearVelocity.sqrMagnitude > 0.01f)
        {
            impulseTimer += Time.deltaTime;
            if (impulseTimer >= impulseRate)
            {
                movementImpulseSource.GenerateImpulse();
                impulseTimer = 0f;
                // movementSFXManager.PlayFootstepSound();
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
        // print(movementDirection);
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
                Vector3 camRight;
                if (weirdRotate){camRight = playerCamera.transform.right;}
                else{camRight = transform.right;}
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

    #region Limit Velocity

    /// <summary>
    /// Limits the character's horizontal velocity to the defined maximum walk speed, and vertical velocity to the defined maximum vertical speed.
    /// </summary>

    private void LimitVelocity()
    {
        Vector3 horizontalVel = GetHorizontalRBVelocity();
        // If the player is actively providing movement input (magnitude >= 1)
        if (movementDirection.magnitude >= 1)
        {
            // If the horizontal velocity exceeds the allowed maximum speed
            if (horizontalVel.magnitude > currentMaxSpeed)
            {
            // Calculate a force in the opposite direction to reduce speed to the maximum allowed
            Vector3 counteract = -horizontalVel.normalized;
            float excess = horizontalVel.magnitude - currentMaxSpeed;
            rigidbody.AddForce(counteract * excess * limitVelocityStrength, ForceMode.VelocityChange);
            }

            // Jumping logic

            if (Mathf.Abs(rigidbody.linearVelocity.y) > maxVerticalSpeed)
            {
                Vector3 counteract = Vector3.up * -Mathf.Sign(rigidbody.linearVelocity.y);
                float excessY = Mathf.Abs(rigidbody.linearVelocity.y) - maxVerticalSpeed;
                rigidbody.AddForce(counteract * excessY * limitVelocityStrength, ForceMode.VelocityChange);
            }
        }
        else
        {
            if (horizontalVel.magnitude > currentMaxSpeed * 0.5f)
            {
                Vector3 counteract = -horizontalVel.normalized;
                float excess = horizontalVel.magnitude - currentMaxSpeed * 0.5f;
                rigidbody.AddForce(counteract * excess * limitVelocityStrength, ForceMode.VelocityChange);
            }

            // Jumping logic

            if (Mathf.Abs(rigidbody.linearVelocity.y) > maxVerticalSpeed)
            {
                Vector3 counteract = Vector3.up * -Mathf.Sign(rigidbody.linearVelocity.y);
                float excessY = Mathf.Abs(rigidbody.linearVelocity.y) - maxVerticalSpeed;
                rigidbody.AddForce(counteract * excessY * limitVelocityStrength, ForceMode.VelocityChange);
            }
        }
    }
    #endregion
    #endregion

    #region Rotation
    /// <summary>
    /// Rotates the character to face the target point determined by the look input, 
    /// with rotation rates that increase as the look input moves further from the center of the screen. 
    /// Also applies vertical rotation to the camera for looking up and down.
    /// </summary>
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
            cameraPitch = Mathf.Clamp(cameraPitch, -10f, 30f);
            playerCamera.transform.localRotation = Quaternion.Euler(-cameraPitch, 0f, 0f);
        }
    }
    #endregion

    #region Shooting
    public override void Shoot(float shootInput)
    {
        //Debug.Log("Shoot method being called, canmove: " + canMove + " Shootinput: " + shootInput);
        if (!canMove) return;
        if (shootInput <= 0f) return;
        weaponMgr.FireWeapons(shootInput);
    }
    #endregion

    #region Jumping
    public override void Jump(float jumpInput)
    {
        if (!canJump) return;

        if (isGrounded)
        {
            currentJumps = 0;
        }

        if (currentJumps >= maxJumps) return;

        currentJumps++;

        float adjustedJumpForce = jumpForce - rigidbody.linearVelocity.y;
        adjustedJumpForce *= jumpInput;

        rigidbody.AddForce(Vector3.up * adjustedJumpForce, ForceMode.VelocityChange);

        canJump = false;
        StartCoroutine(JumpCooldownCoroutine());
    }

    private IEnumerator JumpCooldownCoroutine()
    {
        yield return new WaitForSeconds(jumpCoolDown);
        canJump = true;
    }

    public override void CancelJump()
    {
        if (rigidbody.linearVelocity.y > 0f)
        {
            rigidbody.linearVelocity = new Vector3(
                rigidbody.linearVelocity.x,
                rigidbody.linearVelocity.y * 0.5f,
                rigidbody.linearVelocity.z
            );
        }
    }

    /// <summary>
    /// Applies a downward force to the character to quickly return them to the ground. 
    /// Used by utilities that require the player to be grounded.
    /// </summary>
    public void ReturnToGround()
    {
        if (!isGrounded)
        {
            rigidbody.AddForce(Vector3.down * 10f, ForceMode.VelocityChange);
        }
    }

    #endregion

    #region Dashing
    public override void Dash(Vector2 dashInput)
    {
        if (dashInput.magnitude > 0)
        {
            Debug.Log("Dashing!");
            if (canDash && currentDashes < maxDashes)
            {
                currentDashes++;
                Vector3 dashDirection = (transform.right * dashInput.x + transform.forward * dashInput.y).normalized;
                Vector3 adjustedDashForce = dashDirection * dashSpeed - rigidbody.linearVelocity;
                rigidbody.AddForce(adjustedDashForce, ForceMode.VelocityChange);

                canDash = false;
                StartCoroutine(DashCooldownCoroutine());
                StartCoroutine(DashLengthCoroutine());
            }
        }
        return;
    }

    private IEnumerator DashCooldownCoroutine()
    {
        Debug.Log("Dash Cooling Down!");
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    private IEnumerator DashLengthCoroutine()
    {
        currentlyDashing = true;
        yield return new WaitForSeconds(dashLength);
        currentlyDashing = false;
    }

    private IEnumerator DashRechargeCoroutine()
    {
        Debug.Log("Dashing Rechargin!");
        yield return new WaitForSeconds(dashRecharge);
        currentDashes -= 1;
        currentlyRechargingDash = false;
    }
    #endregion

    #region Knockback
    public void ApplyKnockback(Vector3 forceVec, float knockbackForce, float duration = 1)
    {
        forceVec.Normalize();
        rigidbody.AddForce(forceVec * knockbackForce, ForceMode.Impulse);
        StartCoroutine(KnockbackCoroutine(duration));
    }

    private IEnumerator KnockbackCoroutine(float duration)
    {
        isBeingKnockedBack = true;
        yield return new WaitForSeconds(duration);
        isBeingKnockedBack = false;
    }

    public bool GetIsBeingKnockedBack()
    {
        return isBeingKnockedBack;
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
    public bool GetIsGrounded(){return isGrounded;}
    public float GetDeadZoneSize(){return deadZoneSize;}
    public bool GetCanMove(){return canMove;}
    public void SetCanMove(bool In){canMove = In;}
    private void SetCanMoveFalse(){SetCanMove(false);}
    private void SetCanMoveTrue(){SetCanMove(true);}

    #endregion

    #region Gizmos

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        // Draw the raycast from CharacterLook()
        if (playerCamera != null)
        {
            Vector2 screenPos = new(lookInput.x * Screen.width, lookInput.y * Screen.height);
            Ray ray = playerCamera.ScreenPointToRay(screenPos);
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(ray.origin, targetPoint - ray.origin);

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
}