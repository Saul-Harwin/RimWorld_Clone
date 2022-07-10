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
        if(GameManager.Instance.resourceManager.woodAmount < 30){
            if(jobs.Find(j => j.type == JobType.CHOP_TREE) == null){
                jobs.Add(new Job());
                Debug.Log("Job Added");
            }
        }
    }
}

public class Job {
    public JobType type;
    public Job(){
        type = JobType.CHOP_TREE;
    }
}

public class ChopTreeJob : Job {
}

public enum JobType {
    MOVE,
    CHOP_TREE,
    MINE_ROCK,
}