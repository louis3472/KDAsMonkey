using UnityEngine;
using System.Collections;
using Microsoft.CognitiveServices.Speech;

public class SpeechRecognitionManager : MonoBehaviour
{
    private SpeechRecognizer recognizer;
    private string subscriptionKey = "<b0833dc754454631b983891a13a012ed>";
    private string region = "<eastasia>";

    private async void Start()
    {
        var config = SpeechConfig.FromSubscription(subscriptionKey, region);
        recognizer = new SpeechRecognizer(config);
        recognizer.Recognizing += OnPhraseRecognizing;

        await recognizer.StartContinuousRecognitionAsync();
    }

    private void OnDestroy()
    {
        recognizer.StopContinuousRecognitionAsync().Wait();
        recognizer.Dispose();
    }

    private void OnPhraseRecognizing(object sender, SpeechRecognitionEventArgs args)
    {
        Debug.Log($"Recognized: {args.Result.Text}");
    }
}
