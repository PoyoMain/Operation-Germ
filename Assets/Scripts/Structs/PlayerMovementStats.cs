using System;
using UnityEngine;

[Serializable]
public struct PlayerMovementStats
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

    [Header("Dash")]
    public float DashPower;
    public float DashTime;
}