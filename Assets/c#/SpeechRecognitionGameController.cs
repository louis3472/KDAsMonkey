using UnityEngine;
using UnityEngine.UI;
using Microsoft.CognitiveServices.Speech;
using System.Collections;

public class SpeechRecognitionGameController : MonoBehaviour
{
    public GameObject player;
    public GameObject blockModel1;
    public GameObject blockModel2;
    public Text countdownText;
    public Text scoreText;

    private string subscriptionKey = "b0833dc754454631b983891a13a012ed";
    private string region = "eastasia";
    private SpeechRecognizer recognizer;
    private int totalScore;
    private int round;
    private float roundTime = 180f;
    private float timeRemaining;
    private bool isRecognizing;

    private float minInterval = 5f;
    private float timeSinceLastToggle;

    private void Start()
    {
        round = 1;
        totalScore = 0;
        timeRemaining = roundTime;
        UpdateScoreText();

        StartCoroutine(RoundLoop());

        var config = SpeechConfig.FromSubscription(subscriptionKey, region);
        recognizer = new SpeechRecognizer(config);
        recognizer.Recognizing += OnPhraseRecognizing;
        recognizer.StartContinuousRecognitionAsync();

        timeSinceLastToggle = minInterval;
    }

    private IEnumerator RoundLoop()
    {
        while (round <= 5)
        {
            timeRemaining = roundTime;
            isRecognizing = true;

            while (timeRemaining > 0)
            {
                UpdateCountdownText();
                timeRemaining -= Time.deltaTime;
                timeSinceLastToggle += Time.deltaTime;

                if (timeSinceLastToggle >= minInterval)
                {
                    ToggleBlockModels();
                    timeSinceLastToggle = 0;
                }

                yield return null;
            }

            isRecognizing = false;
            round++;
        }

        // 结束游戏，返回主画面并重置分数
        // SceneManager.LoadScene("MainMenu");
        totalScore = 0;
    }

    private void Update()
    {
        // 添加键盘输入以进行测试
        if (blockModel1.activeInHierarchy)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                totalScore += 5;
                UpdateScoreText();
            }
        }
        else if (blockModel2.activeInHierarchy)
        {
            if (Input.GetKeyDown(KeyCode.G))
            {
                totalScore--;
                UpdateScoreText();
            }
        }
    }

    private void ToggleBlockModels()
    {
        if (blockModel1.activeInHierarchy)
        {
            blockModel1.SetActive(false);
            blockModel2.SetActive(true);
        }
        else
        {
            blockModel1.SetActive(true);
            blockModel2.SetActive(false);
        }
    }

    private void UpdateCountdownText()
    {
        countdownText.text = $"Time Remaining: {Mathf.FloorToInt(timeRemaining)}";
    }

    private void UpdateScoreText()
    {
        scoreText.text = $"Score: {totalScore}";
    }

    private void OnDestroy()
    {
        recognizer.StopContinuousRecognitionAsync();
    }

    private void OnPhraseRecognizing(object sender, SpeechRecognitionEventArgs args)
    {
        string recognizedText = args.Result.Text;

        if (recognizedText.Contains("fuck"))
        {
            Debug.Log("Detected 'fuck'");
            if (blockModel1.activeInHierarchy)
            {
                totalScore += 5;
                UpdateScoreText();
            }
        }
        else if (recognizedText.Contains("god"))
        {
            Debug.Log("Detected 'god'");
            if (blockModel2.activeInHierarchy)
            {
                totalScore--;
                UpdateScoreText();
            }
        }
    }
}
