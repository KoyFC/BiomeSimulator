using UnityEngine;

public abstract class EntityBase : MonoBehaviour
{
    protected float m_Energy = 0f; // 0 to 100
    protected TileData m_CurrentTile = null;
    protected Vector3 m_TargetPosition = Vector3.zero;

    [Header("Movement")]
    [SerializeField, Min(0f)] private float m_TimeToMoveBetweenTiles = 0.5f;
    private float m_MoveTimer = 0f;

    [Header("Energy")]
    [SerializeField, Min(0f)] protected float m_EnergyConsumptionPerSecond = 1f;

    protected virtual void Update()
    {
        if (m_CurrentTile == null) return;

        if (m_MoveTimer <= m_TimeToMoveBetweenTiles)
        {
            m_MoveTimer += Time.deltaTime;
            float t = Mathf.Clamp01(m_MoveTimer / m_TimeToMoveBetweenTiles);
            transform.position = Vector3.Lerp(transform.position, m_TargetPosition, t);
        }
    }

    // TODO: Use a static event to use ticks and divide by tick rate
    protected virtual void OnTick()
    {
        // ConsumeEnergy(m_EnergyConsumptionPerSecond);
    }

    protected void AddEnergy(float amount)
    {
        m_Energy = Mathf.Clamp(m_Energy + amount, 0f, 100f);
    }

    protected void ConsumeEnergy(float amount)
    {
        m_Energy = Mathf.Max(0f, m_Energy - amount);
        if (m_Energy == 0f)
        {
            Die();
        }
    }

    protected void MoveToTile(TileData tile)
    {
        if (tile == null) return;

        m_CurrentTile?.RemoveEntity(this);
        m_CurrentTile = tile;
        m_CurrentTile.AddEntity(this);

        m_TargetPosition = tile.WorldPosition;
        m_MoveTimer = 0f;
    }

    protected virtual void Die()
    {
        Destroy(gameObject);
    }
}
