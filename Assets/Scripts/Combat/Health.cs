using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : NetworkBehaviour
{
    [SerializeField] private int maxHealth = 100;

    [SyncVar(hook = nameof(HandleHealthUpdated))] //This int is being synced to all clients. Whenever it changes
    private int currentHealth;                    //it calls a method called "HandleHealthUpdated"

    public event Action SomehtingDied;
    public event Action<int, int> HealthUpdated;

    #region Server

    public override void OnStartServer()
    {
        currentHealth = maxHealth;

        UnitBase.PlayerDied += HandlePlayerDied;
    }

    public override void OnStopServer()
    {
        UnitBase.PlayerDied += HandlePlayerDied;
    }

    //[Server]
    public void DealDamage(int damageAmount) // dealing damage
    {
        if(currentHealth == 0) { return; }

        currentHealth = Mathf.Max(currentHealth - damageAmount, 0);

        if(currentHealth != 0) { return; }

        SomehtingDied?.Invoke(); // raising an event for whoever needs to know that somthing has died
    }

    //[Server]
    private void HandlePlayerDied(int playerID)
    {
        if(connectionToClient.connectionId != playerID) { return; }

        DealDamage(currentHealth);
    }
    #endregion

    #region Client

    private void HandleHealthUpdated(int oldHealth, int newHealth) // being called whenever the health is updated
    {
        HealthUpdated?.Invoke(newHealth, maxHealth); // raising an event saying the health changed for whoever was listening
    }

    #endregion
}
