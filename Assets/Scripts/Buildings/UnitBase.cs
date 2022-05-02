using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitBase : NetworkBehaviour
{
    [SerializeField] private Health health = null;

    public static event Action<int> PlayerDied;
    public static event Action<UnitBase> BaseSpawned;
    public static event Action<UnitBase> BaseDespawned;

    #region Server

    public override void OnStartServer() // being called to the server when this script is loaded
    {
        health.SomehtingDied += HandleBaseDestroyed; // listen to an event telling if the unit has died

        BaseSpawned?.Invoke(this); // alert that a base has spawned
    }

    public override void OnStopServer() // being called to the server when this script is no longer in use
    {
        BaseDespawned?.Invoke(this); // alert that a base has despawned

        health.SomehtingDied -= HandleBaseDestroyed; // stops listening to an event telling if the unit has died
    }

    //[Server]
    private void HandleBaseDestroyed() // destroying the base
    {
        PlayerDied?.Invoke(connectionToClient.connectionId);

        NetworkServer.Destroy(gameObject);
    }

    #endregion

    #region Client

    #endregion
}
