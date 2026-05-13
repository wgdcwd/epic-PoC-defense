using UnityEngine;

[RequireComponent(typeof(MonsterDeathHandler))]
public class MonsterReward : MonoBehaviour
{
    [SerializeField] private int _moneyReward = 10;
    [SerializeField] private int _experienceReward = 25;
    [SerializeField] private float _experienceShareRadius = 6f;
    [SerializeField] private bool _splitExperienceBetweenHeroes = true;
    [SerializeField] private LayerMask _heroLayerMask = ~0;

    private MonsterDeathHandler _deathHandler;
    private PlayerResourceWallet _resourceWallet;

    private void Awake()
    {
        _deathHandler = GetComponent<MonsterDeathHandler>();
        _resourceWallet = FindFirstObjectByType<PlayerResourceWallet>();
    }

    private void OnEnable()
    {
        if (_deathHandler != null)
            _deathHandler.Died += OnMonsterDied;
    }

    private void OnDisable()
    {
        if (_deathHandler != null)
            _deathHandler.Died -= OnMonsterDied;
    }

    private void OnMonsterDied(MonsterDeathHandler deathHandler)
    {
        GiveMoneyReward();
        GiveExperienceReward();
    }

    private void GiveMoneyReward()
    {
        if (_resourceWallet == null)
            _resourceWallet = FindFirstObjectByType<PlayerResourceWallet>();

        if (_resourceWallet != null)
            _resourceWallet.AddMoney(_moneyReward);
    }

    private void GiveExperienceReward()
    {
        HeroLevel[] nearbyHeroes = FindNearbyHeroes();

        if (nearbyHeroes.Length == 0)
        {
            HeroLevel nearestHero = FindNearestHero();

            if (nearestHero != null)
                nearestHero.AddExperience(_experienceReward);

            return;
        }

        if (!_splitExperienceBetweenHeroes)
        {
            for (int i = 0; i < nearbyHeroes.Length; i++)
                nearbyHeroes[i].AddExperience(_experienceReward);

            return;
        }

        int baseExperience = _experienceReward / nearbyHeroes.Length;
        int remainderExperience = _experienceReward % nearbyHeroes.Length;

        for (int i = 0; i < nearbyHeroes.Length; i++)
        {
            int experience = baseExperience;

            if (i < remainderExperience)
                experience++;

            nearbyHeroes[i].AddExperience(experience);
        }
    }

    private HeroLevel[] FindNearbyHeroes()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, _experienceShareRadius, _heroLayerMask);
        HeroLevel[] heroes = new HeroLevel[hits.Length];
        int heroCount = 0;

        for (int i = 0; i < hits.Length; i++)
        {
            HeroLevel hero = hits[i].GetComponentInParent<HeroLevel>();

            if (hero == null)
                continue;

            UnitRuntimeStats stats = hero.GetComponent<UnitRuntimeStats>();

            if (stats != null && stats.IsDead)
                continue;

            if (ContainsHero(heroes, heroCount, hero))
                continue;

            heroes[heroCount] = hero;
            heroCount++;
        }

        HeroLevel[] result = new HeroLevel[heroCount];

        for (int i = 0; i < heroCount; i++)
            result[i] = heroes[i];

        return result;
    }

    private HeroLevel FindNearestHero()
    {
        HeroLevel[] heroes = FindObjectsByType<HeroLevel>(FindObjectsSortMode.None);
        HeroLevel nearestHero = null;
        float nearestDistance = float.MaxValue;

        for (int i = 0; i < heroes.Length; i++)
        {
            if (heroes[i] == null)
                continue;

            UnitRuntimeStats stats = heroes[i].GetComponent<UnitRuntimeStats>();

            if (stats != null && stats.IsDead)
                continue;

            float distance = (heroes[i].transform.position - transform.position).sqrMagnitude;

            if (distance >= nearestDistance)
                continue;

            nearestDistance = distance;
            nearestHero = heroes[i];
        }

        return nearestHero;
    }

    private bool ContainsHero(HeroLevel[] heroes, int heroCount, HeroLevel hero)
    {
        for (int i = 0; i < heroCount; i++)
        {
            if (heroes[i] == hero)
                return true;
        }

        return false;
    }
}
