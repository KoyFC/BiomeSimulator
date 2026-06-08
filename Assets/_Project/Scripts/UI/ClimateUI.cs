using UnityEngine;

public class ClimateUI : MonoBehaviour
{
    [SerializeField] private GameObject m_Container;
    [SerializeField] private TMPro.TMP_Text m_EventNameText;
    [SerializeField] private TMPro.TMP_Text m_EventDescriptionText;

    private void OnEnable()
    {
        ClimateManager.OnClimateEventStarted += UpdateUI;
        ClimateManager.OnClimateEventEnded += ClearUI;
        ClearUI();
    }

    private void OnDisable()
    {
        ClimateManager.OnClimateEventStarted -= UpdateUI;
        ClimateManager.OnClimateEventEnded -= ClearUI;
    }

    private void UpdateUI(ClimateEventBaseSO climateEvent)
    {
        m_Container.SetActive(true);

        string eventName = climateEvent.EventName;
        if (string.IsNullOrEmpty(eventName)) m_EventNameText.gameObject.SetActive(false);
        else
        {
            m_EventNameText.gameObject.SetActive(true);
            m_EventNameText.text = climateEvent.EventName;
        }

        string description = climateEvent.Description;
        if (string.IsNullOrEmpty(description)) m_EventDescriptionText.gameObject.SetActive(false);
        else
        {
            m_EventDescriptionText.gameObject.SetActive(true);
            m_EventDescriptionText.text = climateEvent.Description;
        }
    }

    private void ClearUI(ClimateEventBaseSO endedEvent = null)
    {
        m_Container.SetActive(false);
        m_EventNameText.text = "";
        m_EventDescriptionText.text = "";
    }
}
