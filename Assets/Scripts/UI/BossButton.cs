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
        if (BossManager.Instance.BossesDefeated < BossManager.Instance.BossesNeededForFinalBoss)
        {
            if (bossPart == BossPart.Brain) gameObject.SetActive(false);
            else if (BossManager.Instance.IsBossDefeated(bossPart)) button.interactable = false;
            else button.interactable = true;
        }
        else
        {
            if (bossPart == BossPart.Brain) button.interactable = true;
            else gameObject.SetActive(false);
        }        
    }

    private void GoToScene()
    {
        BossManager.Instance.GoToBossStage(bossPart);
    }
}
