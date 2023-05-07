using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;
using UnityEngine.Windows.Speech;
using System.Linq;
using System.Collections.Generic;

public class ne : MonoBehaviour
{
    public GameObject player;
    public GameObject npc;
    public GameObject npc_move;
    public GameObject npc_end;
    public GameObject phonePrefab;
    private GameObject phoneInstance;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI warningText; // �K�[ĵ�i���ƪ�UI�奻�ܶq
    private int warningCount = 0; // �K�[ĵ�i���ƭp�ƾ�
    private float gameTime = 180;
    private float timeRemaining;
    private int score = 0;
    private int currentRound = 1;
    private bool cubeActive = false;
    private int keyboard2Counter = 0;
    private bool npcEndSpawned = false;
    private GameObject currentCube;
    private int warningCounter = 0;
    private KeywordRecognizer keywordRecognizer;
    private Dictionary<string, System.Action> keywords = new Dictionary<string, System.Action>();



    void Start()
    {
        {
            timeRemaining = gameTime;
            StartCoroutine(SpawnCube());
            npc_end.SetActive(false);
            keywords.Add("talk", OnTalkDetected);
            keywords.Add("phone", OnPhoneDetected);
            keywords.Add("stop", OnStopDetected);

            keywordRecognizer = new KeywordRecognizer(keywords.Keys.ToArray());
            keywordRecognizer.OnPhraseRecognized += KeywordRecognizer_OnPhraseRecognized;
            keywordRecognizer.Start();

            warningText.text = $"ĵ�i����: {warningCount}"; // ��l��ĵ�i����UI�奻

        }
    }
    public void OnReDetected()
    {
        score += 10;
    }
    void Update()
    {
        UpdateWarningText();

        UpdateTimer();
        UpdateScore();
        TestScoreWithKeyboardInput();

        if ((keyboard2Counter >= 50 || score <= -100) && !npcEndSpawned)
        {
            SpawnNpcEnd();
        }
        if (warningCount >= 3)
        {
            SpawnNpcEnd();
        }

    }
    void UpdateWarningText()
    {
        warningText.text = $"ĵ�i����: {warningCount}";
    }

    IEnumerator SpawnCube()
    {
        while (currentRound <= 5 && !npcEndSpawned)
        {
            // ���� 7 ��� 14 �������H���ɶ�
            yield return new WaitForSeconds(Random.Range(7, 14));

            if (npc != null && currentCube == null) // �T�O�u����e�S��npc_move��Үɤ~�Ыطs��
            {
                npc.SetActive(false);
                Vector3 spawnPosition = npc.transform.position;
                currentCube = Instantiate(npc_move, spawnPosition, Quaternion.identity); // �ϥ� currentCube �Ӧs�x�s�Ыت�npc_move���
                cubeActive = true;

                // ���� 5 ��
                yield return new WaitForSeconds(5);

                if (currentCube != null)
                {
                    Destroy(currentCube);
                    currentCube = null; // ��e��npc_move��Ҥw�Q�P���A�N currentCube �]�m�� null
                }
                cubeActive = false;
                if (!npcEndSpawned)
                {
                    npc.SetActive(true);
                }
            }
        }
        if (!npcEndSpawned)
        {
            EndGame();
        }
    }

    public void OnTalkDetected()
    {
        if (cubeActive)
        {
            score += 5;
        }
        else
        {
            score -= 1;
            warningCount++; // ��sĵ�i����
            if (warningCount >= 3)
            {
                SpawnNpcEnd();
            }
        }
    }
    public void OnPhoneDetected()
    {
        if (phoneInstance == null)
        {
            phoneInstance = Instantiate(phonePrefab, player.transform.position, Quaternion.identity);
            player.SetActive(false);
            if (cubeActive)
            {
                warningCounter++;
            }
            else
            {
                StartCoroutine(ScorePointsAndDestroyPhone());
            }
        }
    }


    IEnumerator ScorePointsAndDestroyPhone()
    {
        for (int i = 0; i < 5; i++)
        {
            score += 2;
            yield return new WaitForSeconds(1);
        }
        DestroyPhoneInstance();
    }

    public void OnStopDetected()
    {
        DestroyPhoneInstance();
    }

    private void KeywordRecognizer_OnPhraseRecognized(PhraseRecognizedEventArgs args)
    {
        System.Action keywordAction;

        if (keywords.TryGetValue(args.text, out keywordAction))
        {
            keywordAction.Invoke();
        }
    }

    void UpdateTimer()
    {
        timeRemaining -= Time.deltaTime;
        timerText.text = Mathf.Floor(timeRemaining).ToString();

        if (timeRemaining <= 0)
        {
            currentRound++;
            scoreText.text = score.ToString();

            if (currentRound <= 5)
            {
                timeRemaining = gameTime;
                StartCoroutine(SpawnCube());
            }
            else
            {
                EndGame();
            }
        }
    }

    void UpdateScore()
    {
        scoreText.text = score.ToString();
    }

    void TestScoreWithKeyboardInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (cubeActive)
            {
                score += 5;
            }
            else
            {
                score -= 1;
                warningCount++; // ��sĵ�i����
                if (warningCount >= 3)
                {
                    SpawnNpcEnd();
                }
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (!cubeActive)
            {
                score -= 1;
                keyboard2Counter++;
            }
            if (cubeActive)
            {
                score -= 5;
                keyboard2Counter++;
            }
            if (keyboard2Counter >= 50 || score <= -100)
            {
                SpawnNpcEnd();
            }
        }
    }
    void SpawnNpcEnd()
    {
        npcEndSpawned = true;
        npc_end.SetActive(true);
        npc_move.SetActive(false);
        npc.SetActive(false);
        StartCoroutine(EndGameAfterDelay(10f));
    }

    IEnumerator EndGameAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        EndGame();
    }

    void EndGame()
    {
        score = 0;
        currentRound = 1;
        SceneManager.LoadScene("ResultsScene");
    }

    private void DestroyPhoneInstance()
    {
        if (phoneInstance != null)
        {
            Destroy(phoneInstance);
            phoneInstance = null;
            player.SetActive(true);
        }
    }
}