using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    [HideInInspector] public List<GridTile> path = new List<GridTile>();
    
    /// <summary>
    /// Finds path from end tile to start tile
    /// </summary>
    /// <param name="startingTile"> Grid tile considered as starting tile. </param>
    /// <param name="endTile"> Grid tile considered as ending tile. </param>
    /// <returns> List of grid tiles, which is the path from end tile to start tile. </returns>
    public List<GridTile> FindPath(GridTile startingTile, GridTile endTile)
    {
        List<GridTile> unvisitedTiles = new List<GridTile>();
        List<GridTile> visitedTiles = new List<GridTile>();
        
        unvisitedTiles.Add(startingTile);

        while (unvisitedTiles.Count > 0)
        {
            GridTile currentTile = unvisitedTiles.OrderBy(p => p.node.fCost).First(); // tile with the lowest fCost

            unvisitedTiles.Remove(currentTile);
            visitedTiles.Add(currentTile);

            if (currentTile == endTile)
            {
                // returns found path
                List<GridTile> foundPath = new List<GridTile>();
                GridTile current = endTile;
                
                while (current != startingTile)
                {
                    if (!foundPath.Contains(current))
                        foundPath.Add(current);
                    
                    current = current.node.previousTile;  
                }
                
                foundPath.Reverse(); // need to reverse, because we are going from end to start
                return foundPath;
            }

            foreach (var neighbourTile in Gridmap.self.FindNeighbourTiles(currentTile).Where(neighbourTile => !visitedTiles.Contains(neighbourTile)))
            {
                neighbourTile.node.gCost = CalculateDistance(startingTile, neighbourTile);
                neighbourTile.node.hCost = CalculateDistance(endTile, neighbourTile);

                neighbourTile.node.previousTile = currentTile;

                if (!unvisitedTiles.Contains(neighbourTile))
                    unvisitedTiles.Add(neighbourTile);
            }
        }
        
        return new List<GridTile>();
    }

    /// <summary>
    /// Calculates distance between two grid tiles
    /// </summary>
    /// <param name="tileA"> First grid tile to process. </param>
    /// <param name="tileB"> Second grid tile to process. </param>
    /// <returns> int value of distance between first and second grid tile. </returns>
    private int CalculateDistance(GridTile tileA, GridTile tileB)
    {
        return Mathf.Abs(tileA.node.gridCoordinates.x - tileB.node.gridCoordinates.x) + Mathf.Abs(tileA.node.gridCoordinates.y - tileB.node.gridCoordinates.y);
    }
    
}