using UnityEngine;

public abstract class EntityBase : MonoBehaviour
{
    protected float m_Energy = 0f; // 0 to 100

    protected void AddEnergy(float amount)
    {
        m_Energy = Mathf.Clamp(m_Energy + amount, 0f, 100f);
    }

    protected void ConsumeEnergy(float amount)
    {
        m_Energy = Mathf.Max(0f, m_Energy - amount);
    }
}
