using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public World world;
    public UnitManager unitManager;

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
}
