public interface IThreat
{
    ThreatType ThreatType { get; }
}

public enum ThreatType { PREDATOR, ENVIRONMENT }
