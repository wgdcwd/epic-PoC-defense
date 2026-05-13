using UnityEngine;

[RequireComponent(typeof(HeroController))]
public class HeroRecruitment : MonoBehaviour
{
    [SerializeField] private float _recruitRadius = 2f;
    [SerializeField] private LayerMask _recruiterLayerMask;
    [SerializeField] private float _checkInterval = 0.15f;
    [SerializeField] private string _debugState;

    private HeroController _hero;
    private float _checkTimer;
    private bool _isRecruited;

    private void Awake()
    {
        _hero = GetComponent<HeroController>();

        if (_recruiterLayerMask.value == 0)
            _recruiterLayerMask = 1 << gameObject.layer;
    }

    private void OnEnable()
    {
        _checkTimer = 0f;
        _isRecruited = _hero != null && _hero.IsRecruited;
        _debugState = _isRecruited ? "Recruited" : "Waiting";
    }

    private void Update()
    {
        if (_hero == null || _hero.IsRecruited || _isRecruited)
            return;

        _checkTimer -= Time.deltaTime;

        if (_checkTimer > 0f)
            return;

        _checkTimer = _checkInterval;
        TryRecruit();
    }

    private void TryRecruit()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, _recruitRadius, _recruiterLayerMask);

        for (int i = 0; i < hits.Length; i++)
        {
            HeroController recruiter = hits[i].GetComponentInParent<HeroController>();

            if (recruiter == null || recruiter == _hero || !recruiter.IsRecruited || recruiter.IsDead)
                continue;

            Recruit(recruiter);
            return;
        }
    }

    private void Recruit(HeroController recruiter)
    {
        _isRecruited = true;
        _debugState = "Recruited by " + recruiter.DisplayName;
        _hero.Recruit();
        enabled = false;
    }
}
