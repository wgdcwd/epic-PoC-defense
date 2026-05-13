using System.Collections;
using UnityEngine;

public class MonsterWaveSystem : MonoBehaviour
{
    [SerializeField] private WaveDefinition[] _waves;
    [SerializeField] private MonsterSpawnPoint[] _spawnPoints;
    [SerializeField] private bool _startOnAwake = true;

    [SerializeField] private int _currentWaveIndex = -1;
    [SerializeField] private float _nextWaveTimer;
    [SerializeField] private bool _isRunning;
    [SerializeField] private int _aliveMonsterCount;

    public int CurrentWaveIndex => _currentWaveIndex;
    public float NextWaveTimer => _nextWaveTimer;
    public int AliveMonsterCount => _aliveMonsterCount;
    public bool IsRunning => _isRunning;
    public bool HasNextWave => HasNextWaveInternal();
    public int CurrentWaveNumber => Mathf.Max(0, _currentWaveIndex + 1);
    public int NextWaveNumber => HasNextWave ? _currentWaveIndex + 2 : 0;
    public int TotalWaveCount => _waves != null ? _waves.Length : 0;

    private void Start()
    {
        if (_startOnAwake)
            StartWaves();
    }

    private void Update()
    {
        if (!_isRunning)
            return;

        if (!HasNextWaveInternal())
            return;

        _nextWaveTimer -= Time.deltaTime;

        if (_nextWaveTimer > 0f)
            return;

        StartNextWave();
    }

    public void StartWaves()
    {
        if (_isRunning)
            return;

        _isRunning = true;
        _currentWaveIndex = -1;
        _nextWaveTimer = GetNextWaveDelay();
    }

    private void StartNextWave()
    {
        _currentWaveIndex++;

        WaveDefinition wave = _waves[_currentWaveIndex];
        SpawnWave(wave);

        _nextWaveTimer = GetNextWaveDelay();
    }

    private void SpawnWave(WaveDefinition wave)
    {
        if (wave == null)
            return;

        for (int groupIndex = 0; groupIndex < wave.GroupCount; groupIndex++)
        {
            MonsterWaveGroup group = wave.GetGroup(groupIndex);

            if (group == null)
                continue;

            StartCoroutine(SpawnGroupRoutine(group));
        }
    }

    private IEnumerator SpawnGroupRoutine(MonsterWaveGroup group)
    {
        MonsterSpawnPoint spawnPoint = GetSpawnPoint(group.SpawnPointIndex);

        if (spawnPoint == null)
            yield break;

        for (int i = 0; i < group.Count; i++)
        {
            MonsterController monster = spawnPoint.Spawn(group.MonsterPrefab);

            if (monster != null)
            {
                _aliveMonsterCount++;
                RegisterMonster(monster);
            }

            if (group.SpawnInterval > 0f)
                yield return new WaitForSeconds(group.SpawnInterval);
        }
    }

    private void RegisterMonster(MonsterController monster)
    {
        MonsterDeathHandler deathHandler = monster.GetComponent<MonsterDeathHandler>();

        if (deathHandler != null)
            deathHandler.Died += OnMonsterDied;
    }

    private void OnMonsterDied(MonsterDeathHandler deathHandler)
    {
        deathHandler.Died -= OnMonsterDied;

        _aliveMonsterCount = Mathf.Max(0, _aliveMonsterCount - 1);
    }

    private MonsterSpawnPoint GetSpawnPoint(int index)
    {
        if (_spawnPoints == null || _spawnPoints.Length == 0)
            return null;

        if (index >= _spawnPoints.Length)
            return _spawnPoints[0];

        return _spawnPoints[index];
    }

    private bool HasNextWaveInternal()
    {
        return _waves != null && _currentWaveIndex + 1 < _waves.Length;
    }

    private float GetNextWaveDelay()
    {
        if (!HasNextWaveInternal())
            return 0f;

        WaveDefinition nextWave = _waves[_currentWaveIndex + 1];

        if (nextWave == null)
            return 0f;

        return nextWave.DelayFromPreviousWave;
    }
}
