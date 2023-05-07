using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class endgame : MonoBehaviour
{
    public Button buttonExit;
    public Button buttonReturn;

    private void Start()
    {
        buttonExit.onClick.AddListener(OnExitClicked);
        buttonReturn.onClick.AddListener(OnReturnClicked);
    }

    private void OnExitClicked()
    {
        Application.Quit();
    }

    private void OnReturnClicked()
    {
        SceneManager.LoadScene("MainMenu");
    }
}