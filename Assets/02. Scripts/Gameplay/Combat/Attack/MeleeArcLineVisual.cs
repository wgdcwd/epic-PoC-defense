using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class MeleeArcLineVisual : MonoBehaviour
{
    private const int ArcSegmentCount = 16;

    private static Material _sharedMaterial;

    private LineRenderer _lineRenderer;
    private float _lifeTime = 0.15f;
    private float _elapsedTime;
    private Color _startColor = new Color(1f, 0.9f, 0.35f, 0.9f);

    private void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.useWorldSpace = false;
        _lineRenderer.loop = false;
        _lineRenderer.startWidth = 0.05f;
        _lineRenderer.endWidth = 0.05f;
        _lineRenderer.sortingOrder = 20;
        _lineRenderer.material = GetSharedMaterial();
    }

    private void Update()
    {
        _elapsedTime += Time.deltaTime;
        float progress = Mathf.Clamp01(_elapsedTime / _lifeTime);
        Color color = _startColor;
        color.a = Mathf.Lerp(_startColor.a, 0f, progress);

        _lineRenderer.startColor = color;
        _lineRenderer.endColor = color;

        if (progress >= 1f)
            Destroy(gameObject);
    }

    public void Initialize(float range, float arcAngle, float lifeTime)
    {
        _lifeTime = Mathf.Max(0.01f, lifeTime);
        _elapsedTime = 0f;

        int positionCount = ArcSegmentCount + 3;
        _lineRenderer.positionCount = positionCount;
        _lineRenderer.SetPosition(0, Vector3.zero);

        float halfAngle = arcAngle * 0.5f;

        for (int i = 0; i <= ArcSegmentCount; i++)
        {
            float progress = (float)i / ArcSegmentCount;
            float angle = Mathf.Lerp(-halfAngle, halfAngle, progress) * Mathf.Deg2Rad;
            Vector3 point = new Vector3(Mathf.Cos(angle) * range, Mathf.Sin(angle) * range, 0f);
            _lineRenderer.SetPosition(i + 1, point);
        }

        _lineRenderer.SetPosition(positionCount - 1, Vector3.zero);
    }

    private static Material GetSharedMaterial()
    {
        if (_sharedMaterial != null)
            return _sharedMaterial;

        Shader shader = Shader.Find("Sprites/Default");

        if (shader == null)
            shader = Shader.Find("Universal Render Pipeline/2D/Sprite-Unlit-Default");

        if (shader == null)
            return null;

        _sharedMaterial = new Material(shader);
        return _sharedMaterial;
    }
}
