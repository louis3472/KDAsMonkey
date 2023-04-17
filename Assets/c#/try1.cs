using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;
using UnityEngine.Windows.Speech;
using System.Linq;
using System.Collections.Generic;

public class try1 : MonoBehaviour
{
    public GameObject player;
    public GameObject npc;
    public GameObject cubePrefab;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI scoreText;
    private float gameTime = 30;
    private float timeRemaining;
    private int score = 0;
    private int currentRound = 1;
    private bool cubeActive = false;

    private KeywordRecognizer keywordRecognizer;
    private Dictionary<string, System.Action> keywords = new Dictionary<string, System.Action>();

    void Start()
    {
        timeRemaining = gameTime;
        StartCoroutine(SpawnCube());
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
    
    }

    IEnumerator SpawnCube()
    {
        while (currentRound <= 5)
        {
            yield return new WaitForSeconds(Random.Range(5, 10));

            if (npc != null)
            {
                Vector3 spawnPosition = npc.transform.position;
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
        EndGame();
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

    void EndGame()
    {
        score = 0;
        currentRound = 1;
        SceneManager.LoadScene("MainMenu");
    }
}
