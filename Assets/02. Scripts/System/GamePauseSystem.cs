using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class GamePauseSystem : MonoBehaviour
{
    [SerializeField] private GameObject _pauseRoot;
    [SerializeField] private Key _toggleKey = Key.Escape;
    [SerializeField] private bool _isPaused;

    public bool IsPaused => _isPaused;

    public event Action<bool> PauseChanged;

    private void Awake()
    {
        ApplyPauseState(false);
    }

    private void Update()
    {
        if (Keyboard.current == null)
            return;

        KeyControl key = Keyboard.current[_toggleKey];

        if (key == null || !key.wasPressedThisFrame)
            return;

        SetPaused(!_isPaused);
    }

    public void SetPaused(bool isPaused)
    {
        if (_isPaused == isPaused)
            return;

        ApplyPauseState(isPaused);
    }

    public void TogglePause()
    {
        SetPaused(!_isPaused);
    }

    private void ApplyPauseState(bool isPaused)
    {
        _isPaused = isPaused;
        Time.timeScale = _isPaused ? 0f : 1f;

        if (_pauseRoot != null)
            _pauseRoot.SetActive(_isPaused);

        PauseChanged?.Invoke(_isPaused);
    }
}
