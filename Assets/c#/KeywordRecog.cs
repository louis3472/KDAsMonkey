using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows.Speech;

public class KeywordRecog : MonoBehaviour
{
    [SerializeField]
    private string[] m_Keywords;

    [SerializeField]
    private Text m_uiText;

    [SerializeField]
    private Transform m_character;

    private KeywordRecognizer m_Recognizer;
    private Dictionary<string, Action> m_actionMap = new Dictionary<string, Action>();
    private void Start()
    {
        m_actionMap.Add("up", Up);
        m_actionMap.Add("down", Down);
        m_actionMap.Add("left", Left);
        m_actionMap.Add("right", Right);

        m_Recognizer = new KeywordRecognizer(m_Keywords);
        m_Recognizer.OnPhraseRecognized += OnPhraseRecognized;
        m_Recognizer.Start();
    }

    private void OnPhraseRecognized(PhraseRecognizedEventArgs args)
    {
        var builder = new StringBuilder();
        builder.AppendFormat("{0} ({1}){2}", args.text, args.confidence, Environment.NewLine);
        builder.AppendFormat("\tTimestamp: {0}{1}", args.phraseStartTime, Environment.NewLine);
        builder.AppendFormat("\tDuration: {0} seconds{1}", args.phraseDuration.TotalSeconds, Environment.NewLine);
        Debug.Log(builder.ToString());
        m_uiText.text = $"You said : {args.text}";

        m_actionMap[args.text].Invoke();
    }

    private void Up()
    {
        m_character.Translate(0, 2, 0);
    }
    private void Down()
    {
        m_character.Translate(0, -2, 0);
    }
    private void Left()
    {
        m_character.Translate(-2, 0, 0);
    }
    private void Right()
    {
        m_character.Translate(2, 0, 0);

    }
}