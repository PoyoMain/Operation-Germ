using UnityEngine;
using UnityEngine.UI;

public class HeldMoveIcon : MonoBehaviour
{
    [Header("Image Components")]
    [SerializeField] private Image iconImageBG;
    [SerializeField] private Image iconImageFill;

    [Header("Listen Events")]
    [SerializeField] private ProjectileEventSO projectileRegisteredEventSO;
    [SerializeField] private FloatEventSO projectilePercentageUpdatedEventSO;
    [SerializeField] private VoidEventSO projectileReleasedEventSO;

    private void OnEnable()
    {
        projectileRegisteredEventSO.OnEventRaised += RegisterProjectile;
        projectilePercentageUpdatedEventSO.OnEventRaised += SetFillAmount;
        projectileReleasedEventSO.OnEventRaised += ReleaseProjectile;
    }

    private void OnDisable()
    {
        projectileRegisteredEventSO.OnEventRaised -= RegisterProjectile;
        projectilePercentageUpdatedEventSO.OnEventRaised -= SetFillAmount;
        projectileReleasedEventSO.OnEventRaised -= ReleaseProjectile;
    }

    private void Start()
    {
        ReleaseProjectile();
    }

    private void RegisterProjectile(ProjectileSO projectile)
    {
        SetIconVisibility(true);
        SetIconSprite(projectile.IconSprite);
        SetFillAmount(1 / projectile.IdentifyAmount);
    }

    private void ReleaseProjectile()
    {
        SetIconVisibility(false);
    }

    private void SetIconVisibility(bool value)
    {
        iconImageBG.enabled = iconImageFill.enabled = value;
    }

    private void SetIconSprite(Sprite sprite)
    {
        iconImageBG.sprite = iconImageFill.sprite = sprite;
    }

    private void SetFillAmount(float amount)
    {
        iconImageFill.fillAmount = amount;
    }
}
