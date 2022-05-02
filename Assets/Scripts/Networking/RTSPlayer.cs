using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RTSPlayer : NetworkBehaviour
{
    [SerializeField] private LayerMask buildingBlocklayer = new LayerMask();
    [SerializeField] private Building[] buildings = new Building[0];
    [SerializeField] private float buildingRangeLimit = 5f;

    [SyncVar(hook = nameof(ClientHandleMoneyUpdated))]//This int is being synced to all clients. Whenever it changes
    private int money = 500;                          //it calls a method called "ClientHandleMoneyUpdated"

    public event Action<int> moneyUpdated;


    private Color teamColor = new Color();
    private List<Unit> myUnits = new List<Unit>();
    private List<Building> myBuildings = new List<Building>();


    public Color GetTeamColor()
    {
        return this.teamColor;
    }

    public List<Unit> GetMyUnits()
    {
        return this.myUnits;
    }

    public int GetMoney()
    {
        return this.money;
    }

    public List<Building> GetMyBuildings()
    {
        return this.myBuildings;
    }

    public bool CanPlaceBuilding(BoxCollider buildingCollider, Vector3 point)
    {
        if (Physics.CheckBox(point + buildingCollider.center, buildingCollider.size / 2, Quaternion.identity, buildingBlocklayer)) { return false; } // if building is overlaping dont place it

        foreach (Building building in myBuildings)
        {
            if ((point - building.transform.position).sqrMagnitude <= buildingRangeLimit * buildingRangeLimit)
            {
                return true;
            }
        }

        return false;
    }

    #region Sevrer

    public override void OnStartServer() // if a unit has been spawned/despawned, it runs a method for it
    {
        Unit.UnitSpawned += ServerHandlerUnitSpawned;
        Unit.UnitDespawned += ServerHandlerUnitDespawned;
        Building.BuildingSpawned += ServerHandleBuildingSpawned;
        Building.BuildingDespawned += ServerHandleBuildingDespawned;
    }

    public override void OnStopServer() // if a unit has been spawned/despawned, it runs a method for it
    {
        Unit.UnitSpawned -= ServerHandlerUnitSpawned;
        Unit.UnitDespawned -= ServerHandlerUnitDespawned;
        Building.BuildingSpawned -= ServerHandleBuildingSpawned;
        Building.BuildingDespawned -= ServerHandleBuildingDespawned;
    }

    public void SetMoney(int newMoney)
    {
        if (!isServer) { return; }
        this.money = newMoney;
    }

    public void SetTeamColor(Color color)
    {
        if (!isServer) { return; }
        this.teamColor = color;
    }

    [Command] // "command" attribute is used for a client to tell the server to do whatever is in the method
    public void CmdTryPlaceBuilding(int buildingId, Vector3 point)
    {
        Building buildingToPlace = null;

        foreach(Building building in buildings)
        {
            if(building.GetId() == buildingId)
            {
                buildingToPlace = building;
                break;
            }
        }
        
        if(buildingToPlace == null) { return; }

        if(money < buildingToPlace.GetPrice()) { return; }

        BoxCollider buildingCollider = buildingToPlace.GetComponent<BoxCollider>();

        if (!CanPlaceBuilding(buildingCollider, point)) { return; }

        GameObject buildingInstance = Instantiate(buildingToPlace.gameObject, point, buildingToPlace.transform.rotation);
        NetworkServer.Spawn(buildingInstance, connectionToClient);

        SetMoney(money - buildingToPlace.GetPrice());
    }

    private void ServerHandlerUnitSpawned(Unit unit) // when a unit spawns, it adds it to the list of spawned units
    {
        if(unit.connectionToClient.connectionId != connectionToClient.connectionId) { return; }

        myUnits.Add(unit);
    }

    private void ServerHandlerUnitDespawned(Unit unit) // when a unit despawns, it removes it from the list of spawned units
    {
        if (unit.connectionToClient.connectionId != connectionToClient.connectionId) { return; }

        myUnits.Remove(unit);
    }

    private void ServerHandleBuildingSpawned(Building building) // when a unit spawns, it adds it to the list of spawned units
    {
        if (building.connectionToClient.connectionId != connectionToClient.connectionId) { return; }

        myBuildings.Add(building);
    }

    private void ServerHandleBuildingDespawned(Building building) // when a unit despawns, it removes it from the list of spawned units
    {
        if (building.connectionToClient.connectionId != connectionToClient.connectionId) { return; }

        myBuildings.Remove(building);
    }

    #endregion

    #region Client

    public override void OnStartAuthority() // this method is being called when a client(with authority) connects
    {
        if (NetworkServer.active) { return; }

        Unit.ClientUnitSpawned += ClientHandlerUnitSpawned;       //listen to events so we 
        Unit.ClientUnitDespawned += ClientHandlerUnitDespawned;   //know if a unit is spawned/despawned

        Building.ClientBuildingSpawned += ClientHandleBuildingSpawned;       //listen to events so we 
        Building.ClientBuildingDespawned += ClientHandleBuildingDespawned;   //know if a building is spawned/despawned
    }

    public override void OnStopClient() // this method is being called when a client(with authority) no longer connected
    {
        if (!isClientOnly || !hasAuthority) { return; }

        Unit.ClientUnitSpawned -= ClientHandlerUnitSpawned;     //stop listening to events that tell us 
        Unit.ClientUnitDespawned -= ClientHandlerUnitDespawned; //if a unit is spawned/despawned

        Building.ClientBuildingSpawned -= ClientHandleBuildingSpawned;       //stops listening to events that tell us 
        Building.ClientBuildingDespawned -= ClientHandleBuildingDespawned;   //if a building is spawned/despawned
    }

    private void ClientHandleMoneyUpdated(int oldMoney, int newMoney) // being called whenever the health is updated
    {
        moneyUpdated?.Invoke(newMoney);
    }


    private void ClientHandlerUnitSpawned(Unit unit) // when a unit spawns, it adds it to the list of spawned units
    {
        myUnits.Add(unit);
    }

    private void ClientHandlerUnitDespawned(Unit unit) // when a unit despawns, it removes it from the list of spawned units
    {
        myUnits.Remove(unit);
    }

    private void ClientHandleBuildingSpawned(Building building) // when a building spawns, it adds it to the list of spawned buildings
    {
        myBuildings.Add(building);
    }

    private void ClientHandleBuildingDespawned(Building building) // when a building despawns, it removes it from the list of spawned buildings
    {
        myBuildings.Remove(building);
    }

    #endregion
}
