using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;
using UnityEngine.Windows.Speech;
using System.Linq;
using System.Collections.Generic;

public class final : MonoBehaviour
{
    public GameObject player;
    public GameObject npc;
    public GameObject npc_move;
    public GameObject npc_end;
    public GameObject phonePrefab;
    public GameObject monkey;
    public GameObject monkey_move;
    public GameObject commandImage1;
    public GameObject commandImage2;
    public GameObject commandImage3;
    public GameObject noisyImage;
    public GameObject noteImage;
    public GameObject paperImage;
    public GameObject pickImage;
    private GameObject phoneInstance;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI warningText;
    public TextMeshProUGUI suspicionText;
    private int warningCount = 0;
    private int suspicionCount = 0;
    private float gameTime = 180;
    private float timeRemaining;
    private int currentRound = 1;
    private bool cubeActive = false;
    private int keyboard2Counter = 0;
    private bool npcEndSpawned = false;
    private GameObject currentCube;
    private int command1State = 0;
    private int command2State = 0;
    private int command3State = 0;
    private int monkeyKingCurrentCommand = 0;
    private int intelligencePoints = 0;
    private KeywordRecognizer keywordRecognizer;
    private Dictionary<string, System.Action> keywords = new Dictionary<string, System.Action>();

    void Start()
    {
        timeRemaining = gameTime;
        StartCoroutine(SpawnCube());
        StartCoroutine(MonkeyKingCommandsCycle());
        npc_end.SetActive(false);
        monkey.SetActive(true);
        monkey_move.SetActive(false);
        commandImage1.SetActive(false);
        commandImage2.SetActive(false);
        commandImage3.SetActive(false);
        noisyImage.SetActive(false);
        noteImage.SetActive(false);
        paperImage.SetActive(false);
        pickImage.SetActive(false);

        keywords.Add("Noisy", OnNoisyDetected);
        keywords.Add("Note", OnNoteDetected);
        keywords.Add("Paper", OnPaperDetected);
        keywords.Add("pick", OnPickDetected);

        keywordRecognizer = new KeywordRecognizer(keywords.Keys.ToArray());
        keywordRecognizer.OnPhraseRecognized += KeywordRecognizer_OnPhraseRecognized;
        keywordRecognizer.Start();

        warningText.text = $": {warningCount}";
        suspicionText.text = $": {suspicionCount}";
    }

    void Update()
    {
        UpdateWarningText();
        UpdateSuspicionText();
        TestKeyboardCommands();
        UpdateTimer();

        if ((keyboard2Counter >= 50 || intelligencePoints <= -100) && !npcEndSpawned)
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
        warningText.text = $" {warningCount}";
    }

    void UpdateSuspicionText()
    {
        suspicionText.text = $" {suspicionCount}";
    }
    void UpdateTimer()
    {
        gameTime -= Time.deltaTime;
        timerText.text = $" {Mathf.Round(gameTime)}";
        if (gameTime <= 0 && currentRound == 1)
        {
            currentRound++;
            gameTime = 180;
        }
        else if (gameTime <= 0 && currentRound == 2)
        {
            SceneManager.LoadScene("ResultsScene");
        }
    }
    IEnumerator SpawnCube()
    {
        yield return new WaitForSeconds(3);

        while (currentRound <= 2 && !npcEndSpawned)
        {
            if (npc != null && currentCube == null)
            {
                npc.SetActive(false);
                Vector3 spawnPosition = npc.transform.position;
                currentCube = Instantiate(npc_move, spawnPosition, Quaternion.identity);
                cubeActive = true;

                yield return new WaitForSeconds(5);

                if (currentCube != null)
                {
                    Destroy(currentCube);
                    currentCube = null;
                }
                cubeActive = false;
                if (!npcEndSpawned)
                {
                    npc.SetActive(true);
                }
            }

            // 等待 7 到 14 秒後再次生成 npc_move
            yield return new WaitForSeconds(Random.Range(7, 14));
        }
    }
    IEnumerator MonkeyKingCommandsCycle()
    {
        // 遊戲開始後等待 5 秒
        yield return new WaitForSeconds(5);

        while (currentRound <= 2 && !npcEndSpawned)
        {
            // 隨機生成猴王指令
            GenerateMonkeyKingCommand();
            // 15 秒後隱藏猴王指令
            yield return new WaitForSeconds(15);
            HideMonkeyKingCommand();

            // 等待 20 秒後再次生成猴王指令
            yield return new WaitForSeconds(Random.Range(5, 10));
        }
    }

