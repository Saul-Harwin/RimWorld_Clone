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
                if(ho.markedForHauling && !ho.currentlyBeingHauled){
                    jobs.Add(new HaulObjectJob(ho));
                    ho.markedForHauling = false;
                    ho.currentlyBeingHauled = true;
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
                cancelled = false;
                return;
            } 
            await Task.Yield();
        }
        assignedUnit.haulingObject = ScriptableObject.Instantiate(obj);
        // Delete Object
        GameManager.Instance.world.objects.Remove(obj);
        GameObject.Destroy(obj.go);
        GameObject.Destroy(obj);
        Tile tempHaulTile = GameManager.Instance.world.tiles[50,50];
        assignedUnit.go.GetComponent<UnitGO>().PathToTile(tempHaulTile);
        while(assignedUnit.occupyingTile != tempHaulTile){
            if(cancelled){
                HaulableObject.CreateNewObject(assignedUnit.haulingObject, assignedUnit.position, assignedUnit.occupyingTile);
                assignedUnit.haulingObject = null;
                assignedUnit.currentJob = null;
                assignedUnit.state = UnitState.IDLE;
                return;
            }
           await Task.Yield(); 
        }
        HaulableObject.CreateNewObject(assignedUnit.haulingObject, assignedUnit.position, assignedUnit.occupyingTile);
        assignedUnit.haulingObject = null;

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
        obj.currentlyBeingHauled = false;
    }
}

public enum JobType {
    MOVE,
    HARVESTOBJECT,
    HAUL,
}