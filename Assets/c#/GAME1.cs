using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;
using UnityEngine.Windows.Speech;
using System.Linq;
using System.Collections.Generic;
using Unity.VisualScripting;
using static System.Net.Mime.MediaTypeNames;

public class GAME1 : MonoBehaviour
{
    public GameObject player;
    public GameObject npc;
    public GameObject npc_move;
    public GameObject npc_end;
    public GameObject phonePrefab;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI warningText;
    public TextMeshProUGUI suspicionText;
    public TextMeshProUGUI wisdomText;
    public GameObject[] instructionImages; // 指令圖片
    public GameObject imageNoisy;
    public GameObject imageNote;
    public GameObject imageThrow;
    public GameObject imageMe;
    public GameObject imageInstruction1;  // 指令圖片1
    public GameObject imageInstruction2;  // 指令圖片2
    public GameObject imageInstruction3;  // 指令圖片3
    public GameObject imageMonkey;
    public GameObject imageMonkeyMove;

    private GameObject phoneInstance;
    private float gameTime = 180;
    private float timeRemaining;
    private int currentRound = 1;
    private bool cubeActive = false;
    private bool npcEndSpawned = false;
    private float suspicionTimer = 15f; // 新增一个懷疑倒计时
    private bool instructionNotCompleted = false; // 新增一个标志以判断是否完成猴王指令

    private GameObject currentCube;
    private int warningCount = 0;
    private int suspicionCount = 0;
    private int wisdomCount = 0;
    private KeywordRecognizer keywordRecognizer;
    private Dictionary<string, System.Action> keywords = new Dictionary<string, System.Action>();
    private int monkeyKingState = 0;
    private float monkeyKingTimer = 0;

    private Dictionary<string, int> recentCommands = new Dictionary<string, int>();
    private Dictionary<string, int> commandCounts = new Dictionary<string, int>();
    private Dictionary<string, int> targetCommandCounts;


    private Dictionary<List<string>, int> monkeyKingTargets = new Dictionary<List<string>, int>
    {
        { new List<string> { "noisy", "paper" }, 1 },
        { new List<string> { "paper", "noisy", "note" }, 1 },
        { new List<string> { "note", "note", "paper", "paper", "noisy" }, 1 }
    };


    void Start()
    {
        timeRemaining = gameTime;
        StartCoroutine(SpawnCube());
        npc_end.SetActive(false);

        keywords.Add("noisy", OnNoisyDetected);
        keywords.Add("note", OnNoteDetected);
        keywords.Add("paper", OnThrowDetected);
        keywords.Add("pick", OnMeDetected);

        keywordRecognizer = new KeywordRecognizer(keywords.Keys.ToArray());
        keywordRecognizer.OnPhraseRecognized += KeywordRecognizer_OnPhraseRecognized;
        keywordRecognizer.Start();

        warningText.text = $": {warningCount}";
        suspicionText.text = $": {suspicionCount}";
        wisdomText.text = $": {wisdomCount}";

        // 隱藏所有指令圖片
        foreach (GameObject image in instructionImages)
        {
            image.SetActive(false);
        }

        // 隱藏所有單詞圖片
        imageNoisy.SetActive(false);
        imageNote.SetActive(false);
        imageThrow.SetActive(false);
        imageMe.SetActive(false);

        imageInstruction1.SetActive(false);
        imageInstruction2.SetActive(false);
        imageInstruction3.SetActive(false);

        imageMonkey.SetActive(true);
        imageMonkeyMove.SetActive(false);

        StartCoroutine(ShowInstructionImagesLoop());


        targetCommandCounts = new Dictionary<string, int>
    {
        { "noisy", 1 },
        { "paper", 1 },
        { "note", 1 }
    };
    }

