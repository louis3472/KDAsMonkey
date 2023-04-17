using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Microsoft.CognitiveServices.Speech;

public class GameController : MonoBehaviour
{
    public GameObject player;
    public GameObject npc1;
    public GameObject npc2;
    public GameObject block1;
    public GameObject block2;
    public Text timerText;
    public Text scoreText;
    public int roundCount;
    public int maxRounds = 5;
    public float roundTime = 180f;
    public int score = 0;
    private float spawnTimer;
    private float penaltyTimer;
    private string subscriptionKey = "b0833dc754454631b983891a13a012ed";
    private string region = "eastasia";
    private SpeechRecognizer recognizer;

    private async void Start()
    {
        roundCount = 1;
        spawnTimer = 2f;
        penaltyTimer = 0f;

        var config = SpeechConfig.FromSubscription(subscriptionKey, region);
        recognizer = new SpeechRecognizer(config);
        recognizer.Recognizing += OnPhraseRecognizing;
        await recognizer.StartContinuousRecognitionAsync();
    }

    private void Update()
    {
        if (roundCount > maxRounds)
        {
            EndGame();
            return;
        }

        roundTime -= Time.deltaTime;
        timerText.text = Mathf.Round(roundTime).ToString();
        scoreText.text = score.ToString();

        if (roundTime <= 0)
        {
            NextRound();
        }

        spawnTimer -= Time.deltaTime;
        if (spawnTimer <= 0)
        {
            StartCoroutine(SpawnBlock1());
        }
    }

    private void OnPhraseRecognizing(object sender, SpeechRecognitionEventArgs args)
    {
        if (args.Result.Text == "fuck" && block1.activeInHierarchy)
        {
            score++;
        }
        else if ((args.Result.Text == "fuck" || args.Result.Text == "shit") && block2.activeInHierarchy)
        {
            penaltyTimer = 1f;
        }
    }

    IEnumerator SpawnBlock1()
    {
        block2.SetActive(false);
        block1.SetActive(true);
        float spawnDuration = Random.Range(2f, 4f);
        yield return new WaitForSeconds(spawnDuration);
        block1.SetActive(false);
        block2.SetActive(true);
        spawnTimer = 2f;
    }

    private void NextRound()
    {
        roundCount++;
        roundTime = 180f;
    }

    private async void EndGame()
    {
        await recognizer.StopContinuousRecognitionAsync();
        SceneManager.LoadScene("MainMenu");
    }
}
