using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ButtonPrompt : MonoBehaviour
{
    [SerializeField] private InputSpriteCollectionSO spriteCollectionSO;
    [Space(10)]
    [SerializeField] private InputActionReference actionReference;
    [SerializeField] private Image promptImage;

    private void OnEnable()
    {
        ReplaceImage();
        InputSystem.onDeviceChange += ReplaceImage;
    }

    private void OnDisable()
    {
        InputSystem.onDeviceChange -= ReplaceImage;
    }

    private void ReplaceImage(InputDevice inputDevice, InputDeviceChange inputDeviceChange)
    {
        if (inputDevice is not Gamepad) return;

        ReplaceImage();
    }

    private void ReplaceImage()
    {
        InputType inputType = Gamepad.current == null ? InputType.Keyboard : InputType.Controller;
        Sprite newSprite = spriteCollectionSO.GetSpriteFromInputAction(actionReference, inputType);

        if (newSprite != null) promptImage.sprite = newSprite;
    }
}
