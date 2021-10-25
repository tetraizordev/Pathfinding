using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable] 
public class TileObject
{
    public string name;
    public Tile[] tile;
    public float weight;
    public bool passable;

    public TileObject(string _name, Tile[] _tileIndex, float _tileWeight, bool _passable) 
    {
        name = _name;
        tile = _tileIndex;
        weight = _tileWeight;
        passable = _passable;
    }
}
