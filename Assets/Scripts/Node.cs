using UnityEngine;

[System.Serializable]
public class Node
{
    public Node parent;
    public Vector2Int position;
    public float weight;
    public bool passable;
    public int tileIndex;
    public int layer;

    public int gCost;
    public int hCost;

    public Node(Vector2Int _position, float _weight, bool _passable, int _tileIndex, int _layer)
    {
        position = _position;
        weight = _weight;
        passable = _passable;
        tileIndex = _tileIndex;
        layer = _layer;
    }

    public int fCost
    {
        get
        {
            return gCost + hCost;
        }
    }
}
