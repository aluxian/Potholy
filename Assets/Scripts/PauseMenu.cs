using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public CanvasGroup cg;
    public RigidbodyCharacter rigidbodyCharacter;

    public bool IsPaused = false;

    public IEnumerator ShowAsync()
    {
        gameObject.SetActive(true);
        cg.alpha = 0;
        IsPaused = true;
        rigidbodyCharacter.allowInputs = false;
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
