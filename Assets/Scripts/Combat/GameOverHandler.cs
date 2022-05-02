using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverHandler : NetworkBehaviour
{
    private List<UnitBase> bases = new List<UnitBase>();

    public static event Action GameOver;
    public static event Action<string> ClientGameOver;

    #region Server

    public override void OnStartServer() // being called to the server when this script is loaded
    {
        UnitBase.BaseSpawned += ServerHandleBaseSpawned; // listen to an event telling if a base has been spawned
        UnitBase.BaseDespawned += ServerHandleBaseDespawned; // listen to an event telling if a base has been despawned
    }

    public override void OnStopServer()
    {
        UnitBase.BaseSpawned -= ServerHandleBaseSpawned; // stops listening to an event telling if a base has been spawned
        UnitBase.BaseDespawned -= ServerHandleBaseDespawned; // stops listening to an event telling if a base has been despawned
    }

    //[Server]
    private void ServerHandleBaseSpawned(UnitBase unitBase) // adding a base the base list
    {
        bases.Add(unitBase);
    }

    //[Server]
    private void ServerHandleBaseDespawned(UnitBase unitBase) // removing a base from the base list and checking if only 1 player left
    {
        bases.Remove(unitBase);

        if(bases.Count != 1) { return; }

        int winnerID = bases[0].connectionToClient.connectionId;

        RpcGameOver($"Player {winnerID}");

        GameOver?.Invoke();
    }

    #endregion

    #region Client

    [ClientRpc] // "ClientRpc" attribute is used to tell all clients to run the code in this method
    private void RpcGameOver(string winnerName)
    {
        ClientGameOver?.Invoke(winnerName); // notifing to whoever is listening that the game is over and telling the winner's name
    }

    #endregion
}
