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
        foreach(Object o in GameManager.Instance.world.objects){
            if(o.markedForHarvest){
                jobs.Add(new HarvestObjectJob(o));
                o.markedForHarvest = false;
                o.currentlyBeingHarvested = true;
            }
        }
    }

    void AssignJobs(){
        foreach (Unit u in GameManager.Instance.unitManager.units){
            if(u.state == UnitState.IDLE){
                foreach (Job j in jobs){
                    if(j.assignedUnit == null){
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
        foreach(Job j in jobs){
            if(!j.beingExecuted && j.assignedUnit != null){
                j.Execute();
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
}

[System.Serializable]
public class HarvestObjectJob : Job{
    public Object obj;
    public HarvestObjectJob(Object objectToHarvest){
        type = JobType.HARVESTOBJECT;
        obj = objectToHarvest;
    }

    public async override void Execute(){
        beingExecuted = true;
        assignedUnit.go.GetComponent<UnitGO>().PathToTile(obj.occupyingTile);
        while(assignedUnit.occupypingTile != obj.occupyingTile){
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
}

public enum JobType {
    MOVE,
    HARVESTOBJECT,
}