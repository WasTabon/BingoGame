using UnityEngine;

public class BoostsManager : MonoBehaviour
{
    public static BoostsManager Instance;
    
    public bool HasLuckyNumber { get; private set; }
    public bool HasSprintBoost { get; private set; }
    public bool HasCardVision { get; private set; }

    [Header("Boost Settings")]
    public int boostPrice = 50;
    public float sprintMultiplier = 1.2f;

    private const string LUCKY_NUMBER_KEY = "boost_lucky_number";
    private const string SPRINT_BOOST_KEY = "boost_sprint";
    private const string CARD_VISION_KEY = "boost_card_vision";

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        LoadBoosts();
    }

    void LoadBoosts()
    {
        HasLuckyNumber = PlayerPrefs.GetInt(LUCKY_NUMBER_KEY, 0) == 1;
        HasSprintBoost = PlayerPrefs.GetInt(SPRINT_BOOST_KEY, 0) == 1;
        HasCardVision = PlayerPrefs.GetInt(CARD_VISION_KEY, 0) == 1;
        
        Debug.Log($"Boosts loaded - Lucky: {HasLuckyNumber}, Sprint: {HasSprintBoost}, Vision: {HasCardVision}");
    }

    public bool BuyLuckyNumber()
    {
        if (HasLuckyNumber)
        {
            Debug.Log("Already purchased Lucky Number");
            return false;
        }

        if (WalletController.Instance.Coins >= boostPrice)
        {
            WalletController.Instance.Coins -= boostPrice;
            HasLuckyNumber = true;
            PlayerPrefs.SetInt(LUCKY_NUMBER_KEY, 1);
            PlayerPrefs.Save();
            Debug.Log("Lucky Number purchased!");
            return true;
        }

        Debug.Log("Not enough coins for Lucky Number");
        return false;
    }

    public bool BuySprintBoost()
    {
        if (HasSprintBoost)
        {
            Debug.Log("Already purchased Sprint Boost");
            return false;
        }

        if (WalletController.Instance.Coins >= boostPrice)
        {
            WalletController.Instance.Coins -= boostPrice;
            HasSprintBoost = true;
            PlayerPrefs.SetInt(SPRINT_BOOST_KEY, 1);
            PlayerPrefs.Save();
            Debug.Log("Sprint Boost purchased!");
            return true;
        }

        Debug.Log("Not enough coins for Sprint Boost");
        return false;
    }

    public bool BuyCardVision()
    {
        if (HasCardVision)
        {
            Debug.Log("Already purchased Card Vision");
            return false;
        }

        if (WalletController.Instance.Coins >= boostPrice)
        {
            WalletController.Instance.Coins -= boostPrice;
            HasCardVision = true;
            PlayerPrefs.SetInt(CARD_VISION_KEY, 1);
            PlayerPrefs.Save();
            Debug.Log("Card Vision purchased!");
            return true;
        }

        Debug.Log("Not enough coins for Card Vision");
        return false;
    }
}