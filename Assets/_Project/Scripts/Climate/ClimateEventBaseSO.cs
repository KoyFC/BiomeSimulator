using UnityEngine;

// [CreateAssetMenu(fileName = "ClimateEventBaseSO", menuName = "Scriptable Objects/ClimateEventBaseSO")]
public abstract class ClimateEventBaseSO : ScriptableObject
{
    public abstract string EventName { get; protected set; }
    public abstract int DurationTicks { get; protected set; }

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
