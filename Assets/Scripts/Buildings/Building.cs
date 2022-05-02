using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : NetworkBehaviour
{
    [SerializeField] private GameObject buildingPreview = null;
    [SerializeField] private Sprite icon = null;
    [SerializeField] private int id = -1;
    [SerializeField] private int price = 100;

    public static event Action<Building> BuildingSpawned;
    public static event Action<Building> BuildingDespawned;

    public static event Action<Building> ClientBuildingSpawned;
    public static event Action<Building> ClientBuildingDespawned;
     
    public GameObject GetBuildingPreview()
    {
        return this.buildingPreview;
    }

    public Sprite GetIcon()
    {
        return this.icon;
    }

    public int GetId()
    {
        return this.id;
    }

    public int GetPrice()
    {
        return this.price;
    }

    #region Server

    public override void OnStartServer()
    {
        BuildingSpawned?.Invoke(this);
    }

    public override void OnStopServer()
    {
        BuildingDespawned?.Invoke(this);
    }

    #endregion

    #region Client

    public override void OnStartAuthority() // when a client(with authority) connects, alert that a building spawned
    {
        ClientBuildingSpawned?.Invoke(this);
    }

    public override void OnStopClient() // when a client(with authority) disconnects, alert that a building had despawned
    {
        if (!hasAuthority) { return; }

        ClientBuildingDespawned?.Invoke(this);
    }

    #endregion

}
