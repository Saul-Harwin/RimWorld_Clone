using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public World world;
    public UnitManager unitManager;
    public GameState state;
    public static event Action<GameState> OnGameStateChanged;

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
    }

    void Start(){
        UpdateGameState(GameState.Freeplay);
    }

    public void UpdateGameState(GameState newState){
        state = newState;

        OnGameStateChanged?.Invoke(newState);

    }

}

public enum GameState {
    Freeplay,
    MoveUnit,
}