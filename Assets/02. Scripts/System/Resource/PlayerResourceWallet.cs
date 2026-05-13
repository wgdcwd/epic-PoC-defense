using UnityEngine;

public class PlayerResourceWallet : MonoBehaviour
{
    [SerializeField] private int _money;

    public int Money => _money;

    public void AddMoney(int amount)
    {
        if (amount <= 0)
            return;

        _money += amount;
    }

    public bool TrySpendMoney(int amount)
    {
        if (amount <= 0)
            return true;

        if (_money < amount)
            return false;

        _money -= amount;
        return true;
    }
}
