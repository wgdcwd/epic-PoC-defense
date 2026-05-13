using UnityEngine;

public interface IHeroInteractable
{
    Transform Transform { get; }
    bool CanInteract(HeroController hero);
    void Interact(HeroController hero);
}
