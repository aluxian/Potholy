using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelChanger : MonoBehaviour
{

    public Animator animator;

    private string sceneName;

    public void LoadSceneAsyncWithFade(string sceneName)
    {
        this.sceneName = sceneName;
        animator.SetTrigger("FadeOut");
    }

    public void OnFadeComplete()
    {
        SceneManager.LoadSceneAsync(sceneName);
    }
}
