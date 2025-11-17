using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Slider healthBar;

    [Header("Listen Events")]
    [SerializeField] private FloatEventSO playerDamagedEventSO;
    [SerializeField] private VoidEventSO playerDeathEventSO;

    private void OnEnable()
    {
        playerDamagedEventSO.OnEventRaised += SetHealthBarValue;
        playerDeathEventSO.OnEventRaised += GoToLossScene;
    }

    private void OnDisable()
    {
        playerDamagedEventSO.OnEventRaised -= SetHealthBarValue;
        playerDeathEventSO.OnEventRaised -= GoToLossScene;
    }

    private void SetHealthBarValue(float value)
    {
        healthBar.value = value;
    }

    private void GoToLossScene()
    {
        SceneManager.LoadScene("LossScene");
    }
}
