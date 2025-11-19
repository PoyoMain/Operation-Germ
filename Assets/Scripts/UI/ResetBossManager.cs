using UnityEngine;

public class ResetBossManager : MonoBehaviour
{
    private void Start()
    {
        if (BossManager.Instance != null)
        {
            BossManager.Instance.ResetBosses();
        }
    }
}
