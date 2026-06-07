using UnityEngine;
using UnityEngine.UI;

public class EnergyDisplay : MonoBehaviour
{
    private EntityBase m_Entity = null;
    [SerializeField] private Transform m_FillImageTransform = null;
    [SerializeField] private GameObject m_Container = null;

    private void Awake()
    {
        m_Entity = GetComponentInParent<EntityBase>();
    }

    private void Update()
    {
        if (m_Entity == null || m_FillImageTransform == null) return;

        bool visible = MapTileManager.Instance.IsTileHighlighted(m_Entity.CurrentTile);
        if (m_Container != null)
        {
            if (m_Container.activeSelf != visible) m_Container.SetActive(visible);
        }

        if (visible)
        {
            m_FillImageTransform.localScale = new Vector3(m_Entity.Energy / m_Entity.MaxEnergy, 1f, 1f);
        }
    }
}
