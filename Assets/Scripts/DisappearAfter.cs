using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisappearAfter : MonoBehaviour
{
    public float seconds;
    public CanvasGroup canvasGroup;

    void Start()
    {
        StartCoroutine(HideAsync());
    }

    private IEnumerator HideAsync()
    {
        yield return new WaitForSeconds(seconds);
        yield return Helpers.FadeAlphaOut(canvasGroup, 1);
        gameObject.SetActive(false);
    }
}
