using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

public class GameOverView : MonoBehaviour
{
    [SerializeField] private GameObject _root;
    [SerializeField] private TMP_Text _titleText;
    [SerializeField] private TMP_Text _bodyText;
    [SerializeField] private Button _restartButton;

    public event Action RestartClicked;

    private void Awake()
    {
        if (_root == null)
            _root = gameObject;

        if (_restartButton != null)
            _restartButton.onClick.AddListener(OnRestartClicked);
    }

    private void OnDestroy()
    {
        if (_restartButton != null)
            _restartButton.onClick.RemoveListener(OnRestartClicked);
    }

    public void Show()
    {
        if (_titleText != null)
            _titleText.text = "Game Over";

        if (_bodyText != null)
            _bodyText.text = "Defense Base destroyed.";

        _root.SetActive(true);
    }

    public void Hide()
    {
        _root.SetActive(false);
    }

    public static GameOverView CreateDefault()
    {
        Canvas canvas = CreateCanvas();
        GameObject root = CreatePanel(canvas.transform);
        GameOverView view = root.AddComponent<GameOverView>();
        TMP_Text titleText = CreateText(root.transform, "Game Over", 36, new Vector2(0f, 60f));
        TMP_Text bodyText = CreateText(root.transform, "Defense Base destroyed.", 20, new Vector2(0f, 10f));
        Button restartButton = CreateButton(root.transform);

        view._root = root;
        view._titleText = titleText;
        view._bodyText = bodyText;
        view._restartButton = restartButton;
        restartButton.onClick.AddListener(view.OnRestartClicked);
        return view;
    }

    private void OnRestartClicked()
    {
        RestartClicked?.Invoke();
    }

    private static Canvas CreateCanvas()
    {
        EnsureEventSystem();

        GameObject canvasObject = new GameObject("GameOverCanvas");
        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;
        canvasObject.AddComponent<CanvasScaler>();
        canvasObject.AddComponent<GraphicRaycaster>();
        return canvas;
    }

    private static void EnsureEventSystem()
    {
        if (EventSystem.current != null)
            return;

        GameObject eventSystemObject = new GameObject("EventSystem");
        eventSystemObject.AddComponent<EventSystem>();
        eventSystemObject.AddComponent<InputSystemUIInputModule>();
    }

    private static GameObject CreatePanel(Transform parent)
    {
        GameObject panelObject = new GameObject("GameOverPanel");
        panelObject.transform.SetParent(parent, false);

        RectTransform rectTransform = panelObject.AddComponent<RectTransform>();
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;

        Image image = panelObject.AddComponent<Image>();
        image.color = new Color(0f, 0f, 0f, 0.72f);
        return panelObject;
    }

    private static TMP_Text CreateText(Transform parent, string text, int fontSize, Vector2 anchoredPosition)
    {
        GameObject textObject = new GameObject(text);
        textObject.transform.SetParent(parent, false);

        RectTransform rectTransform = textObject.AddComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.sizeDelta = new Vector2(520f, 60f);
        rectTransform.anchoredPosition = anchoredPosition;

        TMP_Text tmpText = textObject.AddComponent<TextMeshProUGUI>();
        tmpText.text = text;
        tmpText.fontSize = fontSize;
        tmpText.alignment = TextAlignmentOptions.Center;
        tmpText.color = Color.white;
        return tmpText;
    }

    private static Button CreateButton(Transform parent)
    {
        GameObject buttonObject = new GameObject("RestartButton");
        buttonObject.transform.SetParent(parent, false);

        RectTransform rectTransform = buttonObject.AddComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.sizeDelta = new Vector2(180f, 48f);
        rectTransform.anchoredPosition = new Vector2(0f, -70f);

        Image image = buttonObject.AddComponent<Image>();
        image.color = new Color(0.9f, 0.9f, 0.9f, 1f);

        Button button = buttonObject.AddComponent<Button>();
        CreateButtonText(buttonObject.transform);
        return button;
    }

    private static void CreateButtonText(Transform parent)
    {
        TMP_Text text = CreateText(parent, "Restart", 20, Vector2.zero);
        RectTransform rectTransform = text.GetComponent<RectTransform>();
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;
        text.color = Color.black;
    }
}
