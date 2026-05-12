using UnityEngine;
using UnityEngine.UI;

public class SelectionDragView : MonoBehaviour
{
    [SerializeField] private RectTransform _root;
    [SerializeField] private Image _fillImage;
    [SerializeField] private Image _borderTop;
    [SerializeField] private Image _borderBottom;
    [SerializeField] private Image _borderLeft;
    [SerializeField] private Image _borderRight;
    [SerializeField] private float _borderThickness = 2f;
    [SerializeField] private Color _fillColor = new Color(0.2f, 0.65f, 1f, 0.15f);
    [SerializeField] private Color _borderColor = new Color(0.2f, 0.65f, 1f, 0.9f);

    private RectTransform _canvasRectTransform;
    private Canvas _canvas;

    private void Awake()
    {
        if (_root == null)
            _root = GetComponent<RectTransform>();

        _canvas = GetComponentInParent<Canvas>();

        if (_canvas != null)
            _canvasRectTransform = _canvas.GetComponent<RectTransform>();

        ApplyStyle();
        Hide();
    }

    public void Show(Vector2 startScreenPosition, Vector2 currentScreenPosition)
    {
        if (_root == null || _canvasRectTransform == null)
            return;

        Rect screenRect = GetScreenRect(startScreenPosition, currentScreenPosition);
        Vector2 centerScreenPosition = screenRect.center;

        Camera canvasCamera = GetCanvasCamera();

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _canvasRectTransform,
            centerScreenPosition,
            canvasCamera,
            out Vector2 localCenterPosition);

        _root.gameObject.SetActive(true);
        _root.anchoredPosition = localCenterPosition;
        _root.sizeDelta = screenRect.size;

        UpdateBorder(screenRect.size);
    }

    public void Hide()
    {
        if (_root != null)
            _root.gameObject.SetActive(false);
    }

    private void UpdateBorder(Vector2 size)
    {
        if (_fillImage != null)
            _fillImage.rectTransform.sizeDelta = size;

        SetHorizontalBorder(_borderTop, size.y * 0.5f);
        SetHorizontalBorder(_borderBottom, size.y * -0.5f);
        SetVerticalBorder(_borderLeft, size.x * -0.5f);
        SetVerticalBorder(_borderRight, size.x * 0.5f);
    }

    private void SetHorizontalBorder(Image borderImage, float yPosition)
    {
        if (borderImage == null)
            return;

        RectTransform rectTransform = borderImage.rectTransform;
        rectTransform.anchoredPosition = new Vector2(0f, yPosition);
        rectTransform.sizeDelta = new Vector2(_root.sizeDelta.x, _borderThickness);
    }

    private void SetVerticalBorder(Image borderImage, float xPosition)
    {
        if (borderImage == null)
            return;

        RectTransform rectTransform = borderImage.rectTransform;
        rectTransform.anchoredPosition = new Vector2(xPosition, 0f);
        rectTransform.sizeDelta = new Vector2(_borderThickness, _root.sizeDelta.y);
    }

    private Rect GetScreenRect(Vector2 startScreenPosition, Vector2 currentScreenPosition)
    {
        Vector2 bottomLeft = Vector2.Min(startScreenPosition, currentScreenPosition);
        Vector2 topRight = Vector2.Max(startScreenPosition, currentScreenPosition);

        return Rect.MinMaxRect(bottomLeft.x, bottomLeft.y, topRight.x, topRight.y);
    }

    private Camera GetCanvasCamera()
    {
        if (_canvas == null || _canvas.renderMode == RenderMode.ScreenSpaceOverlay)
            return null;

        return _canvas.worldCamera;
    }

    private void ApplyStyle()
    {
        if (_fillImage != null)
        {
            _fillImage.color = _fillColor;
            _fillImage.raycastTarget = false;
        }

        ApplyBorderStyle(_borderTop);
        ApplyBorderStyle(_borderBottom);
        ApplyBorderStyle(_borderLeft);
        ApplyBorderStyle(_borderRight);
    }

    private void ApplyBorderStyle(Image borderImage)
    {
        if (borderImage == null)
            return;

        borderImage.color = _borderColor;
        borderImage.raycastTarget = false;
    }
}
