using UnityEngine;

public abstract class EntityBase : MonoBehaviour
{
    protected TileData m_CurrentTile = null;
    public TileData CurrentTile => m_CurrentTile;

    private bool m_IsDead = false;
    public bool IsDead => m_IsDead;

    [Header("References")]
    [SerializeField] private Renderer m_Renderer;
    private MaterialPropertyBlock m_PropertyBlock;
    private const string DITHER_ALPHA = "_DitherAlpha";

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
    protected virtual void Awake()
    {
        if (m_Renderer == null) return;

        m_PropertyBlock = new MaterialPropertyBlock();
    }

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
        if (m_IsDead) return;

        m_Energy = m_Energy - amount;
        if (m_Energy <= 0f)
        {
            m_Energy = 0f;
            Die();
        }
    }

    protected virtual void Die()
    {
        if (m_IsDead) return;
        m_IsDead = true;

        m_CurrentTile?.AddNutrients(m_NutrientsOnDeath);
        TileData[] surroundingTiles = MapTileManager.Instance.GetSurroundingTiles(m_CurrentTile);
        foreach (TileData tile in surroundingTiles)
        {
            if (tile == null) continue;
            tile.AddNutrients(m_NutrientsOnDeath);
        }

        m_CurrentTile?.RemoveEntity(this);
        EntityManager.Instance.RemoveEntity(this);
        Destroy(gameObject);
    }
    #endregion

    #region Ticking
    protected virtual void OnTick()
    {
        if (m_IsDead) return;
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

    public void SetAlpha(float alpha)
    {
        if (m_Renderer == null || m_PropertyBlock == null) return;

        m_Renderer.GetPropertyBlock(m_PropertyBlock);
        m_PropertyBlock.SetFloat(DITHER_ALPHA, alpha);
        m_Renderer.SetPropertyBlock(m_PropertyBlock);
    }
}
