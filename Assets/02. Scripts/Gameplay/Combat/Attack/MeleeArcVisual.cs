using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class MeleeArcVisual : MonoBehaviour
{
    [SerializeField] private float _lifeTime = 0.15f;
    [SerializeField] private float _startScale = 0.2f;
    [SerializeField] private float _endScale = 1f;

    private SpriteRenderer _spriteRenderer;
    private float _elapsedTime;
    private Color _startColor;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _startColor = _spriteRenderer.color;
    }

    private void Update()
    {
        _elapsedTime += Time.deltaTime;
        float progress = Mathf.Clamp01(_elapsedTime / _lifeTime);
        float scale = Mathf.Lerp(_startScale, _endScale, progress);

        transform.localScale = new Vector3(scale, scale, 1f);

        Color color = _startColor;
        color.a = Mathf.Lerp(_startColor.a, 0f, progress);
        _spriteRenderer.color = color;

        if (progress >= 1f)
            Destroy(gameObject);
    }

    public void Initialize(float lifeTime)
    {
        _lifeTime = Mathf.Max(0.01f, lifeTime);
        _elapsedTime = 0f;
    }
}
