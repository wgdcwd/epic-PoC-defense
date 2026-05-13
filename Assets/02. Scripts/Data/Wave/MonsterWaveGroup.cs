using System;
using UnityEngine;

[Serializable]
public class MonsterWaveGroup
{
    [SerializeField] private MonsterController _monsterPrefab;
    [SerializeField] private int _count = 1;
    [SerializeField] private float _spawnInterval = 0.5f;
    [SerializeField] private int _spawnPointIndex;

    public MonsterController MonsterPrefab => _monsterPrefab;
    public int Count => Mathf.Max(0, _count);
    public float SpawnInterval => Mathf.Max(0f, _spawnInterval);
    public int SpawnPointIndex => Mathf.Max(0, _spawnPointIndex);
}
