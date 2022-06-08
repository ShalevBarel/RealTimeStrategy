using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UnitSelectionHandler : MonoBehaviour
{
    [SerializeField] private RectTransform unitSelectionArea = null;

    [SerializeField] private LayerMask layerMask = new LayerMask();

    private Vector2 startPosition;

    private RTSPlayer player;
    private Camera mainCamera;

    public List<Unit> selectedUnits { get; } = new List<Unit>();

    private void Start() // being called when this script is first callled
    {
        mainCamera = Camera.main;

        player = NetworkClient.connection.identity.GetComponent<RTSPlayer>();

        Unit.ClientUnitDespawned += HandleUnitDespawned; // listening to see if a unit has despawned
        GameOverHandler.ClientGameOver += ClientHandleGameOver; // listening to see if game is over
    }

    private void OnDestroy() // being called when script is no longer in use
    {
        Unit.ClientUnitDespawned -= HandleUnitDespawned;       //stops listening
        GameOverHandler.ClientGameOver -= ClientHandleGameOver;//
    }

    private void Update() // "update" method is being called every frame. this is where we do our constant checking if the mouse is pressed
    {
        if(Mouse.current.leftButton.wasPressedThisFrame)
        {
            ClearSelectedUnits();
        }
        else if(Mouse.current.leftButton.wasReleasedThisFrame)
        {
            ClearSelectionArea();
        }
        else if(Mouse.current.leftButton.isPressed)
        {
            UpdateSelectionArea();
        }
    }

    private void UpdateSelectionArea() // make the drag selection box
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();

        float areaWidth = mousePosition.x - startPosition.x;
        float areaHeight = mousePosition.y - startPosition.y;

        unitSelectionArea.sizeDelta = new Vector2(Mathf.Abs(areaWidth), Mathf.Abs(areaHeight));
        unitSelectionArea.anchoredPosition = startPosition + new Vector2(areaWidth / 2, areaHeight / 2);

    }

    private void ClearSelectedUnits() //clear the already selected units, and prepare to select others
    {
        if (!Keyboard.current.shiftKey.isPressed)
        {
            foreach (Unit selectedUnit in selectedUnits)
            {
                selectedUnit.Deselect();
            }

            selectedUnits.Clear();
        }

        unitSelectionArea.gameObject.SetActive(true);

        startPosition = Mouse.current.position.ReadValue();

        UpdateSelectionArea();
    }

    private void ClearSelectionArea() // remove the drag selection box, and select everything that was in it & is yours
    {
        unitSelectionArea.gameObject.SetActive(false);

        if (unitSelectionArea.sizeDelta.magnitude == 0)
        {
            OneUnitSelection();
        }

        Vector2 min = unitSelectionArea.anchoredPosition - (unitSelectionArea.sizeDelta / 2);
        Vector2 max = unitSelectionArea.anchoredPosition + (unitSelectionArea.sizeDelta / 2);

        foreach(Unit unit in player.GetMyUnits())
        {
            if (selectedUnits.Contains(unit)) { continue; } // if unit is already selected, move to the next one

            Vector3 screenPosition = mainCamera.WorldToScreenPoint(unit.transform.position);

            if(screenPosition.x < max.x &&
                screenPosition.x > min.x &&
                screenPosition.y < max.y &&
                screenPosition.y > min.y)
            {
                selectedUnits.Add(unit);

                unit.Select();
            }
        }
    }

    private void OneUnitSelection() // is used to select a single unit (no drag box)
    {
        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask)) { return; }

        if (!hit.collider.TryGetComponent<Unit>(out Unit unit)) { return; }

        if (!unit.hasAuthority) { return; }

        selectedUnits.Add(unit);

        foreach (Unit selectedUnit in selectedUnits)
        {
            selectedUnit.Select();
        }

        return;
    }

    private void HandleUnitDespawned(Unit unit)
    {
        selectedUnits.Remove(unit);
    }

    private void ClientHandleGameOver(string winnerName) // called when game over
    {
        enabled = false; // disabling the script
    }
}
