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
private string microphoneDeviceName;


private async void Start()
{
    // 获取麦克风设备名称
    string[] devices = Microphone.devices;
    if (devices.Length > 0)
    {
        microphoneDeviceName = devices[0]; // 使用第一个检测到的麦克风设备
        Debug.Log("Using microphone device: " + microphoneDeviceName);
    }
    else
    {
        Debug.Log("No microphone device detected.");
    }

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
Debug.Log("Recognized phrase: " + args.Result.Text); // 將辨識結果顯示在console上
if (args.Result.Text == "yes" && block1.activeInHierarchy)
{
score++;
if (score >= 5) // 判断分数是否到达指定值
{
SceneManager.LoadScene("MainMenu"); // 切换场景
}
}
else if ((args.Result.Text == "yes" || args.Result.Text == "no") && block2.activeInHierarchy)
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
    score = 0; // 重置分数
}

private async void EndGame()
{
    await recognizer.StopContinuousRecognitionAsync();
    SceneManager.LoadScene("MainMenu");
}


}

