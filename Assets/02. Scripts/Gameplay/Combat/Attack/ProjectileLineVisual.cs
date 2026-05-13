using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class ProjectileLineVisual : MonoBehaviour
{
    private static Material _sharedMaterial;

    private LineRenderer _lineRenderer;

    private void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.useWorldSpace = false;
        _lineRenderer.positionCount = 2;
        _lineRenderer.SetPosition(0, new Vector3(-0.08f, 0f, 0f));
        _lineRenderer.SetPosition(1, new Vector3(0.08f, 0f, 0f));
        _lineRenderer.startWidth = 0.04f;
        _lineRenderer.endWidth = 0.04f;
        _lineRenderer.startColor = Color.yellow;
        _lineRenderer.endColor = Color.yellow;
        _lineRenderer.sortingOrder = 20;
        _lineRenderer.material = GetSharedMaterial();
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
