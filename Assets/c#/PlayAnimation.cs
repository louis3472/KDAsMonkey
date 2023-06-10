using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayAnimation : MonoBehaviour
{
    public Image image; // �o�O�A�n����ʵe�� UI Image
    public Sprite[] frames; // �o�O�A���ʵe���Ҧ��V�A�A�ݭn�b Inspector ���]�w
    public float framesPerSecond = 10.0f; // �o�O�A���ʵe������t��

    void Start()
    {
        StartCoroutine(PlaySpriteAnimation());
    }

    IEnumerator PlaySpriteAnimation()
    {
        int index = 0;

        while (true)
        {
            // ����U�@�V
            image.sprite = frames[index];

            // ���ݤU�@�V
            yield return new WaitForSeconds(0.5f / framesPerSecond);

            // ��s����
            index = (index + 1) % frames.Length;
        }
    }
}
