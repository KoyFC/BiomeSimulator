public interface IConsumable
{
    public float AvailableEnergy { get; }
    public ConsumableType ConsumableType { get; }
    public float Consume(float biteSize);
}

public enum ConsumableType { PLANT, MEAT }
