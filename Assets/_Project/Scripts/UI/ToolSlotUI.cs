using UnityEngine;
using UnityEngine.UI;

public class ToolSlotUI : MonoBehaviour
{
    private ToolBaseSO m_Tool;

    [SerializeField] private TMPro.TMP_Text m_NameText;
    [SerializeField] private Image m_Icon;
    [SerializeField] private Button m_Button;
    [SerializeField] private GameObject m_SelectionHighlight;

    public void Initialize(ToolBaseSO tool)
    {
        m_Tool = tool;
        m_NameText.text = tool.Name;
        m_Icon.sprite = tool.Icon;
        m_Button.onClick.AddListener(() => PlayerToolboxManager.Instance.TrySelectTool(tool));
    }

    private void LateUpdate()
    {
        bool isSelected = PlayerToolboxManager.Instance.CurrentTool == m_Tool;
        if (isSelected == m_SelectionHighlight.activeSelf) return;
        m_SelectionHighlight.SetActive(isSelected);
    }
}
