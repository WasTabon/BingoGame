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
    
    [Header("Turn Notification")]
    [SerializeField] private CanvasGroup turnNotificationPanel;
    [SerializeField] private RectTransform turnNotificationImage;
    [SerializeField] private TextMeshProUGUI turnNotificationText;
    
    [Header("Race Camera")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Camera raceCamera;
    [SerializeField] private CanvasGroup mainCanvasGroup;
    
    [Header("Race Settings")]
    [SerializeField] private HorseRaceController horseRaceController;
    [SerializeField] private float cameraFadeDuration = 1f;
    [SerializeField] private float fovChangeDuration = 1.5f;
    [SerializeField] private float startFOV = 70f;
    [SerializeField] private float raceFOV = 30f;
    
    [Header("Game Mode")]
    [SerializeField] private bool twoPlayersOneBot = false;
    [SerializeField] private bool threePlayers = false;
    
    [Header("Settings")]
    [SerializeField] private float rotationDuration = 1.5f;
    [SerializeField] private float fadeDuration = 0.8f;
    [SerializeField] private float notificationDuration = 0.3f;
    [SerializeField] private float cardFadeDuration = 0.3f;
    [SerializeField] private float buttonStaggerDelay = 0.1f;
    [SerializeField] private Color strikethroughColor = Color.red;
    [SerializeField] private Color inactiveColor = new Color(0.5f, 0.5f, 0.5f, 1f);
    [SerializeField] private Color activeColor = Color.white;
    [SerializeField] private Color highlightColor = Color.yellow;

    private int totalPlayers = 3;
    private int humanPlayers = 1;
    
    private int[,] playerNumbers = new int[5, 5];
    private int[,] bot1Numbers = new int[5, 5];
    private int[,] bot2Numbers = new int[5, 5];
    
    private bool[,] playerMarked = new bool[5, 5];
    private bool[,] bot1Marked = new bool[5, 5];
    private bool[,] bot2Marked = new bool[5, 5];
    
    private List<TextMeshProUGUI> playerTexts = new List<TextMeshProUGUI>();
    private List<TextMeshProUGUI> bot1Texts = new List<TextMeshProUGUI>();
    private List<TextMeshProUGUI> bot2Texts = new List<TextMeshProUGUI>();
    
    private List<Image> playerImages = new List<Image>();
    private List<Image> bot1Images = new List<Image>();
    private List<Image> bot2Images = new List<Image>();
    
    private List<int> availableNumbers = new List<int>();
    private int[] currentChoices = new int[3];
    
    private int currentPlayer = 0;
    private bool canClick = false;
    private bool isRaceMode = false;
    
    private Vector2 notificationHiddenPosition;
    private Vector2 notificationVisiblePosition;

    void Start()
    {
        DetermineGameMode();
        InitializeNumberPool();
        CollectAllComponents();
        GenerateAllBingoCards();
        SetupChoiceButtons();
        InitializeTurnNotification();
        InitializeRaceCamera();
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

    void CollectAllComponents()
    {
        TextMeshProUGUI[] playerTMPs = playerBingoParent.GetComponentsInChildren<TextMeshProUGUI>();
        TextMeshProUGUI[] bot1TMPs = bot1BingoParent.GetComponentsInChildren<TextMeshProUGUI>();
        TextMeshProUGUI[] bot2TMPs = bot2BingoParent.GetComponentsInChildren<TextMeshProUGUI>();
        
        Image[] playerImgs = playerBingoParent.GetComponentsInChildren<Image>();
        Image[] bot1Imgs = bot1BingoParent.GetComponentsInChildren<Image>();
        Image[] bot2Imgs = bot2BingoParent.GetComponentsInChildren<Image>();
        
        playerTexts.AddRange(playerTMPs);
        bot1Texts.AddRange(bot1TMPs);
        bot2Texts.AddRange(bot2TMPs);
        
        playerImages.AddRange(playerImgs);
        bot1Images.AddRange(bot1Imgs);
        bot2Images.AddRange(bot2Imgs);
    }
    
    void DetermineGameMode()
    {
        if (threePlayers)
        {
            totalPlayers = 3;
            humanPlayers = 3;
        }
        else if (twoPlayersOneBot)
        {
            totalPlayers = 3;
            humanPlayers = 2;
        }
        else
        {
            totalPlayers = 3;
            humanPlayers = 1;
        }
    
        Debug.Log($"Game Mode: {humanPlayers} human player(s), {totalPlayers - humanPlayers} bot(s)");
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

    void InitializeTurnNotification()
    {
        notificationVisiblePosition = turnNotificationImage.anchoredPosition;
        notificationHiddenPosition = new Vector2(-Screen.width - turnNotificationImage.rect.width, notificationVisiblePosition.y);
        
        turnNotificationImage.anchoredPosition = notificationHiddenPosition;
        turnNotificationPanel.alpha = 0;
        turnNotificationPanel.gameObject.SetActive(false);
    }

    void InitializeRaceCamera()
    {
        if (raceCamera != null)
        {
            raceCamera.gameObject.SetActive(false);
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
        UpdateCardColors();
        ShowTurnNotification();
    }

    void UpdateCardColors()
    {
        if (currentPlayer == 0)
        {
            SetCardActive(playerBingoParent, playerImages, playerTexts, true);
            SetCardActive(bot1BingoParent, bot1Images, bot1Texts, false);
            SetCardActive(bot2BingoParent, bot2Images, bot2Texts, false);
        }
        else if (currentPlayer == 1)
        {
            SetCardActive(playerBingoParent, playerImages, playerTexts, false);
            SetCardActive(bot1BingoParent, bot1Images, bot1Texts, true);
            SetCardActive(bot2BingoParent, bot2Images, bot2Texts, false);
        }
        else
        {
            SetCardActive(playerBingoParent, playerImages, playerTexts, false);
            SetCardActive(bot1BingoParent, bot1Images, bot1Texts, false);
            SetCardActive(bot2BingoParent, bot2Images, bot2Texts, true);
        }
    }

    void SetCardActive(RectTransform card, List<Image> images, List<TextMeshProUGUI> texts, bool isActive)
    {
        Color targetColor = isActive ? activeColor : inactiveColor;
        float targetScale = isActive ? 1.05f : 0.95f;
        
        card.DOScale(targetScale, cardFadeDuration).SetEase(Ease.OutBack);
        
        foreach (Image img in images)
        {
            img.DOColor(targetColor, cardFadeDuration);
        }
        
        if (isActive)
        {
            card.DOShakeRotation(0.5f, 2f, 10, 90f).SetDelay(cardFadeDuration);
        }
    }

    void ShowTurnNotification()
    {
        string turnText = "";
        if (currentPlayer < humanPlayers)
        {
            turnText = humanPlayers == 1 ? "Your turn" : $"Player {currentPlayer + 1}'s turn";
        }
        else
        {
            int botNumber = currentPlayer - humanPlayers + 1;
            turnText = totalPlayers - humanPlayers == 1 ? "Opponent's turn" : $"Opponent {botNumber}'s turn";
        }
    
        turnNotificationText.text = turnText;
        turnNotificationPanel.gameObject.SetActive(true);
    
        Sequence sequence = DOTween.Sequence();
        sequence.Append(turnNotificationPanel.DOFade(1, notificationDuration));
        sequence.Join(turnNotificationImage.DOAnchorPos(notificationVisiblePosition, notificationDuration).SetEase(Ease.OutBack));
        sequence.AppendInterval(0.5f);
        sequence.Append(turnNotificationImage.DOAnchorPos(notificationHiddenPosition, notificationDuration).SetEase(Ease.InBack));
        sequence.Join(turnNotificationPanel.DOFade(0, notificationDuration));
        sequence.OnComplete(() => 
        {
            turnNotificationPanel.gameObject.SetActive(false);
            EnablePlayerTurn();
        });
    }

    void EnablePlayerTurn()
    {
        if (currentPlayer < humanPlayers)
        {
            canClick = true;
            EnableButtons(true);
        }
        else
        {
            canClick = false;
            EnableButtons(false);
            Invoke("BotMakeChoice", 0.8f);
        }
    }

    void GenerateChoices()
{
    Sequence buttonSequence = DOTween.Sequence();

    List<int> validChoices = new List<int>();
    
    // Визначаємо з якої карточки брати числа в залежності від поточного гравця
    int[,] currentPlayerNumbers;
    bool[,] currentPlayerMarked;
    
    if (currentPlayer == 0)
    {
        currentPlayerNumbers = playerNumbers;
        currentPlayerMarked = playerMarked;
    }
    else if (currentPlayer == 1)
    {
        currentPlayerNumbers = bot1Numbers;
        currentPlayerMarked = bot1Marked;
    }
    else
    {
        currentPlayerNumbers = bot2Numbers;
        currentPlayerMarked = bot2Marked;
    }
    
    // Якщо це хід людини, додаємо гарантоване число з його карточки
    if (currentPlayer < humanPlayers)
    {
        for (int i = 0; i < currentPlayerNumbers.GetLength(0); i++)
        {
            for (int j = 0; j < currentPlayerNumbers.GetLength(1); j++)
            {
                int number = currentPlayerNumbers[i, j];
                if (!currentPlayerMarked[i, j] && availableNumbers.Contains(number))
                {
                    validChoices.Add(number);
                }
            }
        }
    }

    if (validChoices.Count > 0 && currentPlayer < humanPlayers)
    {
        int guaranteedNumber = validChoices[Random.Range(0, validChoices.Count)];
        currentChoices[0] = guaranteedNumber;
    
        for (int i = 1; i < 3; i++)
        {
            int randomIndex = Random.Range(0, availableNumbers.Count);
            currentChoices[i] = availableNumbers[randomIndex];
        }
    
        for (int i = currentChoices.Length - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            int temp = currentChoices[i];
            currentChoices[i] = currentChoices[j];
            currentChoices[j] = temp;
        }
    }
    else
    {
        for (int i = 0; i < 3; i++)
        {
            int randomIndex = Random.Range(0, availableNumbers.Count);
            currentChoices[i] = availableNumbers[randomIndex];
        }
    }

    for (int i = 0; i < 3; i++)
    {
        TextMeshProUGUI buttonText = choiceButtons[i].GetComponentInChildren<TextMeshProUGUI>();
        if (buttonText != null)
        {
            buttonText.text = currentChoices[i].ToString();
        }
    
        RectTransform buttonRect = choiceButtons[i].GetComponent<RectTransform>();
        Vector2 originalPos = buttonRect.anchoredPosition;
        buttonRect.anchoredPosition = new Vector2(originalPos.x, originalPos.y + 200f);
    
        int index = i;
        buttonSequence.Insert(index * buttonStaggerDelay, 
            buttonRect.DOAnchorPos(originalPos, 0.4f).SetEase(Ease.OutBounce));
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
    
        canClick = false;
    
        RectTransform buttonRect = choiceButtons[buttonIndex].GetComponent<RectTransform>();
        Sequence buttonAnim = DOTween.Sequence();
        buttonAnim.Append(buttonRect.DOPunchScale(Vector3.one * 0.2f, 0.3f, 5, 0.5f));
        buttonAnim.OnComplete(() =>
        {
            int chosenNumber = currentChoices[buttonIndex];
            ProcessChoice(chosenNumber, currentPlayer); // Змінено з 0 на currentPlayer
        });
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
        
        RectTransform buttonRect = choiceButtons[bestChoice].GetComponent<RectTransform>();
        Image buttonImage = choiceButtons[bestChoice].GetComponent<Image>();
        
        Sequence botAnim = DOTween.Sequence();
        botAnim.Append(buttonImage.DOColor(highlightColor, 0.2f));
        botAnim.Append(buttonRect.DOPunchScale(Vector3.one * 0.15f, 0.3f, 5, 0.5f));
        botAnim.Append(buttonImage.DOColor(Color.white, 0.2f));
        botAnim.OnComplete(() =>
        {
            int chosenNumber = currentChoices[bestChoice];
            ProcessChoice(chosenNumber, botIndex);
        });
    }

    [ContextMenu("Simulate Player Win Line")]
    void SimulatePlayerWinLine()
    {
        SimulateWinLineForPlayer(playerMarked, playerTexts, playerBingoParent, "Player");
    }

    [ContextMenu("Simulate Bot 1 Win Line")]
    void SimulateBot1WinLine()
    {
        SimulateWinLineForPlayer(bot1Marked, bot1Texts, bot1BingoParent, "Bot 1");
    }

    [ContextMenu("Simulate Bot 2 Win Line")]
    void SimulateBot2WinLine()
    {
        SimulateWinLineForPlayer(bot2Marked, bot2Texts, bot2BingoParent, "Bot 2");
    }

    void SimulateWinLineForPlayer(bool[,] marked, List<TextMeshProUGUI> texts, RectTransform cardParent, string playerName)
    {
        int lineToComplete = -1;
    
        for (int i = 0; i < 5; i++)
        {
            bool allMarked = true;
            for (int j = 0; j < 5; j++)
            {
                if (!marked[i, j])
                {
                    allMarked = false;
                    break;
                }
            }
        
            if (!allMarked)
            {
                lineToComplete = i;
                break;
            }
        }
    
        if (lineToComplete != -1)
        {
            for (int j = 0; j < 5; j++)
            {
                if (!marked[lineToComplete, j])
                {
                    marked[lineToComplete, j] = true;
                    int cellIndex = lineToComplete * 5 + j;
                    if (cellIndex < texts.Count)
                    {
                        TextMeshProUGUI text = texts[cellIndex];
                        Transform cellTransform = text.transform.parent;
                    
                        Sequence markSequence = DOTween.Sequence();
                        markSequence.Append(cellTransform.DOPunchScale(Vector3.one * 0.3f, 0.4f, 8, 0.5f));
                        markSequence.Join(cellTransform.DORotate(new Vector3(0, 0, 360), 0.5f, RotateMode.FastBeyond360));
                        markSequence.Join(text.DOColor(strikethroughColor, cardFadeDuration));
                    }
                }
            }
        
            DOVirtual.DelayedCall(0.6f, () => CheckWin(marked, texts, cardParent, playerName));
        }
    }
    
    void ProcessChoice(int number, int playerIndex)
    {
        availableNumbers.Remove(number);
    
        string playerName = playerIndex < humanPlayers ? $"Player {playerIndex + 1}" : $"Bot {playerIndex - humanPlayers + 1}";
    
        if (playerIndex == 0)
        {
            MarkNumber(playerNumbers, playerMarked, playerTexts, playerBingoParent, number);
            CheckWin(playerMarked, playerTexts, playerBingoParent, playerName);
        }
        else if (playerIndex == 1)
        {
            MarkNumber(bot1Numbers, bot1Marked, bot1Texts, bot1BingoParent, number);
            CheckWin(bot1Marked, bot1Texts, bot1BingoParent, playerName);
        }
        else if (playerIndex == 2)
        {
            MarkNumber(bot2Numbers, bot2Marked, bot2Texts, bot2BingoParent, number);
            CheckWin(bot2Marked, bot2Texts, bot2BingoParent, playerName);
        }
    
        if (!isRaceMode)
        {
            currentPlayer = (currentPlayer + 1) % totalPlayers;
            Invoke("StartNewRound", 0.3f);
        }
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

    void MarkNumber(int[,] numbers, bool[,] marked, List<TextMeshProUGUI> texts, RectTransform cardParent, int targetNumber)
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
                        TextMeshProUGUI text = texts[cellIndex];
                        Transform cellTransform = text.transform.parent;
                        
                        Sequence markSequence = DOTween.Sequence();
                        markSequence.Append(cellTransform.DOPunchScale(Vector3.one * 0.3f, 0.4f, 8, 0.5f));
                        markSequence.Join(cellTransform.DORotate(new Vector3(0, 0, 360), 0.5f, RotateMode.FastBeyond360));
                        markSequence.Join(text.DOColor(strikethroughColor, cardFadeDuration));
                    }
                    return;
                }
            }
        }
    }

    void CheckWin(bool[,] marked, List<TextMeshProUGUI> texts, RectTransform cardParent, string playerName)
    {
        List<int> winningIndices = new List<int>();
        int linesCompleted = 0;
        
        for (int i = 0; i < 5; i++)
        {
            if (marked[i, 0] && marked[i, 1] && marked[i, 2] && marked[i, 3] && marked[i, 4])
            {
                Debug.Log($"{playerName} wins with horizontal line {i}!");
                linesCompleted++;
                for (int j = 0; j < 5; j++)
                {
                    winningIndices.Add(i * 5 + j);
                }
            }
        }
        
        for (int j = 0; j < 5; j++)
        {
            if (marked[0, j] && marked[1, j] && marked[2, j] && marked[3, j] && marked[4, j])
            {
                Debug.Log($"{playerName} wins with vertical line {j}!");
                linesCompleted++;
                for (int i = 0; i < 5; i++)
                {
                    winningIndices.Add(i * 5 + j);
                }
            }
        }
        
        if (marked[0, 0] && marked[1, 1] && marked[2, 2] && marked[3, 3] && marked[4, 4])
        {
            Debug.Log($"{playerName} wins with diagonal line!");
            linesCompleted++;
            for (int i = 0; i < 5; i++)
            {
                winningIndices.Add(i * 5 + i);
            }
        }
        
        if (marked[0, 4] && marked[1, 3] && marked[2, 2] && marked[3, 1] && marked[4, 0])
        {
            Debug.Log($"{playerName} wins with diagonal line!");
            linesCompleted++;
            for (int i = 0; i < 5; i++)
            {
                winningIndices.Add(i * 5 + (4 - i));
            }
        }
        
        if (winningIndices.Count > 0)
        {
            HighlightWinningLine(texts, winningIndices);
            
            if (linesCompleted > 0)
            {
                int winnerIndex = GetPlayerIndex(playerName);
                StartRaceSequence(winnerIndex);
            }
        }
    }

    int GetPlayerIndex(string playerName)
    {
        if (playerName == "Player") return 0;
        if (playerName == "Bot 1") return 1;
        if (playerName == "Bot 2") return 2;
        return 0;
    }

    void StartRaceSequence(int winnerIndex)
    {
        if (isRaceMode) return;
        
        isRaceMode = true;
        CancelInvoke("StartNewRound");
        
        Sequence raceSequence = DOTween.Sequence();
        
        raceSequence.Append(mainCanvasGroup.DOFade(0, cameraFadeDuration));
        
        raceSequence.AppendCallback(() =>
        {
            mainCanvasGroup.gameObject.SetActive(false);
            mainCamera.gameObject.SetActive(false);
            raceCamera.gameObject.SetActive(true);
        });
        
        raceSequence.Append(DOVirtual.Float(0, 1, cameraFadeDuration, value => {}));
        
        raceSequence.Append(DOVirtual.Float(startFOV, raceFOV, fovChangeDuration, value => raceCamera.fieldOfView = value));
        
        raceSequence.AppendCallback(() =>
        {
            horseRaceController.MoveHorse(winnerIndex, () =>
            {
                ReturnToGame();
            });
        });
    }

    void ReturnToGame()
    {
        Sequence returnSequence = DOTween.Sequence();
        
        returnSequence.Append(DOVirtual.Float(raceFOV, startFOV, fovChangeDuration, value => raceCamera.fieldOfView = value));
        
        returnSequence.Append(DOVirtual.Float(0, 1, cameraFadeDuration, value => {}));
        
        returnSequence.AppendCallback(() =>
        {
            raceCamera.gameObject.SetActive(false);
            mainCamera.gameObject.SetActive(true);
            mainCanvasGroup.gameObject.SetActive(true);
        });
        
        returnSequence.Append(mainCanvasGroup.DOFade(1, cameraFadeDuration));
        
        returnSequence.OnComplete(() =>
        {
            isRaceMode = false;
            currentPlayer = (currentPlayer + 1) % 3;
            Invoke("StartNewRound", 0.3f);
        });
    }

    void HighlightWinningLine(List<TextMeshProUGUI> texts, List<int> indices)
    {
        foreach (int index in indices)
        {
            if (index < texts.Count)
            {
                Transform cellTransform = texts[index].transform.parent;
                cellTransform.DOPunchScale(Vector3.one * 0.5f, 0.6f, 10, 1f).SetLoops(3);
            }
        }
    }
}