    private void GenerateMonkeyKingCommand()
    {
        monkeyKingCurrentCommand = Random.Range(1, 4);
        monkey.SetActive(false);
        monkey_move.SetActive(true);

        switch (monkeyKingCurrentCommand)
        {
            case 1:
                commandImage1.SetActive(true);
                command1State = 0;
                break;
            case 2:
                commandImage2.SetActive(true);
                command2State = 0;
                break;
            case 3:
                commandImage3.SetActive(true);
                command3State = 0;
                break;
        }

        // 新增這行，15秒後檢查指令完成狀態
        StartCoroutine(CheckCommandCompletionAfterDelay(15f));
    }
    IEnumerator CheckCommandCompletionAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // 如果當前指令仍未完成，增加懷疑次數
        if (monkeyKingCurrentCommand > 0)
        {
            suspicionCount++;
            HideMonkeyKingCommand();
            if (suspicionCount >= 3)
            {
                SceneManager.LoadScene("ResultsScene");
            }
        }
    }

    private void HideMonkeyKingCommand()
    {
        commandImage1.SetActive(false);
        commandImage2.SetActive(false);
        commandImage3.SetActive(false);
        monkey.SetActive(true);
        monkey_move.SetActive(false);
        monkeyKingCurrentCommand = 0;
    }

    public void OnNoisyDetected()
    {
        ShowCommandImage(noisyImage);
        HandleMonkeyKingCommands("Noisy");
    }

    public void OnNoteDetected()
    {
        ShowCommandImage(noteImage);
        HandleMonkeyKingCommands("Note");
    }

    public void OnPaperDetected()
    {
        ShowCommandImage(paperImage);
        HandleMonkeyKingCommands("Paper");
    }

    public void OnPickDetected()
    {
        ShowCommandImage(pickImage);

        if (cubeActive)
        {
            intelligencePoints += 1;
            if (intelligencePoints >= 10 && currentRound == 2)
            {
                SceneManager.LoadScene("HE");
            }
        }
    }

    private void ShowCommandImage(GameObject commandImage)
    {
        commandImage.SetActive(true);
        StartCoroutine(HideCommandImageAfterDelay(commandImage, 1f));
    }

    IEnumerator HideCommandImageAfterDelay(GameObject commandImage, float delay)
    {
        yield return new WaitForSeconds(delay);
        commandImage.SetActive(false);
    }
    void TestKeyboardCommands()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            OnNoisyDetected();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            OnNoteDetected();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            OnPaperDetected();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            OnPickDetected();
        }
    }
    private void HandleMonkeyKingCommands(string command)
    {
        if (monkeyKingCurrentCommand == 0)
        {
            if (cubeActive && (command == "Noisy" || command == "Note" || command == "Paper"))
            {
                warningCount++;
                if (warningCount >= 3)
                {
                    SpawnNpcEnd();
                }
            }
        }
        else
        {
            switch (monkeyKingCurrentCommand)
            {
                case 1:
                    if (command1State == 0 && command == "Noisy") command1State++;
                    else if (command1State == 1 && command == "Paper")
                    {
                        command1State++;
                        HideMonkeyKingCommand();
                    }
                    break;
                case 2:
                    if (command2State == 0 && command == "Paper") command2State++;
                    else if (command2State == 1 && command == "Noisy") command2State++;
                    else if (command2State == 2 && command == "Note")
                    {
                        command2State++;
                        HideMonkeyKingCommand();
                    }
                    break;
                case 3:
                    if (command3State == 0 && command == "Note") command3State++;
                    else if (command3State == 1 && command == "Note") command3State++;
                    else if (command3State == 2 && command == "Noisy") command3State++;
                    else if (command3State == 3 && command == "Paper") command3State++;
                    else if (command3State == 4 && command == "Paper")
                    {
                        command3State++;
                        HideMonkeyKingCommand();
                    }
                    break;
            }
        }
    }

    //~   void Update()
    //  {
    //    TestVoiceCommands();//
    //    if (currentRound == 2 && gameTime == 180 && !newRoundStarted)
    //      {
    //           newRoundStarted = true;
    //          StartCoroutine(StartNextRound());
    //       }
    //  }

    IEnumerator StartNextRound()
    {
        SceneManager.LoadScene("mid");
        yield return new WaitForSeconds(10);
        SceneManager.LoadScene("SampleScene");
        suspicionCount = 0;
        warningCount = 0;
        gameTime = 180;
    }

    void TestVoiceCommands()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            OnNoisyDetected();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            OnNoteDetected();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            OnPaperDetected();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            OnPickDetected();
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

    void SpawnNpcEnd()
    {
        npcEndSpawned = true;
        npc_end.SetActive(true);

        // 破壞場上的npc
        if (npc != null)
        {
            Destroy(npc);
            npc = null;
        }

        // 破壞克隆出來的npc_move
        if (currentCube != null)
        {
            Destroy(currentCube);
            currentCube = null;
        }

        StartCoroutine(EndGameAfterDelay(5f));
    }

    IEnumerator EndGameAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        EndGame();
    }

    void EndGame()
    {
        currentRound++;
        if (currentRound > 2)
        {
            SceneManager.LoadScene("ResultsScene");
        }
        else
        {
            gameTime = 180;
            StartCoroutine(SpawnCube());
        }
    }
}
