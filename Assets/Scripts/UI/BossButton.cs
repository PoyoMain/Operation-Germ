using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class BossButton : MonoBehaviour
{
    [SerializeField] private BossPart bossPart;
    private Button button;

    private void Awake()
    {
        TryGetComponent(out button);
    }

    private void OnEnable()
    {
        button.onClick.AddListener(GoToScene);
    }

    private void OnDisable()
    {
        button.onClick.RemoveListener(GoToScene);
    }

    private void Start()
    {
        if (bossPart == BossPart.Brain && BossManager.Instance.BossesDefeated < BossManager.Instance.BossesNeededForFinalBoss) button.interactable = false;
        if (BossManager.Instance.IsBossDefeated(bossPart)) button.interactable = false;
    }

    private void GoToScene()
    {
        BossManager.Instance.GoToBossStage(bossPart);
    }
}
