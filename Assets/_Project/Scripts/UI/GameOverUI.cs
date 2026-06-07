using UnityEngine;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private GameObject m_GameOverPanel;
    [SerializeField] private GameObject m_ShowGameOverPanelButton;
    [SerializeField] private TMPro.TextMeshProUGUI m_GameOverMessageText;

    private void Start()
    {
        HideGameOverPanel();
        m_ShowGameOverPanelButton.SetActive(false);
    }

    private void OnEnable()
    {
        EntityManager.OnGameOver += DisplayGameOverPanelWithMessage;
    }

    private void OnDisable()
    {
        EntityManager.OnGameOver -= DisplayGameOverPanelWithMessage;
    }

    private void DisplayGameOverPanelWithMessage(string message)
    {
        m_GameOverMessageText.text = $"Game Over\nReason: {message}";
        ShowGameOverPanel();
    }

    public void ShowGameOverPanel()
    {
        m_GameOverPanel.SetActive(true);
        m_ShowGameOverPanelButton.SetActive(false);
    }

    public void HideGameOverPanel()
    {
        m_GameOverPanel.SetActive(false);
        m_ShowGameOverPanelButton.SetActive(true);
    }
}