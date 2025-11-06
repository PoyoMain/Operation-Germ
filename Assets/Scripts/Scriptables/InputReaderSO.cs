using UnityEngine;

[CreateAssetMenu(fileName = "InputReaderSO", menuName = "Scriptable Objects/InputReaderSO")]
public class InputReaderSO : ScriptableObject
{
    private PlayerControls _playerControls;
    public PlayerControls.GameplayActions Controls { get; private set; }

    private void OnEnable()
    {
        _playerControls = new();
        Controls = _playerControls.Gameplay;
        Controls.Enable();
    }

    private void OnDisable()
    {
        Controls.Disable();
    }
}
