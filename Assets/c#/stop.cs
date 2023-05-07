using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class stop : MonoBehaviour
{
    public GameObject pauseMenu;
    public Button buttonPause;
    public Button buttonResume;
    public Button buttonMainMenu;
    public Button buttonExit;
    private bool isPaused = false;
    private final finalScript;

    private void Start()
    {
        finalScript = FindObjectOfType<final>();
        buttonPause.onClick.AddListener(PauseGame);
        buttonResume.onClick.AddListener(ResumeGame);
        buttonMainMenu.onClick.AddListener(ReturnToMainMenu);
        buttonExit.onClick.AddListener(ExitGame);
        pauseMenu.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused) ResumeGame();
            else PauseGame();
        }
    }

    private void PauseGame()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
        finalScript.enabled = false;
    }

    private void ResumeGame()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
        finalScript.enabled = true;
    }

    private void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    private void ExitGame()
    {
        Application.Quit();
    }
}
