using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeathMenu : MonoBehaviour
{
    public CanvasGroup cg;
    public RigidbodyCharacter rigidbodyCharacter;
    public Text finalScore;

    public bool IsPaused = false;

    public IEnumerator ShowAsync()
    {
        gameObject.SetActive(true);
        cg.alpha = 0;
        IsPaused = true;
        rigidbodyCharacter.allowInputs = false;
        finalScore.text = "FINAL SCORE:\n" + PlayerPrefs.GetInt("score", 0);
        PlayerPrefs.SetInt("score", 0);
        yield return Helpers.FadeAlphaIn(cg, 0.3f);
    }

    public IEnumerator HideAsync()
    {
        yield return Helpers.FadeAlphaOut(cg, 0.3f);
        gameObject.SetActive(false);
        rigidbodyCharacter.allowInputs = true;
        IsPaused = false;
    }
}
