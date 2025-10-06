using UnityEngine;

public class WalletController : MonoBehaviour
{
    public static WalletController Instance;

    public int Coins
    {
        get => _coins;
        set
        {
            _coins = value;
            PlayerPrefs.SetInt("coins", _coins);
        }
    }
    private int _coins;
    
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        _coins = PlayerPrefs.GetInt("coins", 50);
    }
}
