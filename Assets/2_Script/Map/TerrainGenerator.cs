using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

using Photon.Pun;
using Photon.Realtime;

public class TerrainGenerator : MonoBehaviour
{
    public PhotonView PV;
    
    int [,] mapArray;
    float seed = 0f;
    public Tilemap tileMap;
    public TileBase tileBase;
    public int terrainWidth, terrainHeight;

    // 게임 시작 시 랜덤하게 TileMap 생성.
    public IEnumerator GenerateTerrain()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            seed = UnityEngine.Random.Range(.1f, 1f);
            PV.RPC("SynchronizationSeed", RpcTarget.All, seed);
        }

        while (seed == 0f) yield return new WaitForSeconds(0.5f);

        mapArray = GenerateArray(terrainWidth, terrainHeight, true);
        StartCoroutine(RenderMap(PerlinNoiseSmooth(mapArray, seed, 3), tileMap, tileBase));
    }

    /***  아래부터 전부 TileMap 생성 로직. ***/
    public static int[,] GenerateArray(int width, int height, bool empty)
    {
        int[,] map = new int[width, height];

        for (int x = 0; x < map.GetUpperBound(0); x++)
        {
            for (int y = 0; y < map.GetUpperBound(1); y++)
            {
                if (empty)
                    map[x, y] = 0;
                else
                    map[x, y] = 1;
            }
        }
        return map;
    }

    public static IEnumerator RenderMap(int[,] map, Tilemap tilemap, TileBase tile)
    {
        tilemap.ClearAllTiles();

        for (int x = 0; x < map.GetUpperBound(0); x++)
        {
            for (int y = 0; y < map.GetUpperBound(1); y++)
            {
                if (map[x, y] == 1)
                    tilemap.SetTile(new Vector3Int(x, y, 0), tile);
            }
        }
        yield return new WaitForSeconds(.1f);
    }

    public static int[,] PerlinNoise(int[,] map, float seed)
    {
        int newPoint;
        float reduction = 0.5f;

        for (int x = 0; x < map.GetUpperBound(0); x++)
        {
            newPoint = Mathf.FloorToInt((Mathf.PerlinNoise(x, seed) - reduction) * map.GetUpperBound(1));
            newPoint += (map.GetUpperBound(1) / 2);

            for (int y = newPoint; y >= 0; y--) map[x, y] = 1;
        }
        return map;
    }

    public static int[,] PerlinNoiseSmooth(int[,] map, float seed, int interval)
    {
        if (interval > 1)
        {
            int newPoint, points;
            float reduction = 0.5f;

            Vector2Int currentPos, lastPos;
            List<int> noiseX = new List<int>();
            List<int> noiseY = new List<int>();

            for (int x = 0; x < map.GetUpperBound(0); x += interval)
            {
                newPoint = Mathf.FloorToInt((Mathf.PerlinNoise(x, (seed * reduction))) * map.GetUpperBound(1));
                noiseY.Add(newPoint);
                noiseX.Add(x);
            }

            points = noiseY.Count;
            for (int i = 1; i < points; i++)
            {
                currentPos = new Vector2Int(noiseX[i], noiseY[i]);
                lastPos = new Vector2Int(noiseX[i - 1], noiseY[i - 1]);

                Vector2 diff = currentPos - lastPos;

                float heightChange = diff.y / interval;
                float currHeight = lastPos.y;

                for (int x = lastPos.x; x < currentPos.x; x++)
                {
                    for (int y = Mathf.FloorToInt(currHeight); y > 0; y--) map[x, y] = 1;
                    currHeight += heightChange;
                }
            }
        }
        else
            map = PerlinNoise(map, seed);
        
        return map;
    }

    // TileMap 생성 동기화.
    [PunRPC]
    public void SynchronizationSeed(float _seed) => seed = _seed;
    



}
