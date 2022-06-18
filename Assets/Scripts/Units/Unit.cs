using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Unit : NetworkBehaviour
{
    [SerializeField] private int unitCost = 10;
    [SerializeField] private UnitMovement unitMovement = null;
    [SerializeField] private UnityEvent onSelected = null;
    [SerializeField] private UnityEvent onDeselected = null;
    [SerializeField] private Targeter targeter = null;
    [SerializeField] private Health health = null;

    public static event Action<Unit> UnitSpawned;
    public static event Action<Unit> UnitDespawned;

    public static event Action<Unit> ClientUnitSpawned;
    public static event Action<Unit> ClientUnitDespawned;

    public int GetUnitCost()
    {
        return this.unitCost;
    }

    public Targeter GetTargeter()
    {
        return this.targeter;
    }

    public UnitMovement GetUnitMovement()
    {
        return this.unitMovement;
    }

    #region Server

    public override void OnStartServer()
    {
        UnitSpawned?.Invoke(this); // when the server starts, alert that a unit spawned

        health.SomehtingDied += HandleSomethingDied; // listen to an event telling if the unit has died
    }

    public override void OnStopServer()
    {
        UnitDespawned?.Invoke(this); // when the server closes, alert that a unit had despawned

        health.SomehtingDied -= HandleSomethingDied; // listen to an event telling if the unit has died
    }

    private void HandleSomethingDied()
    {
        NetworkServer.Destroy(gameObject);
    }
    #endregion

    #region Client

    public override void OnStartAuthority() // when a client(with authority) connects, alert that a unit spawned
    {
        ClientUnitSpawned?.Invoke(this);
    }

    public override void OnStopClient() // when a client(with authority) disconnects, alert that a unit had despawned
    {
        if (!hasAuthority) { return; }

        ClientUnitDespawned?.Invoke(this);
    }

    public void Select() // alert that a unit has been selected
    {
        if (!hasAuthority) { return; }

        onSelected?.Invoke();
    }

    public void Deselect() // alert that a unit has been deselected
    {
        if (!hasAuthority) { return; }

        onDeselected?.Invoke();
    }

    #endregion
}
