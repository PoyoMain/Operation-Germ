using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class HeldMoveIcon : MonoBehaviour
{
    [Header("Image Components")]
    [SerializeField] private Image iconImageBG;
    [SerializeField] private Image iconImageFill;

    [Header("No Projectile Sprite")]
    [SerializeField] private Sprite emptySprite;

    [Header("Listen Events")]
    [SerializeField] private ProjectileEventSO projectileRegisteredEventSO;
    [SerializeField] private FloatEventSO projectilePercentageUpdatedEventSO;
    [SerializeField] private VoidEventSO projectileReleasedEventSO;

    private Image image;
    private ProjectileSO heldMove;

    private bool IsFilled => iconImageFill.fillAmount >= 1;

    private void Awake()
    {
        TryGetComponent(out image);
    }

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
        heldMove = projectile;

        SetIconVisibility(true);
        SetIconSprite(projectile.IconSpriteEnemy);
    }

    private void ReleaseProjectile()
    {
        SetIconVisibility(false);
        image.sprite = emptySprite;
    }

    private void SetIconVisibility(bool value)
    {
        iconImageBG.enabled = iconImageFill.enabled = value;
    }

    private void SetIconSprite(Sprite newSprite)
    {
        iconImageBG.sprite = iconImageFill.sprite = newSprite;
    }

    private void SetFillAmount(float amount)
    {
        iconImageFill.fillAmount = amount;

        if (IsFilled)
        {
            SetIconSprite(heldMove.IconSpritePlayer);
            image.sprite = heldMove.IconEnemyBorder;
        }
    }
}
