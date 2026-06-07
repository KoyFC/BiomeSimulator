using UnityEngine;
using UnityEngine.UI;

public class ToolboxUI : MonoBehaviour
{
    [Header("Tools")]
    [SerializeField] private GameObject m_Container;
    [SerializeField] private ToolSlotUI m_ToolSlotPrefab;

    [Header("Brush Size")]
    [SerializeField] private Slider m_BrushSizeSlider;
    [SerializeField] private TMPro.TMP_Text m_BrushSizeText;

    private void Awake()
    {
        PlayerToolboxManager.OnToolsChanged += UpdateToolboxUI;
    }

    private void Start()
    {
        m_BrushSizeSlider.onValueChanged.AddListener(OnBrushSizeChanged);
        m_BrushSizeSlider.minValue = 1;
        m_BrushSizeSlider.maxValue = PlayerToolboxManager.Instance.MaxBrushSize;
        m_BrushSizeSlider.value = PlayerToolboxManager.Instance.BrushSize;
    }

    private void OnDestroy()
    {
        PlayerToolboxManager.OnToolsChanged -= UpdateToolboxUI;
        m_BrushSizeSlider.onValueChanged.RemoveListener(OnBrushSizeChanged);
    }

    private void Update()
    {
        m_BrushSizeSlider.value = PlayerToolboxManager.Instance.BrushSize;
        m_BrushSizeText.text = $"Brush Size: {PlayerToolboxManager.Instance.BrushSize}";
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

    private void OnBrushSizeChanged(float newSize)
    {
        int roundedSize = Mathf.RoundToInt(newSize);
        if (roundedSize % 2 == 0) roundedSize += 1;

        PlayerToolboxManager.Instance.BrushSize = roundedSize;
    }
}
