using UnityEngine;

public class BestTickDisplay : MonoBehaviour
{
    [SerializeField] private TMPro.TextMeshProUGUI m_BestTickText;

    private void Start()
    {
        uint bestTickCount = (uint)PlayerPrefs.GetInt("BestTickCount", 0);
        if (bestTickCount == 0) m_BestTickText.gameObject.SetActive(false);
        m_BestTickText.text = $"Most Ticks Survived: {bestTickCount}";
    }
}