using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "SceneTransitionerSO", menuName = "Scriptable Objects/SceneTransitionerSO")]
public class SceneTransitionerSO : ScriptableObject
{
    [SerializeField] private string m_MainMenuSceneName;
    [SerializeField] private string m_PlaySceneName;

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void ReloadCurrentScene()
    {
        LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LoadMainMenuScene()
    {
        LoadScene(m_MainMenuSceneName);
    }

    public void LoadPlayScene()
    {
        LoadScene(m_PlaySceneName);
    }
}
