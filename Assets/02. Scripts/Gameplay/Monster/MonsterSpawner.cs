using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    [SerializeField] private MonsterController _monsterPrefab;
    [SerializeField] private MonsterPath _path;
    [SerializeField] private bool _spawnOnStart = true;
    [SerializeField] private int _spawnCount = 1;
    [SerializeField] private float _spawnSpacing = 1.2f;

    private void Start()
    {
        if (!_spawnOnStart)
            return;

        SpawnMonsters();
    }

    public void SpawnMonsters()
    {
        if (_monsterPrefab == null)
            return;

        for (int i = 0; i < _spawnCount; i++)
        {
            Vector3 offset = new Vector3(i * _spawnSpacing, 0f, 0f);
            MonsterController monster = Instantiate(_monsterPrefab, transform.position + offset, transform.rotation);
            MonsterPathFollower pathFollower = monster.GetComponent<MonsterPathFollower>();

            if (pathFollower != null)
                pathFollower.SetPath(_path);
        }
    }
}
