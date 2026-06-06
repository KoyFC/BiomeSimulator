using UnityEngine;

public abstract class EntityBase : MonoBehaviour
{
    protected TileData m_CurrentTile = null;
    public TileData CurrentTile => m_CurrentTile;

    [Header("Energy")]
    [SerializeField] private float m_InitialEnergy = 100f;
    [SerializeField] protected float m_MaxEnergy = 100f;
    [SerializeField, Min(0f)] protected float m_EnergyConsumptionPerSecond = 1f;
    [SerializeField, Range(0f, 1f)] protected float m_DigestionEfficiency = 0.9f; // How much of the energy is actually gained when consuming food / nutrients
    protected float m_Energy = 0f;

    [Header("Death")]
    [SerializeField] private float m_NutrientsOnDeath = 20f; // How much energy is released to the tile when the entity dies

    [Header("Reproduction")]
    [SerializeField, Range(0f, 1f)] protected float m_ReproductionThreshold = 0.9f;
    [SerializeField, Range(0f, 1f)] protected float m_ReproductionEnergyCost = 0.5f;

    public float Energy => m_Energy;
    public float MaxEnergy => m_MaxEnergy;

    public virtual void Initialize(TileData startingTile)
    {
        m_Energy = m_InitialEnergy;

        SetCurrentTile(startingTile);
        transform.position = startingTile.WorldPosition;
        EntityManager.Instance.AddEntity(this);
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
    public void AddEnergy(float amount)
    {
        m_Energy = Mathf.Clamp(m_Energy + amount, 0f, m_MaxEnergy);
    }

    protected void ConsumeEnergy(float amount)
    {
        m_Energy = m_Energy - amount;
        if (m_Energy <= 0f)
        {
            m_Energy = 0f;
            Die();
        }
    }

    protected virtual void Die()
    {
        m_CurrentTile?.AddNutrients(m_NutrientsOnDeath);
        m_CurrentTile?.RemoveEntity(this);
        EntityManager.Instance.RemoveEntity(this);
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
