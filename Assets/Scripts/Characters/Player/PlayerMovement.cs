using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Scriptable Object")]
    [SerializeField] private InputReaderSO inputReader;

    [Header("Components")]
    [SerializeField] private BoxCollider2D coll;

    [Header("Stats")]
    [SerializeField] private PlayerMovementStats stats;

    // Constants
    private const int ROTATION_FACING_RIGHT = 0;
    private const int ROTATION_FACING_LEFT = 180;
    private const string ANIM_PARAM_MOVING = "IsMoving";
    private const string ANIM_PARAM_GROUNDED = "IsGrounded";
    private const string ANIM_PARAM_KNOCKBACK = "InKnockback";
    private const string ANIM_PARAM_VERTICAL_VELOCITY = "VerticalVelocity";


    // Private Veriables
    private Rigidbody2D rb;
    private Animator anim;
    private PlayerAttack playerAttack;
    private Vector2 moveInput;
    private Vector2 velocity;
    private bool jumpDown;
    private bool jumpHeld;
    private bool dashDown;
    private bool cachedQueryStartInColliders;
    private float time;

    private PlayerControls.GameplayActions Controls => inputReader.Controls;

    private void Awake()
    {
        TryGetComponent(out rb);
        TryGetComponent(out anim);
        TryGetComponent(out playerAttack);

        cachedQueryStartInColliders = Physics2D.queriesStartInColliders;
    }

    private void Update()
    {
        time += Time.deltaTime;
        GetInput();
    }

    private void GetInput()
    {
        moveInput = Controls.Movement.ReadValue<Vector2>();
        if (stats.SnapInput)
        {
            moveInput.x = Mathf.Abs(moveInput.x) < stats.HorizontalDeadZoneThreshold ? 0 : Mathf.Sign(moveInput.x);
            moveInput.y = Mathf.Abs(moveInput.y) < stats.VerticalDeadZoneThreshold ? 0 : Mathf.Sign(moveInput.y);
        }

        jumpDown = Controls.Jump.WasPressedThisFrame();
        jumpHeld = Controls.Jump.IsPressed();
        if (jumpDown)
        {
            jumpToConsume = true;
            timeJumpWasPressed = time;
        }

        if (!dashDown)
        {
            dashDown = Controls.Dash.WasPressedThisFrame();
            dashInputTimer = stats.InputBufferTime;
        }

    }

    private void FixedUpdate()
    {
        CheckCollisions();

        HandleDash();
        HandleKnockback();
        HandleJump();
        HandleDirection();
        HandleGravity();
        HandleAnimation();

        ApplyVelocity();
    }

    #region Dash
    private float dashInputTimer;
    private float dashTimer;
    private readonly float dashDecelerationPercentage = 0.2f;
    private bool hasDashInAir;
    public bool IsDashing => dashTimer > 0;

    private void HandleDash()
    {
        if (dashInputTimer > 0)
        {
            dashInputTimer -= Time.deltaTime;
            if (dashInputTimer <= 0) dashDown = false;
        }


        if (IsDashing)
        {
            dashTimer -= Time.fixedDeltaTime;

            if (dashTimer <= stats.DashTime * dashDecelerationPercentage) velocity.x = Mathf.MoveTowards(velocity.x, 0, stats.GroundDeceleration * Time.fixedDeltaTime);

            if (dashTimer <= 0) anim.SetBool("IsDashing", false);
        }

        if (!dashDown) return;

        if (!IsGrounded && !hasDashInAir) return;

        if (playerAttack.IsParrying) return;

        if (dashDown && !IsDashing) ExecuteDash();

        dashDown = false;
        dashInputTimer = 0;
    }

    private void ExecuteDash()
    {
        int directionInteger = (moveInput == Vector2.right) ? 1 : 
            (moveInput == Vector2.left) ? -1 : 
            (transform.eulerAngles == new Vector3(0, ROTATION_FACING_RIGHT, 0) || transform.eulerAngles == new Vector3(0, -ROTATION_FACING_RIGHT, 0)) ? 1 : -1;
        velocity = new(stats.DashPower * directionInteger, 0);
        dashTimer = stats.DashTime;
        anim.SetBool("IsDashing", true);
        if (!IsGrounded) hasDashInAir = false;

        // Rotate GameObject based on movement input
        if (moveInput.x > 0 && transform.localEulerAngles.y != ROTATION_FACING_RIGHT)
        {
            Vector3 newRot = new(0, ROTATION_FACING_RIGHT, 0);
            transform.localEulerAngles = newRot;
        }
        else if (moveInput.x < 0 && transform.localEulerAngles.y != ROTATION_FACING_LEFT)
        {
            Vector3 newRot = new(0, ROTATION_FACING_LEFT, 0);
            transform.localEulerAngles = newRot;
        }
    }
    #endregion

    #region Collision
    private float frameLeftGround = float.MinValue;
    public bool IsGrounded { get; private set; }

    private void CheckCollisions()
    {
        Physics2D.queriesStartInColliders = false;

        RaycastHit2D groundHit = Physics2D.BoxCast(coll.bounds.center, coll.size, 0, Vector2.down, stats.GrounderDistance, stats.GroundLayers);
        bool ceilingHit = Physics2D.BoxCast(coll.bounds.center, coll.size, 0, Vector2.up, stats.GrounderDistance, stats.CeilingLayers);

        // Hit a Ceiling
        if (ceilingHit)
        {
            velocity.y = Mathf.Min(0, velocity.y);
        }

        // Landed on Ground
        if (!IsGrounded && groundHit)
        {
            IsGrounded = true;
            coyoteUsable = true;
            bufferedJumpUsable = true;
            endedJumpEarly = false;
            hasDashInAir = true;
        }

        //Left the Ground
        else if (IsGrounded && !groundHit)
        {
            IsGrounded = false;
            frameLeftGround = time;
        }

        Physics2D.queriesStartInColliders = cachedQueryStartInColliders;
    }
    #endregion

    #region Jump & Crouch

    private bool jumpToConsume;
    private bool bufferedJumpUsable;
    private bool endedJumpEarly;
    private bool coyoteUsable;
    private float jumpTimer;
    private float timeJumpWasPressed;

    private bool HasBufferedJump => bufferedJumpUsable && time < timeJumpWasPressed + stats.JumpBuffer;
    private bool CanUseCoyote => coyoteUsable && !IsGrounded && time < frameLeftGround + stats.CoyoteTime;

    private void HandleJump()
    {
        if (IsInKnockback) return;

        if (!endedJumpEarly && !IsGrounded && !jumpHeld && rb.linearVelocity.y >= 0)
        {
            endedJumpEarly = true;
        }

        if (!endedJumpEarly && IsDashing)
        {
            endedJumpEarly = true;
            jumpTimer = 0;
        }

        if ((jumpHeld || endedJumpEarly)) HandleJumpMovement();

        if (!jumpToConsume && !HasBufferedJump) return;

        if (playerAttack.IsParrying) return;

        if (IsGrounded || CanUseCoyote) ExecuteJump();

        jumpToConsume = false;
    }

    private void ExecuteJump()
    {
        endedJumpEarly = false;
        timeJumpWasPressed = 0;
        bufferedJumpUsable = false;
        coyoteUsable = false;
        jumpTimer = stats.JumpTime;
        velocity.y = 0;
    }

    private void HandleJumpMovement()
    {
        if (endedJumpEarly && jumpTimer < stats.JumpTime - stats.MinJumpTime)
        {
            return;
        }

        if (jumpTimer > 0)
        {
            jumpTimer -= Time.fixedDeltaTime;
            if (velocity.y < stats.JumpPower) velocity.y = Mathf.MoveTowards(velocity.y, stats.JumpPower, stats.JumpAcceleration * Time.fixedDeltaTime);
        }

    }
    #endregion

    #region Horizontal
    public bool IsMoving { get => moveInput.x != 0; private set => IsMoving = value; }

    private void HandleDirection()
    {
        if (IsInKnockback || IsDashing) return;

        if (IsGrounded && (playerAttack.IsCharging || playerAttack.IsParrying || playerAttack.IsAttacking))
        {
            var deceleration = IsGrounded ? stats.GroundActionDeceleration : stats.AirActionDeceleration;
            velocity.x = Mathf.MoveTowards(velocity.x, 0, deceleration * Time.fixedDeltaTime);
            return;
        }

        if (moveInput.x == 0)
        {
            var deceleration = IsGrounded ? stats.GroundDeceleration : stats.AirDeceleration;
            velocity.x = Mathf.MoveTowards(velocity.x, 0, deceleration * Time.fixedDeltaTime);
        }
        else
        {
            var maxSpeed = IsGrounded ? stats.MaxGroundSpeed : stats.MaxAirSpeed;
            velocity.x = Mathf.MoveTowards(velocity.x, moveInput.x * maxSpeed, stats.HorizontalAcceleration * Time.fixedDeltaTime);
        }

        if (playerAttack.IsCharging || playerAttack.IsParrying || playerAttack.IsAttacking) return;

        // Rotate GameObject based on movement input
        if (moveInput.x > 0 && transform.localEulerAngles.y != ROTATION_FACING_RIGHT)
        {
            Vector3 newRot = new(0, ROTATION_FACING_RIGHT, 0);
            transform.localEulerAngles = newRot;
        }
        else if (moveInput.x < 0 && transform.localEulerAngles.y != ROTATION_FACING_LEFT)
        {
            Vector3 newRot = new(0, ROTATION_FACING_LEFT, 0);
            transform.localEulerAngles = newRot;
        }
    }
    #endregion

    #region Gravity

    private void HandleGravity()
    {
        if (IsDashing) return;

        if (IsGrounded && velocity.y <= 0f)
        {
            velocity.y = stats.GroundingForce;
        }
        else
        {
            float inAirGravity = stats.FallAcceleration;
            if (endedJumpEarly && velocity.y > 0 && jumpTimer < stats.JumpTime - stats.MinJumpTime)
            {
                inAirGravity *= stats.JumpEndEarlyGravityModifier;
            }
            velocity.y = Mathf.MoveTowards(velocity.y, -stats.MaxFallSpeed, inAirGravity * Time.fixedDeltaTime);
        }
    }

    #endregion

    #region Animation
    private void HandleAnimation()
    {
        anim.SetBool(ANIM_PARAM_KNOCKBACK, IsInKnockback);
        anim.SetBool(ANIM_PARAM_MOVING, moveInput.x != 0);
        anim.SetBool(ANIM_PARAM_GROUNDED, IsGrounded);
        
        if (IsGrounded && velocity.y <= 0) anim.SetInteger(ANIM_PARAM_VERTICAL_VELOCITY, 0);
        else if (velocity.y > 0) anim.SetInteger(ANIM_PARAM_VERTICAL_VELOCITY, 1);
        else if (velocity.y < 0) anim.SetInteger(ANIM_PARAM_VERTICAL_VELOCITY, -1);
    }
    #endregion

    #region Knockback
    private float knockbackTimer;
    public bool IsInKnockback => knockbackTimer > 0;

    private void HandleKnockback()
    {
        if (knockbackTimer > 0) knockbackTimer -= Time.fixedDeltaTime;
    }

    public void ApplyKnockback()
    {
        velocity = (-transform.right + transform.up) * stats.KnockbackPower;
        knockbackTimer = stats.KnockbackTime;
    }
    #endregion

    private void ApplyVelocity() => rb.linearVelocity = velocity;
}

