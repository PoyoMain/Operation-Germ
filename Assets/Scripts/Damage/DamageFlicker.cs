using System.Collections;
using UnityEngine;

public class DamageFlicker : MonoBehaviour
{
    [Header("Colors")]
    [SerializeField] private Color regularColor;
    [SerializeField] private Color flashColor;

    [Header("Times")]
    [SerializeField] private float timeBetweenFlashes;

    private bool isFlickering;
    private SpriteRenderer[] spriteRenderers;
    private Coroutine damageFlickerCoroutine;

    private void Awake()
    {
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
    }

    public void StartDamageFlicker()
    {
        if (damageFlickerCoroutine != null) StopCoroutine(damageFlickerCoroutine);
        damageFlickerCoroutine = StartCoroutine(DamageFlickerer());
    }

    public void EndDamageFlicker()
    {
        isFlickering = false;
    }

    private IEnumerator DamageFlickerer()
    {
        isFlickering = true;

        while (isFlickering)
        {
            SetSpriteRendererColors(flashColor);
            yield return new WaitForSeconds(timeBetweenFlashes);
            SetSpriteRendererColors(regularColor);
            yield return new WaitForSeconds(timeBetweenFlashes);
            yield return null;
        }
    }

    private void SetSpriteRendererColors(Color newColor)
    {
        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            spriteRenderers[i].color = newColor;
        }
    }
} 
