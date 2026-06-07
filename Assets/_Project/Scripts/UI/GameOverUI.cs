using UnityEngine;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private GameObject m_GameOverPanel;
    [SerializeField] private TMPro.TextMeshProUGUI m_GameOverMessageText;

    private void Start()
    {
        HideGameOverPanel();
    }

    private void OnEnable()
    {
        EntityManager.OnGameOver += ShowGameOverPanel;
    }

    private void OnDisable()
    {
        EntityManager.OnGameOver -= ShowGameOverPanel;
    }

    public void ShowGameOverPanel(string message)
    {
        m_GameOverPanel.SetActive(true);
        m_GameOverMessageText.text = $"Game Over\nReason: {message}";
    }

    public void HideGameOverPanel()
    {
        m_GameOverPanel.SetActive(false);
    }
}