using UnityEngine;

[CreateAssetMenu(fileName = "WaveDefinition", menuName = "Game/Data/Wave Definition")]
public class WaveDefinition : ScriptableObject
{
    [SerializeField] private float _delayFromPreviousWave = 120f;
    [SerializeField] private MonsterWaveGroup[] _groups;

    public float DelayFromPreviousWave => Mathf.Max(0f, _delayFromPreviousWave);
    public int GroupCount => _groups != null ? _groups.Length : 0;

    public MonsterWaveGroup GetGroup(int index)
    {
        if (_groups == null)
            return null;

        if (index < 0 || index >= _groups.Length)
            return null;

        return _groups[index];
    }
}
