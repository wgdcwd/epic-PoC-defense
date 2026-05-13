using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverController : MonoBehaviour
{
    [SerializeField] private DefenseBase _defenseBase;
    [SerializeField] private GameOverView _view;
    [SerializeField] private bool _pauseOnGameOver = true;

    private bool _isGameOver;

    private void Awake()
    {
        if (_defenseBase == null)
            _defenseBase = FindFirstObjectByType<DefenseBase>();

        if (_view == null)
            _view = FindFirstObjectByType<GameOverView>(FindObjectsInactive.Include);

        if (_view == null)
            _view = GameOverView.CreateDefault();

        _view.Hide();
    }

    private void OnEnable()
    {
        SubscribeDefenseBase();

        if (_view != null)
            _view.RestartClicked += RestartScene;
    }

    private void OnDisable()
    {
        if (_defenseBase != null)
            _defenseBase.Destroyed -= OnDefenseBaseDestroyed;

        if (_view != null)
            _view.RestartClicked -= RestartScene;
    }

    public void SetDefenseBase(DefenseBase defenseBase)
    {
        if (_defenseBase == defenseBase)
            return;

        if (_defenseBase != null)
            _defenseBase.Destroyed -= OnDefenseBaseDestroyed;

        _defenseBase = defenseBase;
        SubscribeDefenseBase();
    }

    private void OnDefenseBaseDestroyed(DefenseBase defenseBase)
    {
        GameOver();
    }

    public void GameOver()
    {
        if (_isGameOver)
            return;

        _isGameOver = true;

        if (_pauseOnGameOver)
            Time.timeScale = 0f;

        if (_view != null)
            _view.Show();
    }

    public void RestartScene()
    {
        Time.timeScale = 1f;
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }

    private void SubscribeDefenseBase()
    {
        if (_defenseBase == null)
            return;

        _defenseBase.Destroyed -= OnDefenseBaseDestroyed;
        _defenseBase.Destroyed += OnDefenseBaseDestroyed;
    }
}
