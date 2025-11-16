using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class HeldMoveIcon : MonoBehaviour
{
    [Header("General Settings")]
    [SerializeField] private bool attackOne;

    [Header("Image Components")]
    [SerializeField] private Image iconImageBG;
    [SerializeField] private Image iconImageFill;
    [SerializeField] private TextMeshProUGUI moveNameTextBox;

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
        if (attackOne && MoveManager.Instance.HeldAttackOne.Item1 != null)
        {
            (ProjectileSO, int) move = MoveManager.Instance.HeldAttackOne;

            RegisterProjectile(move.Item1);
            SetFillAmount((float)move.Item2 / move.Item1.IdentifyAmount);
        }
        else if (!attackOne && MoveManager.Instance.HeldAttackTwo.Item1 != null)
        {
            (ProjectileSO, int) move = MoveManager.Instance.HeldAttackTwo;

            RegisterProjectile(move.Item1);
            SetFillAmount((float)move.Item2 / move.Item1.IdentifyAmount);
        }
        else
        {
            ReleaseProjectile();
        } 
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
        moveNameTextBox.text = "Parry";
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
            moveNameTextBox.text = heldMove.name;
        }
    }
}
