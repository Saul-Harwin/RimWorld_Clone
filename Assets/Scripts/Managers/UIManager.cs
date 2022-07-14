using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{

    public UIState uiState;
    public buttonSet[] buttonSets;

    void Start()
    {
        uiState = UIState.BASEBAR;
    }

    void Update()
    {
        RenderButtons();
    }

    void RenderButtons(){
        foreach (buttonSet b in buttonSets)
        {
            if(b.uiState == uiState) b.buttonSetGameObject.SetActive(true);
            else b.buttonSetGameObject.SetActive(false);
        }
    }

    public void ChangeToBaseBarState(){
        uiState = UIState.BASEBAR;
    }

    public void ChangeToOrdersBar(){
        uiState = UIState.ORDERSBAR;
    }

    public void ChangeToZonesBar(){
        uiState = UIState.ZONESBAR;
    }
}

public enum UIState {
    BASEBAR,
    ORDERSBAR,
    ZONESBAR,
}

[System.Serializable]
public class buttonSet {
    public UIState uiState;
    public GameObject buttonSetGameObject;
}