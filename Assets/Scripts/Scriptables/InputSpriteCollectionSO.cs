using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "InputSpriteCollectionSO", menuName = "Scriptable Objects/InputSpriteCollectionSO")]
public class InputSpriteCollectionSO : ScriptableObject
{
    [SerializeField] private List<InputActionSprite> actionSprites;

    public Sprite GetSpriteFromInputAction(InputActionReference actionReference, InputType inputType)
    {
        InputActionSprite inputActionSprite = actionSprites.Find((x) => x.ActionReference == actionReference && x.InputType == inputType);

        if (inputActionSprite == null) return null;

        return inputActionSprite.Sprite;
    }
}
