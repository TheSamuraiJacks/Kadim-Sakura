using System.Collections;
using UnityEngine;
using TMPro;

public class SplashController : MonoBehaviour
{
    public RectTransform slash1;
    public RectTransform slash2;

    public TextMeshProUGUI japaneseText;
    public TextMeshProUGUI gameTitle;

    void Start()
    {
        slash1.localScale = new Vector3(0, 1, 1);
        slash2.localScale = new Vector3(0, 1, 1);

        japaneseText.text = "";
        gameTitle.alpha = 0;
        gameTitle.transform.localScale = Vector3.zero;

        StartCoroutine(SplashSequence());
    }

    IEnumerator SplashSequence()
    {
        // Slash 1
        yield return ScaleX(slash1, 0.3f);
        yield return new WaitForSeconds(0.1f);

        // Slash 2
        yield return ScaleX(slash2, 0.3f);
        yield return new WaitForSeconds(0.3f);

        // Japanese text hece hece
        string[] chars = { "古", "代", "の", "桜" };
        foreach (var c in chars)
        {
            japaneseText.text += c + "\n";
            yield return new WaitForSeconds(0.25f);
        }

        yield return new WaitForSeconds(0.3f);

        // Game title
        StartCoroutine(FadeIn(gameTitle, 0.5f));
        StartCoroutine(ScaleUp(gameTitle, 0.4f));
    }

    IEnumerator ScaleX(RectTransform obj, float time)
    {
        float t = 0;
        while (t < time)
        {
            t += Time.deltaTime;
            float x = Mathf.Lerp(0, 1, t / time);
            obj.localScale = new Vector3(x, 1, 1);
            yield return null;
        }
    }

    IEnumerator FadeIn(TextMeshProUGUI txt, float time)
    {
        float t = 0;
        while (t < time)
        {
            t += Time.deltaTime;
            txt.alpha = Mathf.Lerp(0, 1, t / time);
            yield return null;
        }
    }

    IEnumerator ScaleUp(TextMeshProUGUI txt, float time)
    {
        float t = 0;
        while (t < time)
        {
            t += Time.deltaTime;
            txt.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, t / time);
            yield return null;
        }
    }
}
