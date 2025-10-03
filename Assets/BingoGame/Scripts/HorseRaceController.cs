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
    
    private Animator playerAnimator;
    private Animator bot1Animator;
    private Animator bot2Animator;
    
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
        
        if (playerIndex == 0)
        {
            horse = playerHorse;
            animator = playerAnimator;
        }
        else if (playerIndex == 1)
        {
            horse = bot1Horse;
            animator = bot1Animator;
        }
        else if (playerIndex == 2)
        {
            horse = bot2Horse;
            animator = bot2Animator;
        }
        
        if (horse != null && animator != null)
        {
            animator.SetBool("isRunning", true);
            
            Vector3 targetPosition = horse.position + horse.forward * distancePerLine;
            float duration = distancePerLine / horseSpeed;
            
            horse.DOMove(targetPosition, duration).SetEase(Ease.Linear).OnComplete(() =>
            {
                animator.SetBool("isRunning", false);
                onComplete?.Invoke();
            });
        }
        else
        {
            onComplete?.Invoke();
        }
    }
}