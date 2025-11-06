using System;
using UnityEngine;

[Serializable]
public struct PlayerAttackStats
{
    [Header("Input")]
    public bool SnapInput;
    public float VerticalDeadZoneThreshold;
    public float HorizontalDeadZoneThreshold;
    public float InputBufferTime;

    [Header("Attacking")]
    public float TimeBetweenAttacks;
    public float TimeBetweenParries;
}
