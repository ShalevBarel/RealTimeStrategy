using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UnitCommandGiver : MonoBehaviour
{
    [SerializeField] private UnitSelectionHandler unitSelectionHandler = null;

    [SerializeField] private LayerMask layerMask = new LayerMask();

    private Camera mainCamera;

    private void Start() // being called when this script is first called
    {
        mainCamera = Camera.main;

        GameOverHandler.ClientGameOver += ClientHandleGameOver;
    }

    private void OnDestroy() // being called when script is no longer in use
    {
        GameOverHandler.ClientGameOver -= ClientHandleGameOver;
    }

    private void Update() // "update" method is being called every frame. this is where we do our constant checking if the player tries to target enemy object or move
    {
        if(!Mouse.current.rightButton.wasPressedThisFrame) { return; }

        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if(!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask)) { return; }

        if(hit.collider.TryGetComponent<Targetable>(out Targetable target))
        {
            if(target.hasAuthority)
            {
                TryMove(hit.point);
                return;
            }
            TryTarget(target);
            return;
        }

        TryMove(hit.point);
    }
    private void TryMove(Vector3 point) // sends the command to move to every selected unit
    {
        foreach(Unit unit in unitSelectionHandler.selectedUnits)
        {
            unit.GetUnitMovement().CmdMove(point);
        }
    }
    private void TryTarget(Targetable target) // sends the command to try and target a targetable
    {
        foreach (Unit unit in unitSelectionHandler.selectedUnits)
        {
            unit.GetTargeter().CmdSetTarget(target.gameObject);
        }
    }

    private void ClientHandleGameOver(string winnerName) // called when game over
    {
        enabled = false; // disabling the script
    }
}
