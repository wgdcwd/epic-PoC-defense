using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class MoveCommandMarker : MonoBehaviour
{
    [SerializeField] private float _lifeTime = 0.45f;
    [SerializeField] private float _startRadius = 0.2f;
    [SerializeField] private float _endRadius = 0.75f;
    [SerializeField] private float _lineWidth = 0.06f;
    [SerializeField] private int _segments = 48;
    [SerializeField] private Color _startColor = new Color(0.2f, 0.9f, 1f, 0.95f);
    [SerializeField] private Color _endColor = new Color(0.2f, 0.9f, 1f, 0f);

    private LineRenderer _lineRenderer;
    private float _elapsedTime;
    private bool _isPlaying;

    private void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.useWorldSpace = false;
        _lineRenderer.loop = true;
        _lineRenderer.positionCount = Mathf.Max(12, _segments);
        _lineRenderer.startWidth = _lineWidth;
        _lineRenderer.endWidth = _lineWidth;
        _lineRenderer.sortingOrder = 100;

        if (_lineRenderer.sharedMaterial == null)
        {
            Shader defaultShader = Shader.Find("Sprites/Default");

            if (defaultShader != null)
                _lineRenderer.sharedMaterial = new Material(defaultShader);
        }

        _lineRenderer.enabled = false;
    }

    private void Update()
    {
        if (!_isPlaying)
            return;

        _elapsedTime += Time.deltaTime;
        float progress = Mathf.Clamp01(_elapsedTime / _lifeTime);
        float radius = Mathf.Lerp(_startRadius, _endRadius, progress);
        Color color = Color.Lerp(_startColor, _endColor, progress);

        DrawRing(radius, color);

        if (progress >= 1f)
        {
            _isPlaying = false;
            _lineRenderer.enabled = false;
        }
    }

    public static MoveCommandMarker CreateDefault()
    {
        GameObject markerObject = new GameObject("MoveCommandMarker");
        return markerObject.AddComponent<MoveCommandMarker>();
    }

    public void ShowAt(Vector2 worldPosition)
    {
        transform.position = new Vector3(worldPosition.x, worldPosition.y, 0f);
        _elapsedTime = 0f;
        _isPlaying = true;
        _lineRenderer.enabled = true;
        DrawRing(_startRadius, _startColor);
    }

    private void DrawRing(float radius, Color color)
    {
        _lineRenderer.startColor = color;
        _lineRenderer.endColor = color;

        for (int i = 0; i < _lineRenderer.positionCount; i++)
        {
            float angle = 2f * Mathf.PI * i / _lineRenderer.positionCount;
            Vector3 position = new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0f);
            _lineRenderer.SetPosition(i, position);
        }
    }
}
