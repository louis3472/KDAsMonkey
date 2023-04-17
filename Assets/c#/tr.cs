using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;
using UnityEngine.Windows.Speech;
using System.Collections.Generic;
using System.Text;
using System;
using Random = UnityEngine.Random;

public class tr: MonoBehaviour
{
    public GameObject player;
    public GameObject npc;
    public GameObject cubePrefab;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI scoreText;
    private float gameTime = 60;
    private float timeRemaining;
    private int score = 0;
    private bool cubeActive = false;

    [SerializeField]
    private string[] m_Keywords;

    [SerializeField]
    private Text m_uiText;

    [SerializeField]
    private Transform m_character;

    private KeywordRecognizer m_Recognizer;
    private Dictionary<string, Action> m_actionMap = new Dictionary<string, Action>();
    void Start()
    {
        timeRemaining = gameTime;
        StartCoroutine(SpawnCube());

        m_actionMap.Add("up", Up);
        m_Recognizer = new KeywordRecognizer(m_Keywords);
        m_Recognizer.OnPhraseRecognized += OnPhraseRecognized;
        m_Recognizer.Start();
    }

    void Update()
    {
        UpdateTimer();
        UpdateScore();
    }

    IEnumerator SpawnCube()
    {
        while (timeRemaining > 0)
        {
            float spawnDelay = Random.Range(5, 10);
            yield return new WaitForSeconds(spawnDelay);

            Vector3 spawnPosition = new Vector3(Random.Range(-10, 10), 1, Random.Range(-10, 10));
            GameObject cube = Instantiate(cubePrefab, spawnPosition, Quaternion.identity);
            cubeActive = true;

            yield return new WaitForSeconds(3);

            if (cube != null)
            {
                Destroy(cube);
            }
            cubeActive = false;
        }
    }
    private void OnPhraseRecognized(PhraseRecognizedEventArgs args)
    {
        var builder = new StringBuilder();
        builder.AppendFormat("{0} ({1}){2}", args.text, args.confidence, Environment.NewLine);
        builder.AppendFormat("\tTimestamp: {0}{1}", args.phraseStartTime, Environment.NewLine);
        builder.AppendFormat("\tDuration: {0} seconds{1}", args.phraseDuration.TotalSeconds, Environment.NewLine);
        Debug.Log(builder.ToString());
        m_uiText.text = $"You said : {args.text}";

        m_actionMap[args.text].Invoke();
    }

    private void Up()
    {
        m_character.Translate(0, 2, 0);
    }
    public void OnVoiceDetected()
    {
        if (cubeActive)
        {
            score += 1;
        }
        else
        {
            score -= 1;
        }
    }

    void UpdateTimer()
    {
        timeRemaining -= Time.deltaTime;
        timerText.text = Mathf.Floor(timeRemaining).ToString();
    }

    void UpdateScore()
    {
        scoreText.text = score.ToString();
    }

    void EndGame()
    {
        // Go back to the main menu and reset the score
        SceneManager.LoadScene("MainMenu");
    }
}