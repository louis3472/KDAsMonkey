using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class menu : MonoBehaviour
{
    public Button buttonStart;
    public Button buttonCredit;
    public Button buttonExit;
    public Button buttonSkip;
    public Button buttonCreditReturn;

    public GameObject imageRules;
    public GameObject imageHowToPlay;
    public GameObject panelCredits;

    private int imageState;

    private void Start()
    {
        buttonStart.onClick.AddListener(OnStartClicked);
        buttonCredit.onClick.AddListener(OnCreditClicked);
        buttonExit.onClick.AddListener(OnExitClicked);
        buttonSkip.onClick.AddListener(OnSkipClicked);
        buttonCreditReturn.onClick.AddListener(OnCreditReturnClicked);

        buttonSkip.gameObject.SetActive(false);
        buttonCreditReturn.gameObject.SetActive(false);
        imageState = 0;
    }

    private void OnStartClicked()
    {
        buttonSkip.gameObject.SetActive(true);
        imageRules.SetActive(true);
        imageHowToPlay.SetActive(false);
        panelCredits.SetActive(false);
    }

    private void OnCreditClicked()
    {
        buttonSkip.gameObject.SetActive(false);
        buttonCreditReturn.gameObject.SetActive(true);
        imageRules.SetActive(false);
        imageHowToPlay.SetActive(false);
        panelCredits.SetActive(true);
    }

    private void OnExitClicked()
    {
        Application.Quit();
    }

    private void OnSkipClicked()
    {
        if (imageState == 0)
        {
            imageRules.SetActive(false);
            imageHowToPlay.SetActive(true);
            imageState = 1;
        }
        else if (imageState == 1)
        {
            buttonSkip.gameObject.SetActive(false);
            SceneManager.LoadScene("Tutorial");
        }
    }

    private void OnCreditReturnClicked()
    {
        buttonCreditReturn.gameObject.SetActive(false);
        panelCredits.SetActive(false);
        ResetToInitialState();
    }

    private void ResetToInitialState()
    {
        imageRules.SetActive(false);
        imageHowToPlay.SetActive(false);
        buttonSkip.gameObject.SetActive(false);
    }
}
