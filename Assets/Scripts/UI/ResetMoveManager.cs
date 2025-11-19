using UnityEngine;

public class ResetMoveManager : MonoBehaviour
{
    [SerializeField] private bool resetAnyway;

    private void Start()
    {
        if (MoveManager.Instance != null)
        {
            if (resetAnyway)
            {
                Destroy(MoveManager.Instance.gameObject);
                return;
            }

            if (BossManager.Instance != null && BossManager.Instance.BossesDefeated == BossManager.Instance.BossesNeededForFinalBoss) return;

            Destroy(MoveManager.Instance.gameObject);
        }
    }
}
