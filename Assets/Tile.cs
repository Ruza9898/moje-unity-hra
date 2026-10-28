using UnityEngine;

public enum TileType
{
    Normal,
    Banana,
    Radiation,
    Finish
}

public class Tile : MonoBehaviour
{
    [SerializeField] private TileType type;

    public TileType Type => type;
}
