using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding
{

    private const int MOVE_STRAIGHT_COST = 10;
    private const int MOVE_DIAGONAL_COST = 14;

    private List<Tile> openList;
    private HashSet<Tile> closedList;

    public static int FindManhattanDistance(Vector2Int a, Vector2Int b){
        int xDistance = Mathf.Abs(a.x - b.x);
        int yDistance = Mathf.Abs(a.y - b.y);
        int remaining = Mathf.Abs(xDistance - yDistance);
        return MOVE_DIAGONAL_COST * Mathf.Min(xDistance, yDistance) + MOVE_STRAIGHT_COST * remaining;
    }
    Tile GetLowestFCostTile(List<Tile> tileList){
        Tile lowestFCostTile = tileList[0];
        for (int i = 1; i < tileList.Count; i++)
        {
           if(tileList[i].pathFindingData.fCost < lowestFCostTile.pathFindingData.fCost) {
                lowestFCostTile = tileList[i];
           }
        }
        return lowestFCostTile;
    }

    List<Tile> CalculatePath(Tile end){
        List<Tile> path = new List<Tile>();
        path.Add(end);
        Tile currentTile = end;
        while(currentTile.pathFindingData.cameFromTile != null){
            path.Add(currentTile.pathFindingData.cameFromTile);
            currentTile = currentTile.pathFindingData.cameFromTile;
        }
        path.Reverse();
        return path;
    }

    List<Tile> GetNeighbourList(Tile tile){
        List<Tile> neighbourList = new List<Tile>();
        World world = GameManager.Instance.world;

        // Left
        if(tile.position.x - 1 >= 0 && world.tiles[tile.position.x - 1, tile.position.y].tileData.walkable){
            neighbourList.Add(world.tiles[tile.position.x - 1, tile.position.y]);
            // LDown
            if(tile.position.y - 1 >= 0 && world.tiles[tile.position.x, tile.position.y - 1].tileData.walkable){
                neighbourList.Add(world.tiles[tile.position.x - 1, tile.position.y - 1]);
            }
            // LUp
            if(tile.position.y + 1 < world.height && world.tiles[tile.position.x, tile.position.y + 1].tileData.walkable){
                neighbourList.Add(world.tiles[tile.position.x - 1, tile.position.y + 1]);
            }
        }

        // Right
        if(tile.position.x + 1 < world.width && world.tiles[tile.position.x + 1, tile.position.y].tileData.walkable){
            neighbourList.Add(world.tiles[tile.position.x + 1, tile.position.y]);
            // RDown
            if(tile.position.y - 1 >= 0 && world.tiles[tile.position.x, tile.position.y - 1].tileData.walkable){
                neighbourList.Add(world.tiles[tile.position.x + 1, tile.position.y - 1]);
            }
            // RUp
            if(tile.position.y + 1 < world.height && world.tiles[tile.position.x, tile.position.y + 1].tileData.walkable){
                neighbourList.Add(world.tiles[tile.position.x + 1, tile.position.y + 1]);
            }
        }

        // Down
        if(tile.position.y - 1 >= 0){
            neighbourList.Add(world.tiles[tile.position.x, tile.position.y - 1]);
        }

        // Up
        if(tile.position.y + 1 < world.height){
            neighbourList.Add(world.tiles[tile.position.x, tile.position.y + 1]);
        }

        return neighbourList;
    }

    public List<Tile> FindPath(Tile start, Tile end){
        World world = GameManager.Instance.world;
        openList = new List<Tile> { start };
        closedList = new HashSet<Tile>();

        for (int x = 0; x < world.width; x++)
        {
            for (int y = 0; y < world.height; y++){
                Tile tile = world.tiles[x,y];
                tile.pathFindingData.gCost = int.MaxValue;
                tile.pathFindingData.CalculateFCost();
                tile.pathFindingData.cameFromTile = null;
            }
        }

        start.pathFindingData.gCost = 0;
        start.pathFindingData.hCost = FindManhattanDistance(start.position, end.position);
        start.pathFindingData.CalculateFCost();

        while(openList.Count > 0){
            Tile currentTile = GetLowestFCostTile(openList);
            if(currentTile == end){
                return CalculatePath(end);
            }
            openList.Remove(currentTile);
            closedList.Add(currentTile);

            foreach (Tile neighbourTile in GetNeighbourList(currentTile)){
                if(closedList.Contains(neighbourTile)) continue;
                if(!neighbourTile.tileData.walkable || neighbourTile.occupyingUnit != null){
                    closedList.Add(neighbourTile);
                    continue;
                }

                int tentativeGCost =
                currentTile.pathFindingData.gCost +
                FindManhattanDistance(currentTile.position, neighbourTile.position);

                if(tentativeGCost < neighbourTile.pathFindingData.gCost){
                    neighbourTile.pathFindingData.cameFromTile = currentTile;
                    neighbourTile.pathFindingData.gCost = tentativeGCost;
                    neighbourTile.pathFindingData.hCost = FindManhattanDistance(
                        neighbourTile.position,
                        end.position
                    );
                    neighbourTile.pathFindingData.CalculateFCost();
                    if(currentTile.pathFindingData.fCost > 2000) return null;   // BAD METHOD NEED TO BE CHANGED I THINK
                    if(!openList.Contains(neighbourTile)){
                        openList.Add(neighbourTile);
                    }
                }

            }
        }
        // Out of nodes on open list
        return null;
    }
}
