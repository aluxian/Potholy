using System.Collections;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public class Helpers
{
    public static IEnumerator FadeAlphaIn(CanvasGroup canvasGroup, float duration)
    {
        while (canvasGroup.alpha < 1f)
        {
            canvasGroup.alpha += Time.deltaTime / duration;
            yield return null;
        }
    }

    public static IEnumerator FadeAlphaOut(CanvasGroup canvasGroup, float duration)
    {
        while (canvasGroup.alpha > 0f)
        {
            canvasGroup.alpha -= Time.deltaTime / duration;
            yield return null;
        }
    }
}
