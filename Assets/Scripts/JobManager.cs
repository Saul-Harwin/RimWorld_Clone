using System.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
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
            foreach(Object o in GameManager.Instance.world.objects){
                if(o.markedForHarvest && !o.currentlyBeingHarvested){
                    jobs.Add(new HarvestObjectJob(o));
                    o.markedForHarvest = false;
                    o.currentlyBeingHarvested = true;
                }
            }
        }
    }

    void AssignJobs(){
        foreach (Unit u in GameManager.Instance.unitManager.units){
            if(u.state == UnitState.IDLE){
                foreach (Job j in jobs){
                    if(j.assignedUnit == null && u.currentJob == null){
                        u.currentJob = j;
                        u.state = UnitState.HARVESTING;
                        j.assignedUnit = u;
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
    public Object obj;
    public HarvestObjectJob(Object objectToHarvest){
        type = JobType.HARVESTOBJECT;
        obj = objectToHarvest;
        obj.associatedJob = this;
    }

    public async override void Execute(){
        if(GameManager.Instance.pathfinder.FindPath(assignedUnit.occupypingTile, obj.occupyingTile) == null){
            Debug.Log("No Path!");
            GameManager.Instance.jobManager.jobs.Remove(this);
            obj.currentlyBeingHarvested = false;
            assignedUnit.FreeUnitFromJob();
        }
        beingExecuted = true;
        obj.associatedJob = this;
        assignedUnit.go.GetComponent<UnitGO>().PathToTile(obj.occupyingTile);
        while(assignedUnit.occupypingTile != obj.occupyingTile){
            if(cancelled){
                assignedUnit.go.GetComponent<UnitGO>().cancelPathing = true;
                obj.currentlyBeingHarvested = false;
                obj.associatedJob = null;
                assignedUnit.currentJob = null;
                assignedUnit.state = UnitState.IDLE;
                assignedUnit.go.GetComponent<UnitGO>().cancelPathing = true;
                obj.currentlyBeingHarvested = false;
                return;
            } 
            await Task.Yield();
        }
        // Delete Object
        GameManager.Instance.world.objects.Remove(obj);
        GameObject.Destroy(obj.go);
        GameObject.Destroy(obj);

        // Remove Job
        GameManager.Instance.jobManager.jobs.Remove(this);

        // Free Unit From Job
        assignedUnit.currentJob = null;
        assignedUnit.state = UnitState.IDLE;
        if(obj.objectType == 0) { GameManager.Instance.resourceManager.woodAmount += 10; }
        else if(obj.objectType == 1) { GameManager.Instance.resourceManager.stoneAmount += 10; }
    }

    public override void RemoveJob()
    {
        // Remove Job
        GameManager.Instance.jobManager.jobs.Remove(this);
        cancelled = true;
        obj.currentlyBeingHarvested = false;
    }
}

public enum JobType {
    MOVE,
    HARVESTOBJECT,
}