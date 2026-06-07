using UnityEngine;
using UnityEngine.UI;

public class ToolboxUI : MonoBehaviour
{
    [SerializeField] private GameObject m_Container;
    [SerializeField] private ToolSlotUI m_ToolSlotPrefab;

    private void Awake()
    {
        PlayerToolboxManager.OnToolsChanged += UpdateToolboxUI;
    }

    private void OnDestroy()
    {
        PlayerToolboxManager.OnToolsChanged -= UpdateToolboxUI;
    }

    private void UpdateToolboxUI(ToolBaseSO[] tools)
    {
        foreach (Transform child in m_Container.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (var tool in tools)
        {
            ToolSlotUI slotUI = Instantiate(m_ToolSlotPrefab, m_Container.transform);
            slotUI.Initialize(tool);
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(m_Container.GetComponent<RectTransform>());
    }
}
