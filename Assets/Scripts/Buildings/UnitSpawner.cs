using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UnitSpawner : NetworkBehaviour, IPointerClickHandler // "IPointerClickHandler" helpsus to know if someone clicked the object this script is attached to
{
    [SerializeField] private Health health = null;
    [SerializeField] private Unit unitPrefab = null;
    [SerializeField] private Transform unitSpawnPoint = null;
    [SerializeField] private TMP_Text remainingUnitsText = null;
    [SerializeField] private Image UnitProgressImage = null;
    [SerializeField] private int maxQueue = 5;
    [SerializeField] private float spawnMoveRange = 7f;
    [SerializeField] private float unitSpawnDuration = 5f;

    private float progressImageVelocity;

    [SyncVar(hook = nameof(ClientHandleQueuedUnitsUpdated))]
    private int queuedUnits;
    [SyncVar]
    private float UnitTimer;

    private void Update()
    {
        if (isServer)
        {
            ProduceUnits();
        }

        if (isClient)
        {
            UpdateTimerDisplay();
        }
    }

    #region Server

    public override void OnStartServer() // this method is called when this scripts is loaded
    {
        health.SomehtingDied += HandleSomethingDied; // listening to an event telling if the unit spawner has died
    }

    public override void OnStopServer() // this method is called when this scripts stops being used
    {
        health.SomehtingDied -= HandleSomethingDied; // stops listening to an event telling if the unit spawner has died
    }


    //[Server]
    private void ProduceUnits()
    {
        if(queuedUnits == 0) { return; }

        UnitTimer += Time.deltaTime;

        if(UnitTimer < unitSpawnDuration) { return; }

        GameObject unitInstance = Instantiate(unitPrefab.gameObject,
            unitSpawnPoint.position,
            unitSpawnPoint.rotation);

        NetworkServer.Spawn(unitInstance, connectionToClient);

        Vector3 spawnOffset = Random.insideUnitSphere * spawnMoveRange;
        spawnOffset.y = unitSpawnPoint.position.y;

        UnitMovement unitMovement = unitInstance.GetComponent<UnitMovement>();
        unitMovement.ServerMove(unitSpawnPoint.position + spawnOffset);

        queuedUnits--;
        UnitTimer = 0;
        
    }
    //[Server]
    private void HandleSomethingDied() // if player died destroy this object
    {
        NetworkServer.Destroy(gameObject);
    }
    
    [Command] // "command" attribute is used for a client to tell the server to do whatever is in the method
    private void CmdSpawnUnit() // method used to spawn units
    {
        if(queuedUnits == maxQueue) { return; }

        RTSPlayer player = connectionToClient.identity.GetComponent<RTSPlayer>();

        if (player.GetMoney() < unitPrefab.GetUnitCost()){ return; }

        queuedUnits++;

        player.SetMoney(player.GetMoney() - unitPrefab.GetUnitCost());
    }

    #endregion

    #region Client

    private void UpdateTimerDisplay()
    {
        float newProgress = UnitTimer / unitSpawnDuration;

        if(newProgress < UnitProgressImage.fillAmount)
        {
            UnitProgressImage.fillAmount = newProgress;
        }
        else
        {
            UnitProgressImage.fillAmount = Mathf.SmoothDamp(UnitProgressImage.fillAmount, newProgress, ref progressImageVelocity, 0.1f);
        }
    }

    private void ClientHandleQueuedUnitsUpdated(int oldUnits, int newUnits)
    {
        remainingUnitsText.text = queuedUnits.ToString();
    }

    public void OnPointerClick(PointerEventData eventData) // method is checking if whoever clicked has authority (checking you didnt click other client's spawner)
    {
        if (eventData.button == PointerEventData.InputButton.Left && hasAuthority) 
        {
            CmdSpawnUnit();
        }

    }

    #endregion
}
