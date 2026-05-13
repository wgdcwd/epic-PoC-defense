using TMPro;
using UnityEngine;

public class WaveStatusView : MonoBehaviour
{
    [SerializeField] private GameObject _root;
    [SerializeField] private TMP_Text _waveText;
    [SerializeField] private TMP_Text _nextWaveTimeText;
    [SerializeField] private TMP_Text _monsterCountText;

    private void Awake()
    {
        if (_root == null)
            _root = gameObject;
    }

    public void Show()
    {
        _root.SetActive(true);
    }

    public void Hide()
    {
        _root.SetActive(false);
    }

    public void SetWaveText(string text)
    {
        _waveText.text = text;
    }

    public void SetNextWaveTimeText(string text)
    {
        _nextWaveTimeText.text = text;
    }

    public void SetMonsterCountText(string text)
    {
        _monsterCountText.text = text;
    }
}
