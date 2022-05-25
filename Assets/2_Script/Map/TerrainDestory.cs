using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

using Photon.Pun;
using Photon.Realtime;

public class TerrainDestory : MonoBehaviour
{
    public Tilemap terrain;
    public static TerrainDestory instance;

    private void Awake() => instance = this;
    
    // 공격에 명중될 시 인근 TileMap 제거.
    public void DestroyTerrain(Vector3 explosionLocation, float radius) 
    {
        for (int x = -(int)radius; x < radius; x++)
        {
            for (int y = -(int)radius; y < radius; y++)
            {
                Vector3Int tilePos = terrain.WorldToCell(explosionLocation + new Vector3(x, y, 0));
                if (terrain.GetTile(tilePos) != null)
                    terrain.SetTile(tilePos, null);
            }
        }
    }

}
