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
        Time.timeScale = 1f;
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

    public void QuitGame()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
