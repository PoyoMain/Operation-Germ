using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BossManager : MonoBehaviour
{
    public static BossManager Instance { get; private set; }

    [SerializeField] private int bossesToUnlockFinalBoss = 4;

    private Dictionary<BossPart, bool> bosses;

    public int BossesDefeated => bosses.Count(x => x.Value == true);
    public int BossesNeededForFinalBoss => bossesToUnlockFinalBoss;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    private void Start()
    {
        bosses = new();
    }

    public void GoToBossStage(BossPart bossPart)
    {
        SceneManager.LoadScene(bossPart + "Scene");
    }

    public void DefeatBoss(BossPart bossPart)
    {
        bosses.Add(bossPart, true);
        SceneManager.LoadScene("StageSelectScene");
    }

    public void ResetBosses()
    {
        bosses.Clear();
    }

    public bool IsBossDefeated(BossPart bossPart)
    {
        bosses.TryGetValue(bossPart, out bool value);
        return value;
    }
}

public enum BossPart
{
    Stomach,
    Heart,
    Lungs,
    Kidney,
    Brain
}
