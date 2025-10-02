using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System.Collections.Generic;

public class BallsController : MonoBehaviour
{
    [Header("Bingo Cards")]
    [SerializeField] private RectTransform playerBingoParent;
    [SerializeField] private RectTransform bot1BingoParent;
    [SerializeField] private RectTransform bot2BingoParent;
    
    [Header("Choice Buttons")]
    [SerializeField] private Button[] choiceButtons;
    
    [Header("Rotation Objects")]
    [SerializeField] private Transform firstRotationObject;
    [SerializeField] private Transform secondRotationObject;
    
    [Header("Fade Objects")]
    [SerializeField] private CanvasGroup bottomPanel;
    [SerializeField] private CanvasGroup topPanel;
    
    [Header("Settings")]
    [SerializeField] private float rotationDuration = 3f;
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private Color strikethroughColor = Color.red;
    
    private int[,] playerNumbers = new int[5, 5];
    private int[,] bot1Numbers = new int[5, 5];
    private int[,] bot2Numbers = new int[5, 5];
    
    private bool[,] playerMarked = new bool[5, 5];
    private bool[,] bot1Marked = new bool[5, 5];
    private bool[,] bot2Marked = new bool[5, 5];
    
    private List<TextMeshProUGUI> playerTexts = new List<TextMeshProUGUI>();
    private List<TextMeshProUGUI> bot1Texts = new List<TextMeshProUGUI>();
    private List<TextMeshProUGUI> bot2Texts = new List<TextMeshProUGUI>();
    
    private List<int> availableNumbers = new List<int>();
    private int[] currentChoices = new int[3];
    
    private int currentPlayer = 0;
    private bool canClick = false;

    void Start()
    {
        InitializeNumberPool();
        CollectAllTexts();
        GenerateAllBingoCards();
        SetupChoiceButtons();
        StartGameSequence();
    }

    void InitializeNumberPool()
    {
        availableNumbers.Clear();
        for (int i = 1; i <= 99; i++)
        {
            availableNumbers.Add(i);
        }
    }

    void CollectAllTexts()
    {
        TextMeshProUGUI[] playerTMPs = playerBingoParent.GetComponentsInChildren<TextMeshProUGUI>();
        TextMeshProUGUI[] bot1TMPs = bot1BingoParent.GetComponentsInChildren<TextMeshProUGUI>();
        TextMeshProUGUI[] bot2TMPs = bot2BingoParent.GetComponentsInChildren<TextMeshProUGUI>();
        
        playerTexts.AddRange(playerTMPs);
        bot1Texts.AddRange(bot1TMPs);
        bot2Texts.AddRange(bot2TMPs);
    }

    void GenerateAllBingoCards()
    {
        GenerateBingoCard(playerTexts, playerNumbers);
        GenerateBingoCard(bot1Texts, bot1Numbers);
        GenerateBingoCard(bot2Texts, bot2Numbers);
    }

    void GenerateBingoCard(List<TextMeshProUGUI> texts, int[,] numbers)
    {
        List<int> usedNumbers = new List<int>();
        
        for (int i = 0; i < 25; i++)
        {
            int number;
            do
            {
                number = Random.Range(1, 100);
            } while (usedNumbers.Contains(number));
            
            usedNumbers.Add(number);
            
            int row = i / 5;
            int col = i % 5;
            numbers[row, col] = number;
            
            if (i < texts.Count)
            {
                texts[i].text = number.ToString();
            }
        }
    }

    void SetupChoiceButtons()
    {
        for (int i = 0; i < choiceButtons.Length; i++)
        {
            int index = i;
            choiceButtons[i].onClick.AddListener(() => OnChoiceButtonClicked(index));
        }
    }

    void StartGameSequence()
    {
        bottomPanel.gameObject.SetActive(false);
        topPanel.gameObject.SetActive(false);
        bottomPanel.alpha = 0;
        topPanel.alpha = 0;
        
        Sequence sequence = DOTween.Sequence();
        
        sequence.Append(firstRotationObject.DORotate(new Vector3(0, 0, 360), rotationDuration, RotateMode.FastBeyond360)
            .SetEase(Ease.InOutQuad));
        
        sequence.Append(secondRotationObject.DORotate(new Vector3(0, 0, -90), rotationDuration, RotateMode.FastBeyond360)
            .SetEase(Ease.InOutQuad));
        
        sequence.AppendCallback(() =>
        {
            bottomPanel.gameObject.SetActive(true);
            topPanel.gameObject.SetActive(true);
        });
        
        sequence.Append(bottomPanel.DOFade(1, fadeDuration));
        sequence.Join(topPanel.DOFade(1, fadeDuration));
        
        sequence.OnComplete(() => StartNewRound());
    }

