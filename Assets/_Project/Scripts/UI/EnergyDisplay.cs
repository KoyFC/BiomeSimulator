using UnityEngine;
using UnityEngine.UI;

public class EnergyDisplay : MonoBehaviour
{
    private EntityBase m_Entity = null;
    [SerializeField] private Transform m_FillImageTransform = null;

    private void Awake()
    {
        m_Entity = GetComponentInParent<EntityBase>();
    }

    private void Update()
    {
        if (m_Entity == null || m_FillImageTransform == null) return;

        m_FillImageTransform.localScale = new Vector3(m_Entity.Energy / m_Entity.MaxEnergy, 1f, 1f);
    }
}
