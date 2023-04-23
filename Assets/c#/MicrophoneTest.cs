using UnityEngine;
using System.Collections;

public class MicrophoneTest : MonoBehaviour
{
    void Start()
    {
        string[] devices = Microphone.devices;
        if (devices.Length == 0)
        {
            Debug.Log("No microphone device detected.");
        }
        else
        {
            foreach (string device in devices)
            {
                Debug.Log("Microphone device detected: " + device);
            }
            AudioSource audioSource = GetComponent<AudioSource>();
            audioSource.clip = Microphone.Start(devices[0], true, 10, 44100);
            audioSource.loop = true;
            while (!(Microphone.GetPosition(null) > 0)) { }
            audioSource.Play();
            Debug.Log("Microphone recording started.");
        }
    }
}
