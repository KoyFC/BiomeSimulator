using UnityEngine;
using UnityEngine.UI;

public class ToolSlotUI : MonoBehaviour
{
    [SerializeField] private TMPro.TMP_Text m_NameText;
    [SerializeField] private Image m_Icon;

    public void Initialize(ToolBaseSO tool)
    {
        m_NameText.text = tool.Name;
        m_Icon.sprite = tool.Icon;
    }
}
