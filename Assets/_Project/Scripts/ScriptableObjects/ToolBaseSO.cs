using UnityEngine;

// [CreateAssetMenu(fileName = "ToolBaseSO", menuName = "Scriptable Objects/ToolBaseSO")]
public abstract class ToolBaseSO : ScriptableObject
{
    [field: Header("Tool Data")]
    [field: SerializeField] public string Name { get; private set; } = "TOOL";
    [field: SerializeField] public Sprite Icon { get; private set; }
    [field: SerializeField, Min(1)] public int Size { get; private set; } = 1;
    public int Radius => Size / 2;
    [field: SerializeField, TextArea] public string Description { get; private set; }

    public abstract void UseTool(TileData targetTile);

    private void OnValidate()
    {
        if (Size % 2 == 0)
        {
            Size += 1;
        }
    }
}
