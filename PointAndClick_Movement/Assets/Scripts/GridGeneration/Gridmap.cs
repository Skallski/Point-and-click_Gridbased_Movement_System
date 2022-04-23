using System;
using System.Collections.Generic;
using SkalluUtils.Extensions;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Gridmap : MonoBehaviour
{
    private static Gridmap _self;
    public static Gridmap self => _self;

    #region Inspector Fields
    [Tooltip("gridTile prefab script reference")]
    public GameObject gridTilePrefab;
    
    [Tooltip("container to store every gridTile object")]
    public GameObject gridTileContainerObj;
    #endregion

    public Dictionary<Vector2Int, GridTile> terrainMap;
    
    private void Awake()
    {
        if (_self != null && _self != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _self = this;
            
            terrainMap = new Dictionary<Vector2Int, GridTile>();
        }
    }
    
    private void Start()
    {
        var tileMap = gameObject.GetComponentInChildren<Tilemap>();

        if (tileMap != null)
            BuildGridMap(tileMap);
        else
            throw new Exception("There is no suitable tile map to build grid map!".Color(Color.red));
    }

    /// <summary>
    /// Builds grid map based on "walkable" tile map
    /// </summary>
    /// <param name="tileMap"> Tile map, from which we want to build grid map. </param>
    private void BuildGridMap(Tilemap tileMap)
    {
        var tileMapBounds = tileMap.cellBounds;
        
        // iterates through all tiles inside tilemap
        for (int z = tileMapBounds.min.z; z <= tileMapBounds.max.z; z++)
        {
            for (int y = tileMapBounds.min.y; y < tileMapBounds.max.y; y++)
            {
                for (int x = tileMapBounds.min.x; x < tileMapBounds.max.x; x++)
                {
                    var tileCoordinates = new Vector3Int(x, y, z);

                    if (tileMap.HasTile(tileCoordinates) && !terrainMap.ContainsKey((Vector2Int) tileCoordinates))
                    {
                        // instantiates grid tile, sets its position, grid coordinates and adds it to terrain map
                        var gridTile = Instantiate(gridTilePrefab, gridTileContainerObj.transform).GetComponent<GridTile>();
                        
                        var gridTileObjWorldPos = tileMap.GetCellCenterWorld(tileCoordinates);
                        gridTile.transform.position = new Vector3(gridTileObjWorldPos.x, gridTileObjWorldPos.y, gridTileObjWorldPos.z + 1);
                        gridTile.node.gridCoordinates = tileCoordinates;
                        
                        terrainMap.Add((Vector2Int) tileCoordinates, gridTile);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Gets neighbours of the given tile
    /// </summary>
    /// <param name="tileToCheck"> Grid tile, whose neighbours we want to get. </param>
    /// <returns> List of grid tiles, which are the neighbours of the given tile. </returns>
    public List<GridTile> FindNeighbourTiles(GridTile tileToCheck)
    {
        List<GridTile> neighbourTilesList = new List<GridTile>();
        
        Vector2Int coordinatesToCheck = new Vector2Int(tileToCheck.node.gridCoordinates.x, tileToCheck.node.gridCoordinates.y);

        CheckNeighbour(new Vector2Int(coordinatesToCheck.x, coordinatesToCheck.y + 1), neighbourTilesList); // top neighbour tile
        CheckNeighbour(new Vector2Int(coordinatesToCheck.x, coordinatesToCheck.y - 1), neighbourTilesList); // bottom neighbour tile
        CheckNeighbour(new Vector2Int(coordinatesToCheck.x + 1, coordinatesToCheck.y), neighbourTilesList); // right neighbour tile
        CheckNeighbour(new Vector2Int(coordinatesToCheck.x - 1, coordinatesToCheck.y), neighbourTilesList); // left neighbour tile

        return neighbourTilesList;
    }

    /// <summary>
    /// Checks if there is neighbour tile based on given coordinates
    /// </summary>
    /// <param name="coordinatesToCheck"> Coordinates of grid tile, we want to check. </param>
    /// <param name="neighbourTilesList"> List of neighbour tiles. </param>
    private void CheckNeighbour(Vector2Int coordinatesToCheck, List<GridTile> neighbourTilesList)
    {
        if (terrainMap.ContainsKey(coordinatesToCheck))
            neighbourTilesList.Add(terrainMap[coordinatesToCheck]);
        else
            return;
    }

}