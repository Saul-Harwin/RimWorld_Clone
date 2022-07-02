using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    public List<Unit> units = new List<Unit>();
    [SerializeField] int startingUnitCount;
    [SerializeField] int spawnRangeFromCentre;
    [SerializeField] GameObject unitPrefab;
    [SerializeField] Unit testUnit;

    public void SpawnUnits(){
        Vector2Int centerTile = new Vector2Int(
            GameManager.Instance.world.width / 2,
            GameManager.Instance.world.height / 2
        );
        int minX = centerTile.x - spawnRangeFromCentre;
        int maxX = centerTile.x + spawnRangeFromCentre;
        int minY = centerTile.y - spawnRangeFromCentre;
        int maxY = centerTile.y + spawnRangeFromCentre;
        for (int i = 0; i < startingUnitCount; i++)
        {
            Unit unit = testUnit;
            Vector2Int spawnPos = new Vector2Int();
            int count = 0;
            bool foundSpawn = false;
            while(!foundSpawn){
                if(count > 100){
                    Debug.Log("LOOP INFINTE");
                    break;
                }
                Vector2Int rp = new Vector2Int(
                    Random.Range(minX, maxX),
                    Random.Range(minY, maxY)
                );
                if(!GameManager.Instance.world.tiles[rp.x,rp.y].occupied){
                    foundSpawn = true;
                    GameManager.Instance.world.tiles[rp.x,rp.y].occupied = true;
                    spawnPos = rp;
                }
                count++;
            }
            
            unit.Initialize(
                spawnPos, 
                unitPrefab
            );
            units.Add(unit);
        }
    }
}
