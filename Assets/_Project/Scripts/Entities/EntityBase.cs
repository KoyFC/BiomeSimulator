using UnityEngine;

public abstract class EntityBase : MonoBehaviour
{
    [SerializeField] private float m_InitialEnergy = 100f;
    protected float m_Energy = 0f; // 0 to 100
    protected TileData m_CurrentTile = null;

    [Header("Energy")]
    [SerializeField, Min(0f)] protected float m_EnergyConsumptionPerSecond = 1f;

    public virtual void Initialize(TileData startingTile)
    {
        m_Energy = m_InitialEnergy;

        SetCurrentTile(startingTile);
        transform.position = startingTile.WorldPosition;
    }

    #region Unity Methods
    protected virtual void OnEnable()
    {
        TickManager.OnTick += OnTick;
    }

    protected virtual void OnDisable()
    {
        TickManager.OnTick -= OnTick;
    }

    protected virtual void Update()
    {

    }
    #endregion

    #region Energy
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

    protected virtual void Die()
    {
        Destroy(gameObject);
    }
    #endregion

    #region Ticking
    protected virtual void OnTick()
    {
        ConsumeEnergy(m_EnergyConsumptionPerSecond * TickManager.TickTime);
    }
    #endregion

    protected void SetCurrentTile(TileData tile)
    {
        if (tile == null) return;

        m_CurrentTile?.RemoveEntity(this);
        m_CurrentTile = tile;
        m_CurrentTile.AddEntity(this);
    }
}
