using UnityEngine;

// [CreateAssetMenu(fileName = "ClimateEventBaseSO", menuName = "Scriptable Objects/ClimateEventBaseSO")]
public abstract class ClimateEventBaseSO : ScriptableObject
{
    [field: SerializeField] public string EventName { get; protected set; } = "Event";
    [field: SerializeField] public int DurationTicks { get; protected set; } = 10;
    [field: SerializeField, TextArea] public string Description { get; protected set; } = "";

    protected int m_TicksRemaining;
    public bool IsFinished => m_TicksRemaining <= 0;

    public virtual void StartEvent()
    {
        m_TicksRemaining = DurationTicks;
    }

    public virtual void OnTick()
    {
        if (IsFinished) return;
        m_TicksRemaining--;
        ExecuteTickEffect();
    }

    protected abstract void ExecuteTickEffect();
    public virtual void EndEvent() { }
}
