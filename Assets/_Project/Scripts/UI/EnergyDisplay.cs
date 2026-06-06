using UnityEngine;
using UnityEngine.UI;

public class EnergyDisplay : MonoBehaviour
{
    [SerializeField] private EntityBase m_Entity = null;
    [SerializeField] private Slider m_EnergySlider = null;

    private void Awake()
    {
        if (m_Entity == null) m_Entity = GetComponentInParent<EntityBase>();
        if (m_EnergySlider == null) m_EnergySlider = GetComponentInChildren<Slider>();
    }

    private void Update()
    {
        if (m_Entity == null || m_EnergySlider == null) return;

        m_EnergySlider.value = m_Entity.Energy / m_Entity.MaxEnergy;
    }
}
