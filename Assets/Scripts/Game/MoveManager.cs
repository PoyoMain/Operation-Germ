using UnityEngine;

public class MoveManager : MonoBehaviour
{
    public static MoveManager Instance { get; private set; }

    [SerializeField] private bool destroyOtherInstance = false;

    private (ProjectileSO, int) heldAttackOne = new();
    private (ProjectileSO, int) heldAttackTwo = new();

    public (ProjectileSO, int) HeldAttackOne { get => heldAttackOne; set => heldAttackOne = value; }
    public (ProjectileSO, int) HeldAttackTwo { get => heldAttackTwo; set => heldAttackTwo = value; }

    private void Awake()
    {
        if (destroyOtherInstance)
        {
            if (Instance != null) Destroy(Instance.gameObject);
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            if (Instance == null)
            {
                //destroyOtherInstance = true;
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else Destroy(gameObject);
        }
    }

    private void SetAttackOneProjectile(ProjectileSO projectile) => heldAttackOne.Item1 = projectile;
    private void SetAttackOneAmount(int amount) => heldAttackOne.Item2 = amount;
    private void SetAttackTwoProjectile(ProjectileSO projectile) => heldAttackTwo.Item1 = projectile;
    private void SetAttackTwoAmount(int amount) => heldAttackTwo.Item2 = amount;
}
