using UnityEngine;
using UnityEngine.SceneManagement;

public class LogoTransition : MonoBehaviour
{
    public string nextSceneName = "Main"; // Name of the next scene to load

    public void OnAnimationEvent()
    {
        SceneManager.LoadScene(nextSceneName);
    }
}