using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _coinText;
    [SerializeField] private TextMeshProUGUI _coinText2;

    private void Update()
    {
        _coinText.text = WalletController.Instance.Coins.ToString();
        _coinText2.text = WalletController.Instance.Coins.ToString();
    }

    public void LoadLevel(string levelName)
    {
        SceneManager.LoadScene(levelName);
    }
}
