using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; set; }
    public World world;
    public UnitManager unitManager;
    public JobManager jobManager;
    public ResourceManager resourceManager;
    public GameState state;
    public static event Action<GameState> OnGameStateChanged;
    public Pathfinding pathfinder;

    private void Awake() 
    { 
        // ENSURES ONLY ONE INSTANCE OF GameManager EXISTS
        if (Instance != null && Instance != this) 
        { 
            Destroy(this); 
        } 
        else 
        { 
            Instance = this; 
        } 
        world = GetComponentInChildren<World>();
        unitManager = GetComponentInChildren<UnitManager>();
        jobManager = GetComponentInChildren<JobManager>();
        resourceManager = GetComponentInChildren<ResourceManager>();
        pathfinder = new Pathfinding();
    }

    void Start(){
        UpdateGameState(GameState.GenerateWorld);
    }

    public void UpdateGameState(GameState newState){
        state = newState;

        switch(newState) {
            case GameState.GenerateWorld:
                world.GenerateWorld();
                UpdateGameState(GameState.SpawnUnits);
                break;
            case GameState.SpawnUnits:
                unitManager.SpawnUnits();
                UpdateGameState(GameState.Freeplay);
                break;
        }

        OnGameStateChanged?.Invoke(newState);

    }

}

public enum GameState {
    GenerateWorld,
    SpawnUnits,
    Freeplay,
    MoveUnit,
}