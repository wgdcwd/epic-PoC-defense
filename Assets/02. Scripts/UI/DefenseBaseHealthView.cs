using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DefenseBaseHealthView : MonoBehaviour
{
    [SerializeField] private GameObject _root;
    [SerializeField] private TMP_Text _labelText;
    [SerializeField] private Slider _healthSlider;

    private void Awake()
    {
        EnsureRoot();
    }

    public void Show()
    {
        EnsureRoot();
        _root.SetActive(true);
    }

    public void Hide()
    {
        EnsureRoot();
        _root.SetActive(false);
    }

    public void SetHealth(float currentHealth, float maxHealth)
    {
        float safeMaxHealth = Mathf.Max(1f, maxHealth);
        float safeCurrentHealth = Mathf.Clamp(currentHealth, 0f, safeMaxHealth);

        if (_healthSlider != null)
        {
            _healthSlider.minValue = 0f;
            _healthSlider.maxValue = safeMaxHealth;
            _healthSlider.value = safeCurrentHealth;
        }

        if (_labelText != null)
        {
            int current = Mathf.CeilToInt(safeCurrentHealth);
            int max = Mathf.CeilToInt(safeMaxHealth);
            _labelText.text = "Base HP " + current + " / " + max;
        }
    }

    public static DefenseBaseHealthView CreateDefault()
    {
        Canvas canvas = CreateCanvas();
        GameObject root = CreateRoot(canvas.transform);
        DefenseBaseHealthView view = root.AddComponent<DefenseBaseHealthView>();
        TMP_Text labelText = CreateLabel(root.transform);
        Slider slider = CreateSlider(root.transform);

        view._root = root;
        view._labelText = labelText;
        view._healthSlider = slider;
        return view;
    }

    private void EnsureRoot()
    {
        if (_root == null)
            _root = gameObject;
    }

    private static Canvas CreateCanvas()
    {
        GameObject canvasObject = new GameObject("DefenseBaseHealthCanvas");
        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 80;
        canvasObject.AddComponent<CanvasScaler>();
        canvasObject.AddComponent<GraphicRaycaster>();
        return canvas;
    }

    private static GameObject CreateRoot(Transform parent)
    {
        GameObject rootObject = new GameObject("DefenseBaseHealthPanel");
        rootObject.transform.SetParent(parent, false);

        RectTransform rectTransform = rootObject.AddComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0.5f, 1f);
        rectTransform.anchorMax = new Vector2(0.5f, 1f);
        rectTransform.pivot = new Vector2(0.5f, 1f);
        rectTransform.sizeDelta = new Vector2(360f, 52f);
        rectTransform.anchoredPosition = new Vector2(0f, -18f);

        return rootObject;
    }

    private static TMP_Text CreateLabel(Transform parent)
    {
        GameObject labelObject = new GameObject("BaseHealthLabel");
        labelObject.transform.SetParent(parent, false);

        RectTransform rectTransform = labelObject.AddComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0f, 0.5f);
        rectTransform.anchorMax = new Vector2(1f, 1f);
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;

        TMP_Text text = labelObject.AddComponent<TextMeshProUGUI>();
        text.alignment = TextAlignmentOptions.Center;
        text.fontSize = 18f;
        text.color = Color.white;
        return text;
    }

    private static Slider CreateSlider(Transform parent)
    {
        GameObject sliderObject = new GameObject("BaseHealthSlider");
        sliderObject.transform.SetParent(parent, false);

        RectTransform sliderRectTransform = sliderObject.AddComponent<RectTransform>();
        sliderRectTransform.anchorMin = new Vector2(0f, 0f);
        sliderRectTransform.anchorMax = new Vector2(1f, 0.45f);
        sliderRectTransform.offsetMin = Vector2.zero;
        sliderRectTransform.offsetMax = Vector2.zero;

        Slider slider = sliderObject.AddComponent<Slider>();
        slider.transition = Selectable.Transition.None;

        Image background = CreateImage("Background", sliderObject.transform, new Color(0.15f, 0.15f, 0.15f, 0.9f));
        Image fill = CreateImage("Fill", sliderObject.transform, new Color(0.88f, 0.16f, 0.12f, 1f));

        RectTransform backgroundRectTransform = background.GetComponent<RectTransform>();
        backgroundRectTransform.anchorMin = Vector2.zero;
        backgroundRectTransform.anchorMax = Vector2.one;
        backgroundRectTransform.offsetMin = Vector2.zero;
        backgroundRectTransform.offsetMax = Vector2.zero;

        RectTransform fillRectTransform = fill.GetComponent<RectTransform>();
        fillRectTransform.anchorMin = Vector2.zero;
        fillRectTransform.anchorMax = Vector2.one;
        fillRectTransform.offsetMin = Vector2.zero;
        fillRectTransform.offsetMax = Vector2.zero;

        slider.targetGraphic = background;
        slider.fillRect = fillRectTransform;
        slider.direction = Slider.Direction.LeftToRight;
        return slider;
    }

    private static Image CreateImage(string objectName, Transform parent, Color color)
    {
        GameObject imageObject = new GameObject(objectName);
        imageObject.transform.SetParent(parent, false);

        RectTransform rectTransform = imageObject.AddComponent<RectTransform>();
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;

        Image image = imageObject.AddComponent<Image>();
        image.color = color;
        return image;
    }
}
