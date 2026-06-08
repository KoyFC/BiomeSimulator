using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TickControllerUI : MonoBehaviour
{
    [SerializeField] private float[] m_TickSpeeds;
    [SerializeField] private Transform m_ButtonContainer;
    [SerializeField] private Button m_ButtonPrefab;
    [SerializeField] private Color m_SelectedColor = Color.yellow;

    private Button[] m_Buttons;
    private Image m_LastClickedButtonImage;

    private void OnEnable()
    {
        TickManager.OnTimeScaleChanged += OnTimeScaleChanged;
    }

    private void OnDisable()
    {
        TickManager.OnTimeScaleChanged -= OnTimeScaleChanged;
    }

    private void Start()
    {
        m_Buttons = new Button[m_TickSpeeds.Length];
        for (int i = 0; i < m_TickSpeeds.Length; i++)
        {
            int index = i;
            Button button = Instantiate(m_ButtonPrefab, m_ButtonContainer);
            button.GetComponentInChildren<TMPro.TMP_Text>().text = $"{m_TickSpeeds[i]}x";
            button.onClick.AddListener(() => OnTickSpeedButtonClicked(index));

            if (m_TickSpeeds[index] == TickManager.Instance.TimeScale)
            {
                m_LastClickedButtonImage = button.GetComponent<Image>();
                m_LastClickedButtonImage.color = m_SelectedColor;
            }

            m_Buttons[i] = button;
        }
    }

    private void OnTickSpeedButtonClicked(int index)
    {
        float timeScale = m_TickSpeeds[index];
        TickManager.Instance.SetTimeScale(timeScale);

        Button button = m_Buttons[index];

        m_LastClickedButtonImage.color = Color.white;
        m_LastClickedButtonImage = button.GetComponent<Image>();
        button.GetComponent<Image>().color = m_SelectedColor;
    }

    private void OnTimeScaleChanged(float newTimeScale)
    {
        for (int i = 0; i < m_TickSpeeds.Length; i++)
        {
            if (Mathf.Approximately(m_TickSpeeds[i], newTimeScale))
            {
                Button button = m_Buttons[i];
                m_LastClickedButtonImage.color = Color.white;
                m_LastClickedButtonImage = button.GetComponent<Image>();
                button.GetComponent<Image>().color = m_SelectedColor;
                break;
            }
        }
    }
}