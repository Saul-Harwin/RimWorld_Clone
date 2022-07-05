using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding
{

    private List<Tile> openList;
    private List<Tile> closedList;

    int FindManhattanDistance(Vector2Int firstPos, Vector2Int secondPos){
        return (
            Mathf.Abs(firstPos.x - secondPos.x) +
            Mathf.Abs(firstPos.y - secondPos.y)
        );
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
        }

        // Right
        if(tile.position.x + 1 < world.width && world.tiles[tile.position.x + 1, tile.position.y].tileData.walkable){
            neighbourList.Add(world.tiles[tile.position.x + 1, tile.position.y]);
        }

        // Down
        if(tile.position.y - 1 >= 0 && world.tiles[tile.position.x, tile.position.y - 1].tileData.walkable){
            neighbourList.Add(world.tiles[tile.position.x, tile.position.y - 1]);
        }

        // Up
        if(tile.position.y + 1 < world.height && world.tiles[tile.position.x, tile.position.y + 1].tileData.walkable){
            neighbourList.Add(world.tiles[tile.position.x, tile.position.y + 1]);
        }

        return neighbourList;
    }

    public List<Tile> FindPath(Tile start, Tile end){
        World world = GameManager.Instance.world;
        openList = new List<Tile> { start };
        closedList = new List<Tile>();

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