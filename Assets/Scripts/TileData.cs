using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

// Scriptable object to store tile properties, mainly the tile speed modifier
[CreateAssetMenu]

public class TileData : ScriptableObject
{
    public TileBase[] tiles;
    public float speedMod;
}
