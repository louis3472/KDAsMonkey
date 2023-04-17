using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;
using UnityEngine.Windows.Speech;
using System.Linq;
using System.Collections.Generic;

public class tryy : MonoBehaviour
{
    public GameObject player;
    public GameObject npc;
    public GameObject npc_move;
    public GameObject npc_end;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI scoreText;
    private float gameTime = 180;
    private float timeRemaining;
    private int score = 0;
    private int currentRound = 1;
    private bool cubeActive = false;
    private int keyboard2Counter = 0;
    private bool npcEndSpawned = false;
    private GameObject currentCube; // 新增此變量來存儲當前的npc_move實例
    private KeywordRecognizer keywordRecognizer;
    private Dictionary<string, System.Action> keywords = new Dictionary<string, System.Action>();



    void Start()
    {
        timeRemaining = gameTime;
        StartCoroutine(SpawnCube());
        npc_end.SetActive(false);
        timeRemaining = gameTime;
        StartCoroutine(SpawnCube());
        npc_end.SetActive(false);
        keywords.Add("record", OnReDetected);
        keywords.Add("shit", OnWuwuDetected);
        keywords.Add("me", OnMeDetected);
        keywords.Add("read", OnReadDetected);

        keywords.Add("hair", OnAaDetected);
        keywords.Add("phone", OnphoneDetected);
        keywords.Add("sleep", OnsleepDetected);
        keywords.Add("paper", OnpaperDetected);

        keywordRecognizer = new KeywordRecognizer(keywords.Keys.ToArray());
        keywordRecognizer.OnPhraseRecognized += KeywordRecognizer_OnPhraseRecognized;
        keywordRecognizer.Start();
    }
    public void OnReDetected()
    {
        score += 10;
    }
    void Update()
    {
        UpdateTimer();
        UpdateScore();
        TestScoreWithKeyboardInput();

        if ((keyboard2Counter >= 50 || score <= -100) && !npcEndSpawned)
        {
            SpawnNpcEnd();
        }
    }

    IEnumerator SpawnCube()
    {
        while (currentRound <= 5 && !npcEndSpawned)
        {
            // 等待 7 秒至 14 秒之間的隨機時間
            yield return new WaitForSeconds(Random.Range(7, 14));

            if (npc != null && currentCube == null) // 確保只有當前沒有npc_move實例時才創建新的
            {
                npc.SetActive(false);
                Vector3 spawnPosition = npc.transform.position;
                currentCube = Instantiate(npc_move, spawnPosition, Quaternion.identity); // 使用 currentCube 來存儲新創建的npc_move實例
                cubeActive = true;

                // 等待 5 秒
                yield return new WaitForSeconds(5);

                if (currentCube != null)
                {
                    Destroy(currentCube);
                    currentCube = null; // 當前的npc_move實例已被銷毀，將 currentCube 設置為 null
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

    public void OnReadDetected()
    {
        if (cubeActive)
        {
            score += 5;
        }
        else
        {
            score -= 1;
        }
    }
    public void OnMeDetected()
    {
        if (cubeActive)
        {
            score += 5;
        }
        else
        {
            score -= 1;
        }
    }
    public void OnWuwuDetected()
    {
        if (cubeActive)
        {
            score += 5;
        }
        else
        {
            score -= 1;
        }
    }

    public void OnAaDetected()
    {
        if (!cubeActive)
        {
            score -= 1;
        }
    }
    public void OnphoneDetected()
    {
        if (!cubeActive)
        {
            score -= 1;
        }
    }
    public void OnsleepDetected()
    {
        if (!cubeActive)
        {
            score -= 1;
        }
    }
    public void OnpaperDetected()
    {
        if (!cubeActive)
        {
            score -= 1;
        }
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
}