    void StartNewRound()
    {
        if (availableNumbers.Count < 3)
        {
            InitializeNumberPool();
        }
        
        GenerateChoices();
        
        if (currentPlayer == 0)
        {
            canClick = true;
            EnableButtons(true);
        }
        else
        {
            canClick = false;
            EnableButtons(false);
            Invoke("BotMakeChoice", 1f);
        }
    }

    void GenerateChoices()
    {
        for (int i = 0; i < 3; i++)
        {
            int randomIndex = Random.Range(0, availableNumbers.Count);
            currentChoices[i] = availableNumbers[randomIndex];
            
            TextMeshProUGUI buttonText = choiceButtons[i].GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
            {
                buttonText.text = currentChoices[i].ToString();
            }
        }
    }

    void EnableButtons(bool enabled)
    {
        foreach (Button btn in choiceButtons)
        {
            btn.interactable = enabled;
        }
    }

    void OnChoiceButtonClicked(int buttonIndex)
    {
        if (!canClick) return;
        
        int chosenNumber = currentChoices[buttonIndex];
        ProcessChoice(chosenNumber, 0);
    }

    void BotMakeChoice()
    {
        int botIndex = currentPlayer;
        int[,] botNumbers = botIndex == 1 ? bot1Numbers : bot2Numbers;
        
        int bestChoice = -1;
        for (int i = 0; i < 3; i++)
        {
            if (HasNumber(botNumbers, currentChoices[i]))
            {
                bestChoice = i;
                break;
            }
        }
        
        if (bestChoice == -1)
        {
            bestChoice = Random.Range(0, 3);
        }
        
        int chosenNumber = currentChoices[bestChoice];
        ProcessChoice(chosenNumber, botIndex);
    }

    void ProcessChoice(int number, int playerIndex)
    {
        availableNumbers.Remove(number);
        
        if (playerIndex == 0)
        {
            MarkNumber(playerNumbers, playerMarked, playerTexts, number);
            CheckWin(playerMarked, "Player");
        }
        else if (playerIndex == 1)
        {
            MarkNumber(bot1Numbers, bot1Marked, bot1Texts, number);
            CheckWin(bot1Marked, "Bot 1");
        }
        else if (playerIndex == 2)
        {
            MarkNumber(bot2Numbers, bot2Marked, bot2Texts, number);
            CheckWin(bot2Marked, "Bot 2");
        }
        
        currentPlayer = (currentPlayer + 1) % 3;
        Invoke("StartNewRound", 0.5f);
    }

    bool HasNumber(int[,] numbers, int targetNumber)
    {
        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                if (numbers[i, j] == targetNumber)
                {
                    return true;
                }
            }
        }
        return false;
    }

    void MarkNumber(int[,] numbers, bool[,] marked, List<TextMeshProUGUI> texts, int targetNumber)
    {
        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                if (numbers[i, j] == targetNumber)
                {
                    marked[i, j] = true;
                    int cellIndex = i * 5 + j;
                    if (cellIndex < texts.Count)
                    {
                        texts[cellIndex].color = strikethroughColor;
                    }
                    return;
                }
            }
        }
    }

    void CheckWin(bool[,] marked, string playerName)
    {
        for (int i = 0; i < 5; i++)
        {
            if (marked[i, 0] && marked[i, 1] && marked[i, 2] && marked[i, 3] && marked[i, 4])
            {
                Debug.Log($"{playerName} wins with horizontal line {i}!");
            }
        }
        
        for (int j = 0; j < 5; j++)
        {
            if (marked[0, j] && marked[1, j] && marked[2, j] && marked[3, j] && marked[4, j])
            {
                Debug.Log($"{playerName} wins with vertical line {j}!");
            }
        }
        
        if (marked[0, 0] && marked[1, 1] && marked[2, 2] && marked[3, 3] && marked[4, 4])
        {
            Debug.Log($"{playerName} wins with diagonal line!");
        }
        
        if (marked[0, 4] && marked[1, 3] && marked[2, 2] && marked[3, 1] && marked[4, 0])
        {
            Debug.Log($"{playerName} wins with diagonal line!");
        }
    }
}