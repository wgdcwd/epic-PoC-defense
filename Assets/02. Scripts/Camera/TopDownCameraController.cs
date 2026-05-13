using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Camera))]
public class TopDownCameraController : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 8f;
    [SerializeField] private float _focusMoveSpeed = 16f;
    [SerializeField] private float _zoomSpeed = 2f;
    [SerializeField] private float _minOrthographicSize = 3f;
    [SerializeField] private float _maxOrthographicSize = 12f;
    [SerializeField] private bool _useBounds = true;
    [SerializeField] private Vector2 _minBounds = new Vector2(-20f, -12f);
    [SerializeField] private Vector2 _maxBounds = new Vector2(20f, 12f);
    [SerializeField] private HeroSelectionSystem _selectionSystem;

    private Camera _camera;
    private Vector3 _focusTargetPosition;
    private bool _hasFocusTarget;

    private void Awake()
    {
        _camera = GetComponent<Camera>();

        if (_selectionSystem == null)
            _selectionSystem = FindFirstObjectByType<HeroSelectionSystem>();
    }

    private void Update()
    {
        HandleZoomInput();
        HandleFocusInput();
        HandleKeyboardMoveInput();
        MoveToFocusTarget();
        ClampToBounds();
    }

    private void HandleKeyboardMoveInput()
    {
        if (Keyboard.current == null)
            return;

        Vector2 input = ReadMoveInput();

        if (input.sqrMagnitude <= 0f)
            return;

        Vector3 movement = new Vector3(input.x, input.y, 0f);
        transform.position += movement.normalized * _moveSpeed * Time.deltaTime;
        _hasFocusTarget = false;
    }

    private void HandleFocusInput()
    {
        if (Keyboard.current == null)
            return;

        if (!Keyboard.current.spaceKey.wasPressedThisFrame)
            return;

        if (_selectionSystem == null || !_selectionSystem.HasSelection)
            return;

        Vector2 selectionCenter = _selectionSystem.GetSelectionCenter();
        _focusTargetPosition = new Vector3(selectionCenter.x, selectionCenter.y, transform.position.z);
        _hasFocusTarget = true;
    }

    private void MoveToFocusTarget()
    {
        if (!_hasFocusTarget)
            return;

        transform.position = Vector3.MoveTowards(
            transform.position,
            _focusTargetPosition,
            _focusMoveSpeed * Time.deltaTime);

        if ((transform.position - _focusTargetPosition).sqrMagnitude <= 0.001f)
            _hasFocusTarget = false;
    }

    private void HandleZoomInput()
    {
        if (Mouse.current == null || _camera == null)
            return;

        Vector2 scroll = Mouse.current.scroll.ReadValue();

        if (Mathf.Approximately(scroll.y, 0f))
            return;

        if (_camera.orthographic)
        {
            float nextSize = _camera.orthographicSize - scroll.y * _zoomSpeed * 0.01f;
            _camera.orthographicSize = Mathf.Clamp(nextSize, _minOrthographicSize, _maxOrthographicSize);
            ClampToBounds();
            return;
        }

        float nextFieldOfView = _camera.fieldOfView - scroll.y * _zoomSpeed * 0.02f;
        _camera.fieldOfView = Mathf.Clamp(nextFieldOfView, 20f, 80f);
    }

    private Vector2 ReadMoveInput()
    {
        Vector2 input = Vector2.zero;

        if (Keyboard.current.wKey.isPressed)
            input.y += 1f;

        if (Keyboard.current.sKey.isPressed)
            input.y -= 1f;

        if (Keyboard.current.dKey.isPressed)
            input.x += 1f;

        if (Keyboard.current.aKey.isPressed)
            input.x -= 1f;

        return input;
    }

    private void ClampToBounds()
    {
        if (!_useBounds)
            return;

        Vector3 position = transform.position;

        if (_camera != null && _camera.orthographic)
        {
            float halfHeight = _camera.orthographicSize;
            float halfWidth = halfHeight * _camera.aspect;
            float minX = _minBounds.x + halfWidth;
            float maxX = _maxBounds.x - halfWidth;
            float minY = _minBounds.y + halfHeight;
            float maxY = _maxBounds.y - halfHeight;

            position.x = ClampAxis(position.x, minX, maxX);
            position.y = ClampAxis(position.y, minY, maxY);
        }
        else
        {
            position.x = ClampAxis(position.x, _minBounds.x, _maxBounds.x);
            position.y = ClampAxis(position.y, _minBounds.y, _maxBounds.y);
        }

        transform.position = position;
    }

    private float ClampAxis(float value, float min, float max)
    {
        if (min > max)
            return (min + max) * 0.5f;

        return Mathf.Clamp(value, min, max);
    }
}
