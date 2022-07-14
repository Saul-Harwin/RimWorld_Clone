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
    public bool cancelled = false;
    public abstract void Execute();
    public abstract void RemoveJob();
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
            if(cancelled){
                obj.currentlyBeingHarvested = false;
                obj.associatedJob = null;
                assignedUnit.currentJob = null;
                assignedUnit.state = UnitState.IDLE;
                assignedUnit.go.GetComponent<UnitGO>().cancelPathing = true;
                obj.currentlyBeingHarvested = false;
                cancelled = false;
                return;
            } 
            await Task.Yield();
        }
        // Drop Harvested Object
        HaulableObject.CreateNewObject(obj.droppedObject, obj.position, obj.occupyingTile);

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

    public override void RemoveJob()
    {
        // Remove Job
        GameManager.Instance.jobManager.jobs.Remove(this);
        cancelled = true;
        obj.currentlyBeingHarvested = false;
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
        obj.markedForHauling = false;
        obj.currentlyBeingHauled = true;
        beingExecuted = true;
        obj.associatedJob = this;
        assignedUnit.state = UnitState.HAULING;
        assignedUnit.go.GetComponent<UnitGO>().PathToTile(obj.occupyingTile);
        while(assignedUnit.occupyingTile != obj.occupyingTile){
            if(cancelled){
                obj.associatedJob = null;
                assignedUnit.currentJob = null;
                assignedUnit.state = UnitState.IDLE;
                assignedUnit.go.GetComponent<UnitGO>().cancelPathing = true;
                obj.currentlyBeingHauled = false;
                obj.markedForHauling = true;
                HaulableObject.CreateNewObject(assignedUnit.haulingObject, assignedUnit.position, assignedUnit.occupyingTile);
                assignedUnit.haulingObject = null;
                cancelled = false;
                return;
            } 
            await Task.Yield();
        }
        // Pick up Floor Object
        if(obj == null){
            assignedUnit.currentJob = null;
            assignedUnit.state = UnitState.IDLE;
            assignedUnit.go.GetComponent<UnitGO>().cancelPathing = true;
            assignedUnit.haulingObject = null;
            return;
        }
        assignedUnit.haulingObject = ScriptableObject.Instantiate(obj);

        // Delete Floor Object
        GameManager.Instance.world.objects.Remove(obj);
        GameObject.Destroy(obj.go);
        GameObject.Destroy(obj);

        Tile HaulToTile = null;
        // Haul To Free Space
        bool standingOnEmptyStockpileTile = false;
        while(!standingOnEmptyStockpileTile){
            HaulToTile = Stockpile.findFreeSpace();
            assignedUnit.go.GetComponent<UnitGO>().PathToTile(HaulToTile);
            if(HaulToTile == null) cancelled = true;
            while(assignedUnit.occupyingTile != HaulToTile){
                if(cancelled){
                    obj.associatedJob = null;
                    assignedUnit.currentJob = null;
                    assignedUnit.state = UnitState.IDLE;
                    assignedUnit.go.GetComponent<UnitGO>().cancelPathing = true;
                    if(assignedUnit.occupyingTile.occupyingObject != null){
                        Tile nearbyEmptyTile = World.findNearbyEmptyTile(assignedUnit.occupyingTile);
                        assignedUnit.go.GetComponent<UnitGO>().PathToTile(World.findNearbyEmptyTile(nearbyEmptyTile));
                        while(assignedUnit.occupyingTile != nearbyEmptyTile){
                            await Task.Yield();
                        }
                    }
                    obj.currentlyBeingHauled = false;
                    obj.markedForHauling = true;
                    HaulableObject.CreateNewObject(assignedUnit.haulingObject, assignedUnit.position, assignedUnit.occupyingTile);
                    assignedUnit.haulingObject = null;
                    cancelled = false;
                    return;
                } 
                if(Stockpile.freeSpaces() == 0) cancelled = true;
                await Task.Yield();
            }
            if(assignedUnit.occupyingTile.occupyingObject == null){
                standingOnEmptyStockpileTile = true;
            }
        }

        HaulableObject.CreateNewObject(assignedUnit.haulingObject, HaulToTile.position, assignedUnit.occupyingTile);
        assignedUnit.haulingObject = null;

        // Remove Job
        GameManager.Instance.jobManager.jobs.Remove(this);
        obj.associatedJob = null;
        assignedUnit.currentJob = null;
        assignedUnit.state = UnitState.IDLE;
        assignedUnit.go.GetComponent<UnitGO>().cancelPathing = true;
        assignedUnit.haulingObject = null;
        cancelled = false;
        obj.currentlyBeingHauled = false;
        return;
    }

    public override void RemoveJob()
    {
        // Remove Job
        GameManager.Instance.jobManager.jobs.Remove(this);
        cancelled = true;
        obj.currentlyBeingHauled = false;
    }
}

public enum JobType {
    MOVE,
    HARVESTOBJECT,
    HAUL,
}