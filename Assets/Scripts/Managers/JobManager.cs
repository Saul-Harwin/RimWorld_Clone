using System.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class JobManager : MonoBehaviour
{
    public List<Job> jobs;

    void Start(){
        jobs = new List<Job>();
    }

    void Update(){
        CreateJobs();
        AssignJobs();
        ExecuteJobs();
    }

    void CreateJobs(){
        // Harvesting
        if(GameManager.Instance.world.objects.Count != 0){
            foreach(HarvestableObject ho in GameManager.Instance.world.objects.OfType<HarvestableObject>()){
                if(ho.markedForHarvest && !ho.currentlyBeingHarvested){
                    jobs.Add(new HarvestObjectJob(ho));
                    ho.markedForHarvest = false;
                    ho.currentlyBeingHarvested = true;
                }
            }

        // Hauling
            foreach(HaulableObject ho in GameManager.Instance.world.objects.OfType<HaulableObject>()){
                if(Stockpile.freeSpaces() != 0){
                    if(ho.markedForHauling && !ho.currentlyBeingHauled){
                        jobs.Add(new HaulObjectJob(ho));
                        ho.markedForHauling = false;
                        ho.currentlyBeingHauled = true;
                    }
                }
            }
        }
    }

    void AssignJobs(){
        for (int i = 0; i < GameManager.Instance.unitManager.units.Count; i++)
        {
            if(GameManager.Instance.unitManager.units[i].state == UnitState.IDLE){
                for (int j = 0; j < jobs.Count; j++)
                {
                    if(jobs[j].assignedUnit == null && GameManager.Instance.unitManager.units[i].currentJob == null){
                        GameManager.Instance.unitManager.units[i].currentJob = jobs[j];
                        jobs[j].assignedUnit = GameManager.Instance.unitManager.units[i];
                    }
                }
            }
        }
    }

    void ExecuteJobs()
    {
        for (int j = 0; j < jobs.Count; j++){
            if(!jobs[j].beingExecuted && jobs[j].assignedUnit != null){
                jobs[j].Execute();
            }
        }
    }
}

[System.Serializable]
abstract public class Job {
    public JobType type;
    public Unit assignedUnit;
    public bool beingExecuted;
    public abstract void Execute();
    public abstract void CancelJob();
}

[System.Serializable]
public class HarvestObjectJob : Job{
    public HarvestableObject obj;
    public HarvestObjectJob(HarvestableObject objectToHarvest){
        type = JobType.HARVESTOBJECT;
        obj = objectToHarvest;
        obj.associatedJob = this;
    }

    public async override void Execute(){
        if(GameManager.Instance.pathfinder.FindPath(assignedUnit.occupyingTile, obj.occupyingTile) == null){
            Debug.Log("No Path!");
            GameManager.Instance.jobManager.jobs.Remove(this);
            obj.currentlyBeingHarvested = false;
            assignedUnit.FreeUnitFromJob();
        }
        beingExecuted = true;
        obj.associatedJob = this;
        assignedUnit.state = UnitState.HARVESTING;
        assignedUnit.go.GetComponent<UnitGO>().PathToTile(obj.occupyingTile);
        while(assignedUnit.occupyingTile != obj.occupyingTile){
            await Task.Yield();
        }
        // Drop Harvested Object
        HaulableObject droppedObject = HaulableObject.CreateNewObject(obj.droppedObject, obj.occupyingTile);
        droppedObject.markedForHauling = true;

        // Delete Object
        GameManager.Instance.world.objects.Remove(obj);
        GameObject.Destroy(obj.go);
        GameObject.Destroy(obj);

        // Remove Job
        GameManager.Instance.jobManager.jobs.Remove(this);

        // Free Unit From Job
        assignedUnit.currentJob = null;
        assignedUnit.state = UnitState.IDLE;
    }