    void Update()
    {
        UpdateWarningText();
        UpdateSuspicionText();
        UpdateWisdomText();
        UpdateTimer();

        if (instructionNotCompleted) // 如果指令未完成
        {
            suspicionTimer -= Time.deltaTime; // 开始倒计时
            if (suspicionTimer <= 0) // 倒计时结束
            {
                suspicionCount++; // 增加懷疑次數
                suspicionTimer = 15f; // 重置倒计时
                instructionNotCompleted = false; // 重置指令未完成标志
            }
        }
        if (instructionNotCompleted && suspicionTimer <= 0) // 倒计时结束
        {
            suspicionCount++; // 增加懷疑次數
            suspicionTimer = 15f; // 重置倒计时
            instructionNotCompleted = false; // 重置指令未完成标志
        }

        if (warningCount >= 3)
        {
            npc.SetActive(false);
            npc_move.SetActive(false);
            if (!npcEndSpawned)
            {
                npc_end.SetActive(true);
                npcEndSpawned = true;
            }
        }

        MonkeyKingLogic();

        if (warningCount >= 3 || suspicionCount >= 3)
        {
            EndGame("ResultsScene");
        }

        if (wisdomCount >= 10 && timeRemaining <= 0)
        {
            EndGame("HE");
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

    void UpdateWisdomText()
    {
        wisdomText.text = $" {wisdomCount}";
    }
    IEnumerator ShowInstructionImages()
    {
        yield return new WaitForSeconds(5f);
        while (true)
        {
            int randomIndex = Random.Range(0, 3);
            GameObject selectedImage = null;
            switch (randomIndex)
            {
                case 0:
                    selectedImage = imageInstruction1;
                    break;
                case 1:
                    selectedImage = imageInstruction2;
                    break;
                case 2:
                    selectedImage = imageInstruction3;
                    break;
            }
            ShowMonkeyKingImage(selectedImage, 15f);
            yield return new WaitForSeconds(20f);
        }
    }
    IEnumerator ShowInstructionImagesLoop()
    {
        while (true)
        {
            int randomIndex = Random.Range(0, 3);
            GameObject selectedImage = null;
            switch (randomIndex)
            {
                case 0:
                    selectedImage = imageInstruction1;
                    break;
                case 1:
                    selectedImage = imageInstruction2;
                    break;
                case 2:
                    selectedImage = imageInstruction3;
                    break;
            }
            ShowMonkeyKingImage(selectedImage, 15f);
            yield return new WaitForSeconds(20f);
        }
    }

    IEnumerator SpawnCube()
    {
        while (currentRound <= 2 && !npcEndSpawned)
        {
            // 等待 7 秒至 14 秒之間的隨機時間
            yield return new WaitForSeconds(Random.Range(7, 14));

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
        }
        if (!npcEndSpawned)
        {
            EndGame("ResultsScene");
        }
    }


    public void OnNoisyDetected()
    {
        ShowImage(imageNoisy);
        CheckCommand("noisy");
    }

    public void OnNoteDetected()
    {
        ShowImage(imageNote);
        CheckCommand("note");
    }

    public void OnThrowDetected()
    {
        ShowImage(imageThrow);
        CheckCommand("paper");
    }

    public void OnMeDetected()
    {
        ShowImage(imageMe);
        CheckCommand("pick");
    }

    private void KeywordRecognizer_OnPhraseRecognized(PhraseRecognizedEventArgs args)
    {
        System.Action keywordAction;

        if (keywords.TryGetValue(args.text, out keywordAction))
        {
            keywordAction.Invoke();
        }
    }

    private List<List<string>> monkeyKingCommands = new List<List<string>>
{
    new List<string> { "noisy", "paper" },
    new List<string> { "paper", "noisy", "note" },
    new List<string> { "note", "note", "paper", "paper", "noisy" }
};
    void UpdateTimer()
    {
        timeRemaining -= Time.deltaTime;
        timerText.text = Mathf.Floor(timeRemaining).ToString();

        if (timeRemaining <= 0)
        {
            currentRound++;
            if (currentRound <= 2)
            {
                SceneManager.LoadScene(currentRound == 2 ? "SampleScene2" : "SampleScene");
                timeRemaining = gameTime;
                StartCoroutine(SpawnCube());
            }
            else
            {
                EndGame("ResultsScene");
            }
        }
    }

    void ShowImage(GameObject image, bool monkeyKingInstruction = false)
    {
        StartCoroutine(ShowImageCoroutine(image, monkeyKingInstruction));
    }


    IEnumerator ShowImageCoroutine(GameObject image, bool monkeyKingInstruction)
    {
        image.SetActive(true);

        if (monkeyKingInstruction)
        {
            // 當猴王指示圖片顯示時
            imageMonkey.SetActive(false);
            imageMonkeyMove.SetActive(true);
        }

        yield return new WaitForSeconds(1f);
        image.SetActive(false);

        if (monkeyKingInstruction)
        {
            // 當猴王指示圖片消失時
            imageMonkey.SetActive(true);
            imageMonkeyMove.SetActive(false);
        }
    }
    IEnumerator ShowMonkeyKingImageCoroutine(GameObject image, float duration)
    {
        image.SetActive(true);
        imageMonkey.SetActive(false);
        imageMonkeyMove.SetActive(true);

        yield return new WaitForSeconds(duration);

        instructionNotCompleted = true; // 指令未完成
        suspicionTimer = 15f; // 重置倒计时

        image.SetActive(false);
        imageMonkey.SetActive(true);
        imageMonkeyMove.SetActive(false);
    }

    bool CheckInstructionCompletion(GameObject image)
    {
        if (image == imageInstruction1 && commandCounts.ContainsKey("noisy") && commandCounts["noisy"] >= 1 &&
            commandCounts.ContainsKey("paper") && commandCounts["paper"] >= 1)
        {
            commandCounts["noisy"] -= 1;
            commandCounts["paper"] -= 1;
            wisdomCount++; // 增加智慧点数
            return true;
        }
        else if (image == imageInstruction2 && commandCounts.ContainsKey("noisy") && commandCounts["noisy"] >= 1 &&
                 commandCounts.ContainsKey("paper") && commandCounts["paper"] >= 1 &&
                 commandCounts.ContainsKey("note") && commandCounts["note"] >= 1)
        {
            commandCounts["noisy"] -= 1;
            commandCounts["paper"] -= 1;
            commandCounts["note"] -= 1;
            wisdomCount++; // 增加智慧点数
            return true;
        }
        else if (image == imageInstruction3 && commandCounts.ContainsKey("noisy") && commandCounts["noisy"] >= 1 &&
                 commandCounts.ContainsKey("paper") && commandCounts["paper"] >= 2 &&
                 commandCounts.ContainsKey("note") && commandCounts["note"] >= 2)
        {
            commandCounts["noisy"] -= 1;
            commandCounts["paper"] -= 2;
            commandCounts["note"] -= 2;
            wisdomCount++; // 增加智慧点数
            return true;
        }

        return false;
    }

    void ShowMonkeyKingImage(GameObject image, float duration)
    {
        StartCoroutine(ShowMonkeyKingImageCoroutine(image, duration));
    }

    void MonkeyKingLogic()
    {
        if (recentCommands.Count >= 5 && recentCommands.All(x => x.Key == "noisy"))
        {
            StartCoroutine(DisableWarningForSeconds(10f));
        }
        else if (recentCommands.Count >= 3 && recentCommands.Take(3).All(x => x.Key == "note" || x.Key == "paper"))
        {
            StartCoroutine(DisableWarningForSeconds(5f));
        }
    }

    IEnumerator DisableWarningForSeconds(float seconds)
    {
        cubeActive = false;
        yield return new WaitForSeconds(seconds);
        cubeActive = true;
    }

    void CheckCommand(string command)
    {
        if (cubeActive && command != "pick")
        {
            warningCount++;
        }

        if (!recentCommands.ContainsKey(command))
        {
            recentCommands[command] = 0;
        }
        recentCommands[command]++;
        if (recentCommands.Count > 5)
        {
            recentCommands.Remove(recentCommands.First().Key);
        }

        if (!commandCounts.ContainsKey(command))
        {
            commandCounts[command] = 0;
        }
        commandCounts[command]++;

        if (command == "pick" && currentCube != null)
        {
            wisdomCount++;
            Destroy(currentCube);
            currentCube = null;
            npc.SetActive(true); // 顯示 npc
            StartCoroutine(DisableNpcForSeconds(1f)); // 1 秒後消失
        }

        if (instructionNotCompleted)
        {
            CheckCommandCompletion();
        }
    }
    IEnumerator DisableNpcForSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        npc.SetActive(false);
    }

    void CheckCommandCompletion()
    {
        bool instructionCompleted = false;

        foreach (var command in monkeyKingTargets.Keys)
        {
            bool isCompleted = command.All(k => commandCounts.ContainsKey(k) && commandCounts[k] >= monkeyKingTargets[command]);
            if (isCompleted)
            {
                wisdomCount++;
                foreach (var k in command)
                {
                    commandCounts[k] -= monkeyKingTargets[command];
                }
                instructionCompleted = true;
                instructionNotCompleted = false; // 指令已完成
                break;
            }
        }

        if (!instructionCompleted)
        {
            suspicionCount++; // 增加懷疑次數
            instructionNotCompleted = false; // 指令已完成
        }
    }


void EndGame(string sceneName)
    {
        SceneManager.LoadScene("ResultsScene");
    }

}
