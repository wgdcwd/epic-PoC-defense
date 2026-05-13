using UnityEngine;

[RequireComponent(typeof(WaveStatusView))]
public class WaveStatusController : MonoBehaviour
{
    [SerializeField] private MonsterWaveSystem _waveSystem;
    [SerializeField] private WaveStatusView _view;
    [SerializeField] private float _refreshInterval = 0.1f;

    private float _refreshTimer;

    private void Awake()
    {
        if (_waveSystem == null)
            _waveSystem = FindFirstObjectByType<MonsterWaveSystem>();

        if (_view == null)
            _view = GetComponent<WaveStatusView>();
    }

    private void OnEnable()
    {
        Refresh();
    }

    private void Update()
    {
        _refreshTimer -= Time.deltaTime;

        if (_refreshTimer > 0f)
            return;

        _refreshTimer = _refreshInterval;
        Refresh();
    }

    private void Refresh()
    {
        if (_view == null)
            return;

        if (_waveSystem == null)
        {
            _view.Hide();
            return;
        }

        _view.Show();
        _view.SetWaveText(GetWaveText());
        _view.SetNextWaveTimeText(GetNextWaveTimeText());
        _view.SetMonsterCountText("Monsters: " + GetActiveMonsterCount());
    }

    private string GetWaveText()
    {
        if (_waveSystem.TotalWaveCount <= 0)
            return "Wave: -";

        if (_waveSystem.CurrentWaveNumber <= 0)
            return "Wave: 0 / " + _waveSystem.TotalWaveCount;

        return "Wave: " + _waveSystem.CurrentWaveNumber + " / " + _waveSystem.TotalWaveCount;
    }

    private string GetNextWaveTimeText()
    {
        if (!_waveSystem.IsRunning)
            return "Next Wave: Paused";

        if (!_waveSystem.HasNextWave)
            return "Next Wave: None";

        int remainingSeconds = Mathf.CeilToInt(Mathf.Max(0f, _waveSystem.NextWaveTimer));
        int minutes = remainingSeconds / 60;
        int seconds = remainingSeconds % 60;

        return "Next Wave " + _waveSystem.NextWaveNumber + ": " + minutes.ToString("00") + ":" + seconds.ToString("00");
    }

    private int GetActiveMonsterCount()
    {
        MonsterController[] monsters = FindObjectsByType<MonsterController>(FindObjectsSortMode.None);
        int count = 0;

        for (int i = 0; i < monsters.Length; i++)
        {
            UnitRuntimeStats stats = monsters[i].GetComponent<UnitRuntimeStats>();

            if (stats != null && stats.IsDead)
                continue;

            count++;
        }

        return count;
    }
}
