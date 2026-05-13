using System;
using System.Collections.Generic;
using UnityEngine;

public class HeroRoster : MonoBehaviour
{
    public static HeroRoster Instance { get; private set; }

    private readonly List<HeroController> _heroes = new List<HeroController>();

    public IReadOnlyList<HeroController> Heroes => _heroes;

    public event Action Changed;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    private void Start()
    {
        HeroController[] heroes = FindObjectsByType<HeroController>(FindObjectsSortMode.None);

        for (int i = 0; i < heroes.Length; i++)
            Register(heroes[i]);
    }

    public void Register(HeroController hero)
    {
        if (hero == null || !hero.IsRecruited || _heroes.Contains(hero))
            return;

        _heroes.Add(hero);
        Changed?.Invoke();
    }

    public void Unregister(HeroController hero)
    {
        if (hero == null)
            return;

        if (!_heroes.Remove(hero))
            return;

        Changed?.Invoke();
    }
}
