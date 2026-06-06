using UnityEngine;

// [CreateAssetMenu(fileName = "ToolBaseSO", menuName = "Scriptable Objects/ToolBaseSO")]
public abstract class ToolBaseSO : ScriptableObject
{
    public abstract void UseTool(TileData targetTile);
}
