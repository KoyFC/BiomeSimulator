using UnityEngine;

[CreateAssetMenu(fileName = "Entity Data", menuName = "Scriptable Objects/Entity Data")]
public class EntityDataSO : ScriptableObject
{
    [Header("Entity")]
    [field: SerializeField] public string Name { get; private set; } = "ENTITY";
    [field: SerializeField] public Sprite Sprite { get; private set; }
    [field: SerializeField] public EntityBase Prefab { get; private set; }
}
