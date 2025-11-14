using System;
using UnityEngine;
using UnityEngine.InputSystem;

[Serializable]
public class InputActionSprite
{
    [SerializeField] private InputActionReference actionReference;
    [SerializeField] private Sprite sprite;
    [SerializeField] private InputType inputType;

    public InputActionReference ActionReference => actionReference;
    public Sprite Sprite => sprite;
    public InputType InputType => inputType;
}

public enum InputType
{
    Keyboard,
    Controller
}