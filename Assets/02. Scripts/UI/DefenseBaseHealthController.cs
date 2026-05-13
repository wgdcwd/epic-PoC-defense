using UnityEngine;

public class DefenseBaseHealthController : MonoBehaviour
{
    [SerializeField] private DefenseBase _defenseBase;
    [SerializeField] private DefenseBaseHealthView _view;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void CreateRuntimeController()
    {
        DefenseBase defenseBase = FindFirstObjectByType<DefenseBase>();

        if (defenseBase == null)
            return;

        DefenseBaseHealthController existingController =
            FindFirstObjectByType<DefenseBaseHealthController>(FindObjectsInactive.Include);

        if (existingController != null)
        {
            existingController.SetDefenseBase(defenseBase);
            return;
        }

        GameObject controllerObject = new GameObject(nameof(DefenseBaseHealthController));
        DefenseBaseHealthController controller = controllerObject.AddComponent<DefenseBaseHealthController>();
        controller.SetDefenseBase(defenseBase);
    }

    private void Awake()
    {
        if (_defenseBase == null)
            _defenseBase = FindFirstObjectByType<DefenseBase>();

        if (_view == null)
            _view = FindFirstObjectByType<DefenseBaseHealthView>(FindObjectsInactive.Include);

        if (_view == null)
            _view = DefenseBaseHealthView.CreateDefault();

        Refresh();
    }

    private void OnEnable()
    {
        SubscribeDefenseBase();
        Refresh();
    }

    private void OnDisable()
    {
        if (_defenseBase != null)
        {
            _defenseBase.HealthChanged -= OnHealthChanged;
            _defenseBase.Destroyed -= OnDestroyed;
        }
    }

    public void SetDefenseBase(DefenseBase defenseBase)
    {
        if (_defenseBase == defenseBase)
            return;

        if (_defenseBase != null)
        {
            _defenseBase.HealthChanged -= OnHealthChanged;
            _defenseBase.Destroyed -= OnDestroyed;
        }

        _defenseBase = defenseBase;
        SubscribeDefenseBase();
        Refresh();
    }

    private void SubscribeDefenseBase()
    {
        if (_defenseBase == null)
            return;

        _defenseBase.HealthChanged -= OnHealthChanged;
        _defenseBase.Destroyed -= OnDestroyed;
        _defenseBase.HealthChanged += OnHealthChanged;
        _defenseBase.Destroyed += OnDestroyed;
    }

    private void OnHealthChanged(DefenseBase defenseBase)
    {
        Refresh();
    }

    private void OnDestroyed(DefenseBase defenseBase)
    {
        Refresh();
    }

    private void Refresh()
    {
        if (_view == null || _defenseBase == null)
            return;

        _view.Show();
        _view.SetHealth(_defenseBase.CurrentHealth, _defenseBase.MaxHealth);
    }
}
