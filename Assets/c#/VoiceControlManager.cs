using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class VoiceControlManager : MonoBehaviour
{
    public GameObject player;
    public GameObject npc;
    public Animator npcAnimator;
    public TextMeshProUGUI timerText;
    public int gameRounds = 5;
    private int currentRound = 1;
    private int score = 0;
    private float timeRemaining = 180;

    void Start()
    {
        StartCoroutine(GameLoop());
    }

    void Update()
    {
        UpdateTimer();
    }

    IEnumerator GameLoop()
    {
        while (currentRound <= gameRounds)
        {
            yield return StartCoroutine(PlayRound());
            currentRound++;
        }

        EndGame();
    }

    IEnumerator PlayRound()
    {
        // Round start

        // Initialize round variables
        timeRemaining = 180;
        bool npcLookingBack = false;

        while (timeRemaining > 0)
        {
            // Randomly make the NPC look back or pretend to look back
            int random = Random.Range(0, 2);
            if (random == 0)
            {
                npcAnimator.SetTrigger("LookBack");
                npcLookingBack = true;
            }
            else
            {
                npcAnimator.SetTrigger("PretendLookBack");
                npcLookingBack = false;
            }

            // Wait for 2 seconds
            yield return new WaitForSeconds(2);

            // If the player made a sound while the NPC was looking back
            if (/* player made sound  && */ npcLookingBack)
            {
                score += 5;
            }

            // If the player made a sound while the NPC was facing the blackboard
            if (/* player made sound  && */ !npcLookingBack)
            {
                score -= 1;
            }

            npcAnimator.SetTrigger("FaceBlackboard");

            // Wait for a random time before the next action
            yield return new WaitForSeconds(Random.Range(2, 5));
        }

        // Round end
    }

    void UpdateTimer()
    {
        timeRemaining -= Time.deltaTime;
        timerText.text = Mathf.Floor(timeRemaining).ToString();
    }

    void EndGame()
    {
        // Go back to the main menu and reset the score
        SceneManager.LoadScene("MainMenu");
    }
}