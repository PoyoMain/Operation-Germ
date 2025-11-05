using System;
using UnityEngine;

[Serializable]
public struct PlayerStats
{
    [Header("Layers")]
    public LayerMask PlayerLayer;
    public LayerMask CeilingLayers;
    public LayerMask GroundLayers;

    [Header("Input")]
    public bool SnapInput;
    public float VerticalDeadZoneThreshold;
    public float HorizontalDeadZoneThreshold;
    public float InputBufferTime;

    [Header("Movement")]
    public float MaxGroundSpeed;
    public float MaxAirSpeed;
    public float HorizontalAcceleration;
    public float GroundDeceleration;
    public float AirDeceleration;
    public float GroundingForce;
    public float GrounderDistance;
    public float PlatformDistance;

    [Header("Jump")]
    public float JumpPower;
    public float JumpTime;
    public float MinJumpTime;
    public float MaxFallSpeed;
    public float JumpAcceleration;
    public float FallAcceleration;
    public float JumpEndEarlyGravityModifier;
    public float CoyoteTime;
    public float JumpBuffer;

    [Header("Attacking")]
    public float TimeBetweenAttacks;
    public float SpecialAttackTime;
    public float TimeBetweenSpecialAttacks;
    public int SpecialAttackEnergyCost;

    [Header("Knockback")]
    public float SurfaceKnockback;
    public float KnockbackAppliedTime;

    [Header("Dash")]
    public float DashPower;
    public float DashTime;

    [Header("Health")]
    public int MaxHealth;
    public float InvincibleTime;
}