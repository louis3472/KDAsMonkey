using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayAnimation : MonoBehaviour
{
    public Image image; // 這是你要播放動畫的 UI Image
    public Sprite[] frames; // 這是你的動畫的所有幀，你需要在 Inspector 中設定
    public float framesPerSecond = 10.0f; // 這是你的動畫的播放速度

    void Start()
    {
        StartCoroutine(PlaySpriteAnimation());
    }

    IEnumerator PlaySpriteAnimation()
    {
        int index = 0;

        while (true)
        {
            // 播放下一幀
            image.sprite = frames[index];

            // 等待下一幀
            yield return new WaitForSeconds(0.5f / framesPerSecond);

            // 更新索引
            index = (index + 1) % frames.Length;
        }
    }
}
