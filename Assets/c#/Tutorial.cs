using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Windows.Speech;
using System.Linq;

public class Tutorial : MonoBehaviour
{
    public Image paperImage;
    public Image noisyImage;
    public Image noteImage;
    public Image pickImage;
    public Image okImage;
    public GameObject paper;
    public GameObject noisy;
    public GameObject note;
    public GameObject pick;
    public GameObject ok;
    public GameObject go;  // Add your new GameObject here
    public AudioSource backgroundAudio;

    private KeywordRecognizer keywordRecognizer;
    private Dictionary<string, System.Action> keywords = new Dictionary<string, System.Action>();
    private List<Image> tutorialImages;
    private List<GameObject> tutorialObjects;
    private Queue<GameObject> objectQueue;

    private void Start()
    {
        tutorialImages = new List<Image> { paperImage, noisyImage, noteImage, pickImage, okImage };
        tutorialObjects = new List<GameObject> { paper, noisy, note, pick, ok };
        objectQueue = new Queue<GameObject>(new[] { paper, noisy, note, pick, ok });

        // Hide all tutorial images and objects at the start
        foreach (var image in tutorialImages)
        {
            image.gameObject.SetActive(false);
        }
        foreach (var obj in tutorialObjects)
        {
            obj.SetActive(false);
        }
        go.SetActive(false);  // Also hide the new GameObject

        // Show first tutorial image at the start
        paperImage.gameObject.SetActive(true);

        // Add keyword commands to dictionary
        keywords.Add("Paper", () => { ProcessKeyword(paper, paperImage); });
        keywords.Add("Noisy", () => { ProcessKeyword(noisy, noisyImage); });
        keywords.Add("Note", () => { ProcessKeyword(note, noteImage); });
        keywords.Add("Pick", () => { ProcessKeyword(pick, pickImage); });
        keywords.Add("OK", () => { ProcessKeyword(ok, okImage); });

        // Setup keyword recognizer and start it
        keywordRecognizer = new KeywordRecognizer(keywords.Keys.ToArray());
        keywordRecognizer.OnPhraseRecognized += KeywordRecognizer_OnPhraseRecognized;
        keywordRecognizer.Start();
    }

    private void Update()
    {
        // Listen for number key presses
        if (Input.GetKeyDown(KeyCode.Alpha1)) ProcessKeyword(paper, paperImage);
        else if (Input.GetKeyDown(KeyCode.Alpha2)) ProcessKeyword(noisy, noisyImage);
        else if (Input.GetKeyDown(KeyCode.Alpha3)) ProcessKeyword(note, noteImage);
        else if (Input.GetKeyDown(KeyCode.Alpha4)) ProcessKeyword(pick, pickImage);
        else if (Input.GetKeyDown(KeyCode.Alpha5)) ProcessKeyword(ok, okImage);
        else if (Input.GetKeyDown(KeyCode.Alpha6)) SceneManager.LoadScene("SampleScene");
    }

    private void ProcessKeyword(GameObject commandObject, Image commandImage)
    {
        if (objectQueue.Count == 0 || objectQueue.Peek() != commandObject) return;

        StartCoroutine(ShowAndHide(commandObject, commandImage, 2.0f));
        objectQueue.Dequeue();
    }

    private void KeywordRecognizer_OnPhraseRecognized(PhraseRecognizedEventArgs args)
    {
        System.Action keywordAction;

        if (keywords.TryGetValue(args.text, out keywordAction))
        {
            keywordAction.Invoke();
        }
    }

    IEnumerator ShowAndHide(GameObject go, Image image, float delay)
    {
        go.SetActive(true);
        image.gameObject.SetActive(true);
        yield return new WaitForSeconds(delay);
        go.SetActive(false);
        image.gameObject.SetActive(false);

        if (objectQueue.Count > 0)
        {
            tutorialImages[tutorialObjects.IndexOf(objectQueue.Peek())].gameObject.SetActive(true);
        }
        else
        {
            // Activate new GameObject and stop background audio
            this.go.SetActive(true);
            backgroundAudio.Stop();
            yield return new WaitForSeconds(5.0f);
            SceneManager.LoadScene("SampleScene");
        }
    }
}
