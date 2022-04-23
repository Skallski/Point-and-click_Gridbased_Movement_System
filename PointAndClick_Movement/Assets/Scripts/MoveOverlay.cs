using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MoveOverlay : MonoBehaviour
{
    [HideInInspector] public List<GridTile> tilesInsideMoveRange = new List<GridTile>();

    /// <summary>
    /// Gets tiles in given range of given tile 
    /// </summary>
    /// <param name="startingTile"> Grid tile considered as starting tile. </param>
    /// <param name="rangeOfTiles"> value that specifies the range according to which tiles will be checked. </param>
    /// <returns>  </returns>
    private List<GridTile> FindTilesInRange(GridTile startingTile, int rangeOfTiles)
    {
        var tilesInRange = new List<GridTile> {startingTile};
        var previousStepTiles = new List<GridTile> {startingTile};
        
        int stepCount = 0;

        while (stepCount < rangeOfTiles)
        {
            var neighbourTiles = new List<GridTile>();
    
            foreach (var tile in previousStepTiles)
            {
                neighbourTiles.AddRange(Gridmap.self.FindNeighbourTiles(tile));
            }
            
            tilesInRange.AddRange(neighbourTiles);
            previousStepTiles = neighbourTiles.Distinct().ToList();
            stepCount += 1;
        }

        return tilesInRange.Distinct().ToList();
    }
    
    /// <summary>
    /// Shows grid tiles, which are surrounding the given tile in given range
    /// </summary>
    /// <param name="startingTile"> Grid tile considered as starting tile. </param>
    /// <param name="rangeOfTiles"> value that specifies how many grid tiles from "starting tile" are to be shown. </param>
    public void ShowSurroundingTilesForMove(GridTile startingTile, int rangeOfTiles)
    {
        if (tilesInsideMoveRange.Count > 0)
        {
            foreach (var tile in tilesInsideMoveRange)
            {
                tile.Hide();
            }
        }

        tilesInsideMoveRange = FindTilesInRange(startingTile, rangeOfTiles);
        var tilesInCloseRange = FindTilesInRange(startingTile, rangeOfTiles - 1);
    
        foreach (var tile in tilesInsideMoveRange)
        {
            tile.Show();
            
            // set the farthest grid tiles as yellow
            if (!tilesInCloseRange.Contains(tile))
                tile.Highlight(Color.yellow);
        }
    }

}