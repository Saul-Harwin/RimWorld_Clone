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
        int _count = 0;
        Vector2Int spawnAreaCenter = new Vector2Int();
        bool foundSpawnAreaCenter = false;
        while(!foundSpawnAreaCenter){
            if(_count > 100){
                Debug.Log("LOOP INFINTE");
                break;
            }

            if(GameManager.Instance.world.tiles[centerTile.x,centerTile.y].tileData.type == 2){
                foundSpawnAreaCenter = true;
                spawnAreaCenter = centerTile;
            }
            else {
                int rx = Random.Range(0 + spawnRangeFromCentre, GameManager.Instance.world.width - spawnRangeFromCentre);
                int ry = Random.Range(0 + spawnRangeFromCentre, GameManager.Instance.world.height - spawnRangeFromCentre);
                if(GameManager.Instance.world.tiles[rx,ry].tileData.type == 2){
                    foundSpawnAreaCenter = true;
                    spawnAreaCenter = new Vector2Int(rx,ry);
                }
            }
            _count++;
        }
        int minX = spawnAreaCenter.x - spawnRangeFromCentre;
        int maxX = spawnAreaCenter.x + spawnRangeFromCentre;
        int minY = spawnAreaCenter.y - spawnRangeFromCentre;
        int maxY = spawnAreaCenter.y + spawnRangeFromCentre;
        for (int i = 0; i < startingUnitCount; i++)
        {
            Unit unit = ScriptableObject.Instantiate(testUnit);
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
                if(GameManager.Instance.world.tiles[rp.x,rp.y].occupyingUnit == null &&
                    GameManager.Instance.world.tiles[rp.x,rp.y].tileData.type == 2){
                    foundSpawn = true;
                    GameManager.Instance.world.tiles[rp.x,rp.y].SetOccupyingUnit(unit);
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