    public override void CancelJob()
    {
        if(assignedUnit != null){
            assignedUnit.currentJob = null;
            assignedUnit.state = UnitState.IDLE;
            assignedUnit.go.GetComponent<UnitGO>().cancelPathing = true;
        }
        obj.currentlyBeingHarvested = false;
        obj.associatedJob = null;
        GameManager.Instance.jobManager.jobs.Remove(this);
    }
}

[System.Serializable]
public class HaulObjectJob : Job{
    public HaulableObject obj;
    public HaulObjectJob(HaulableObject objectToHaul){
        type = JobType.HAUL;
        obj = objectToHaul;
        obj.associatedJob = this;
    }

    public async override void Execute(){
        if(GameManager.Instance.pathfinder.FindPath(assignedUnit.occupyingTile, obj.occupyingTile) == null){
            Debug.Log("No Path!");
            GameManager.Instance.jobManager.jobs.Remove(this);
            obj.currentlyBeingHauled = false;
            assignedUnit.FreeUnitFromJob();
        }
        beingExecuted = true;
        StockpileTile reservedTile = Stockpile.findFreeSpace();
        if(reservedTile == null){
            return;
        }
        reservedTile.reserved = true;

        // Path to ground objects tile
        // and wait for the unit to reach
        // -------------------------------
        assignedUnit.go.GetComponent<UnitGO>().PathToTile(obj.occupyingTile);
        while(assignedUnit.occupyingTile != obj.occupyingTile){
            await Task.Yield();
        }

        // Pickup Object From Floor:
        // Create a copy in Unit
        // Delete object from world
        // -------------------------------
        assignedUnit.haulingObject = ScriptableObject.Instantiate(obj);

        GameObject.Destroy(obj.go);
        obj = null;

        // Path to reserved StockpileTile
        // Wait For Unit To Reach reserved
        // StockpileTile
        // -------------------------------
        assignedUnit.go.GetComponent<UnitGO>().PathToTile(reservedTile.tile);
        while(assignedUnit.occupyingTile != reservedTile.tile){
            await Task.Yield();
        }

        // Drop Object Held By Unit:
        // Create a copy on reservedTile
        // Set haulingObject to null
        // -------------------------------
        obj = HaulableObject.CreateNewObject(ScriptableObject.Instantiate(assignedUnit.haulingObject), assignedUnit.occupyingTile);
        assignedUnit.haulingObject = null;

        // Reserved Tile
        reservedTile.reserved = false;

        // Unit
        assignedUnit.currentJob = null;
        assignedUnit.state = UnitState.IDLE;

        // Object
        obj.stockpile = reservedTile.stockpile;
        obj.markedForHauling = false;
        obj.currentlyBeingHauled = false;
        obj.associatedJob = null;
    }

    public override void CancelJob(){
        // Unit
        if(assignedUnit != null){
            assignedUnit.currentJob = null;
            assignedUnit.state = UnitState.IDLE;
            assignedUnit.go.GetComponent<UnitGO>().cancelPathing = true;
        }

        // Hauling Object
        if(assignedUnit.haulingObject != null){
            HaulableObject newObj;
            if(assignedUnit.occupyingTile.occupyingObject == null){
                newObj = HaulableObject.CreateNewObject(assignedUnit.haulingObject, assignedUnit.occupyingTile);
            }
            else{
                newObj = HaulableObject.CreateNewObject(assignedUnit.haulingObject, World.findNearbyEmptyTile(assignedUnit.occupyingTile));
            }
            newObj.markedForHauling = true;
            newObj.currentlyBeingHauled = false;
            obj.associatedJob = null;
        }

        // Object
        if (obj != null){
            obj.markedForHauling = true;
            obj.currentlyBeingHauled = false;
            obj.associatedJob = null;
        }

        // Job
        GameManager.Instance.jobManager.jobs.Remove(this);
    }
    
}

public enum JobType {
    MOVE,
    HARVESTOBJECT,
    HAUL,
}