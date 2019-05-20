using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class MenuController : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip clickSound;
    public LevelChanger levelChanger;
    public PauseMenu pauseMenu;
    public DeathMenu deathMenu;

    // flag to prevent multiple clicks
    //private bool disableClicks;

    public void OnClick_Start()
    {
        // behaviour common to all buttons
        OnClick();

        // switch level
        levelChanger.LoadSceneAsyncWithFade("Level1");
    }

    public void TogglePauseMenu()
    {
        if (pauseMenu.IsPaused)
        {
            OnClick_Resume();
        } else
        {
            OnClick_PauseMenu();
        }
    }

    public void OnClick_Menu()
    {
        // behaviour common to all buttons
        OnClick();

        PlayerPrefs.SetInt("score", 0);

        // switch level
        levelChanger.LoadSceneAsyncWithFade("Menu");
    }

    public void OnClick_PauseMenu()
    {
        // behaviour common to all buttons
        OnClick();

        // switch level
        StartCoroutine(pauseMenu.ShowAsync());
        //levelChanger.LoadSceneAsyncWithFade("Menu");
    }

    public void OnClick_Restart()
    {
        // behaviour common to all buttons
        OnClick();

        PlayerPrefs.SetInt("score", 0);

        // switch level
        levelChanger.LoadSceneAsyncWithFade("Level1");
    }

    public void OnClick_Resume()
    {
        // behaviour common to all buttons
        OnClick();

        StartCoroutine(pauseMenu.HideAsync());
    }

    public void OnClick_Quit()
    {
        // behaviour common to all buttons
        OnClick();

        PlayerPrefs.SetInt("score", 0);
        Application.Quit(0);
    }

    private void OnClick()
    {
        // prevent multiple clicks
        //if (disableClicks)
        //{
        //    return;
        //}
        //disableClicks = true;


        // unhighlight
        EventSystem.current.SetSelectedGameObject(null);

        // play sound
        if (audioSource != null)
        {
            audioSource.clip = clickSound;
            audioSource.Play();
        }
    }

    internal void ShowDeathScreen()
    {
        // behaviour common to all buttons
        //OnClick();

        // switch level
        StartCoroutine(deathMenu.ShowAsync());
        //levelChanger.LoadSceneAsyncWithFade("Menu");
    }
}
