using UnityEngine;
using DG.Tweening;

public class HorseRaceController : MonoBehaviour
{
    [Header("Horses")]
    [SerializeField] private Transform playerHorse;
    [SerializeField] private Transform bot1Horse;
    [SerializeField] private Transform bot2Horse;
    
    [Header("Settings")]
    [SerializeField] private float distancePerLine = 5f;
    [SerializeField] private float horseSpeed = 3f;
    [SerializeField] private float finishLineX = 90f;
    [SerializeField] private float finishThreshold = 2f;
    
    private Animator playerAnimator;
    private Animator bot1Animator;
    private Animator bot2Animator;
    
    private bool raceFinished = false;
    
    void Start()
    {
        if (playerHorse != null) playerAnimator = playerHorse.GetComponent<Animator>();
        if (bot1Horse != null) bot1Animator = bot1Horse.GetComponent<Animator>();
        if (bot2Horse != null) bot2Animator = bot2Horse.GetComponent<Animator>();
    }
    
    public void MoveHorse(int playerIndex, System.Action onComplete)
    {
        Transform horse = null;
        Animator animator = null;
        string playerName = "";
    
        if (playerIndex == 0)
        {
            horse = playerHorse;
            animator = playerAnimator;
            playerName = "Player";
        }
        else if (playerIndex == 1)
        {
            horse = bot1Horse;
            animator = bot1Animator;
            playerName = "Bot 1";
        }
        else if (playerIndex == 2)
        {
            horse = bot2Horse;
            animator = bot2Animator;
            playerName = "Bot 2";
        }
    
        if (horse != null && animator != null)
        {
            Debug.Log($"[{playerName}] Starting move from X: {horse.position.x}");
        
            animator.SetBool("isRunning", true);
        
            float distance = distancePerLine;
            if (BoostsManager.Instance.HasSprintBoost)
            {
                distance *= BoostsManager.Instance.sprintMultiplier;
            }
        
            Vector3 targetPosition = horse.position + horse.forward * distance;
            float duration = distance / horseSpeed;
        
            Debug.Log($"[{playerName}] Target position X: {targetPosition.x}, Distance: {distance}");
        
            horse.DOMove(targetPosition, duration).SetEase(Ease.Linear).OnComplete(() =>
            {
                animator.SetBool("isRunning", false);
            
                float currentX = horse.position.x;
                Debug.Log($"[{playerName}] Finished move at X: {currentX}, Finish line: {finishLineX}, Race finished: {raceFinished}");
            
                if (!raceFinished && currentX <= finishLineX)
                {
                    raceFinished = true;
                    Debug.Log($"🏆 {playerName} wins the race! Position X: {currentX}");
                }
            
                onComplete?.Invoke();
            });
        }
        else
        {
            onComplete?.Invoke();
        }
    }
    
    public void ResetRace()
    {
        raceFinished = false;
        Debug.Log("Race reset!");
    }
}