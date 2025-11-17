using UnityEngine;

public class PlayerStateManager : MonoBehaviour
{
    private bool isMoving;
    private bool isGrounded;
    private bool isAttacking;
    private bool isParrying;
    private bool isCharging;

    public bool IsMoving { get => isMoving; }
    public bool IsGrounded { get => isGrounded; }
    public bool IsAttacking { get => isAttacking; }
    public bool IsParrying { get => isParrying; }
    public bool IsCharging { get => isCharging; }

    public void SetMoving(bool value) => isMoving = value;
    public void SetGrounded(bool value) => isGrounded = value;
    public void SetAttacking(bool value) => isAttacking = value;
    public void SetParrying(bool value) => isParrying = value;
    public void SetCharging(bool value) => isCharging = value;
}
