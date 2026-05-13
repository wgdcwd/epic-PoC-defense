using UnityEngine;

public class MonsterSpawnPoint : MonoBehaviour
{
    [SerializeField] private MonsterPath _path;
    [SerializeField] private float _spawnRadius = 0.2f;

    public MonsterController Spawn(MonsterController prefab)
    {
        if (prefab == null)
            return null;

        Vector2 offset = Random.insideUnitCircle * _spawnRadius;
        Vector3 position = transform.position + new Vector3(offset.x, offset.y, 0f);
        MonsterController monster = Instantiate(prefab, position, transform.rotation);
        MonsterPathFollower pathFollower = monster.GetComponent<MonsterPathFollower>();

        if (pathFollower != null)
            pathFollower.SetPath(_path);

        return monster;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, _spawnRadius);
    }
}
