using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class endgame : MonoBehaviour
{
    public Button buttonExit;
    public Button buttonReturn;
    public VideoPlayer videoPlayer;
    public Image endGameImage;
    public AudioSource endGameMusic;

    private bool videoEnded = false;

    private void Start()
    {
        videoPlayer.loopPointReached += OnVideoEnded;

        // Start playing the video
        videoPlayer.Play();

        buttonExit.onClick.AddListener(OnExitClicked);
        buttonReturn.onClick.AddListener(OnReturnClicked);
        buttonExit.gameObject.SetActive(false);
        buttonReturn.gameObject.SetActive(false);

        // Initially hide the image and stop the music
        endGameImage.gameObject.SetActive(false);
        endGameMusic.Stop();
    }

    private void Update()
    {
        // If the left mouse button is clicked, stop the video
        if (Input.GetMouseButtonDown(0))
        {
            videoPlayer.Stop();
            OnVideoEnded(videoPlayer);
        }
    }

    private void OnVideoEnded(VideoPlayer vp)
    {
        videoEnded = true;
        buttonExit.gameObject.SetActive(true);
        buttonReturn.gameObject.SetActive(true);

        // Disable the video player
        videoPlayer.enabled = false;

        // Show the image and play the music after the video ends
        endGameImage.gameObject.SetActive(true);
        endGameMusic.Play();
    }

    private void OnExitClicked()
    {
        if (videoEnded)
        {
            Application.Quit();
        }
    }

    private void OnReturnClicked()
    {
        if (videoEnded)
        {
            SceneManager.LoadScene("MainMenu");
        }
    }
}
