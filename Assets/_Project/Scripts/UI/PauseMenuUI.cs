using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenuUI : MonoBehaviour
{
    [SerializeField] private GameObject m_PauseMenuPanel;
    private bool m_IsPaused = false;
    private bool m_GameOver = false;

    private void Awake()
    {
        EntityManager.OnGameOver += OnGameOver;
        m_PauseMenuPanel.SetActive(false);
    }

    private void OnDestroy()
    {
        EntityManager.OnGameOver -= OnGameOver;
    }

    private void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            TogglePauseMenu();
        }
    }

    public void TogglePauseMenu()
    {
        if (m_GameOver) return;

        m_IsPaused = !m_IsPaused;
        m_PauseMenuPanel.SetActive(m_IsPaused);

        // Here we don't want to set the Tick TimeScale, we want Unity's
        if (m_IsPaused) Time.timeScale = 0f;
        else Time.timeScale = 1f;
    }

    private void OnGameOver(string message)
    {
        m_GameOver = true;
        m_PauseMenuPanel.SetActive(false);
        Time.timeScale = 1f;
    }
}