using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : NetworkBehaviour
{
    [SerializeField] private Transform playerCameraTransform = null;
    [SerializeField] private float speed = 20f;
    [SerializeField] private float screenBorderThickness = 10f;
    [SerializeField] private Vector2 screenXLimit = Vector2.zero;
    [SerializeField] private Vector2 screenZLimit = Vector2.zero;

    private Vector2 previousInput;

    private Controls controls;

    public override void OnStartAuthority() // called when a client with authority starts to use this script
    {
        playerCameraTransform.gameObject.SetActive(true);

        controls = new Controls();

        controls.Player.MoveCamera.performed += SetPreviousInput; // when w/a/s/d is pressed/let go
        controls.Player.MoveCamera.canceled += SetPreviousInput;  // it calls "SetPreviousInput" method

        controls.Enable();
    }

    private void Update() // checks every frame if client has authority & is focused - if he is it updates the camera position
    {
        if (!hasAuthority || !Application.isFocused) { return; }

        UpdateCameraPosition();
    }

    private void UpdateCameraPosition() // moves the camera if needed
    {
        Vector3 pos = playerCameraTransform.position;

        if(previousInput == Vector2.zero) // if there was no keyboard input check if mouse is in a corner
        {
            Vector3 cursorMovement = Vector3.zero;

            Vector2 cursorPosition = Mouse.current.position.ReadValue();

            if(cursorPosition.y >= Screen.height - screenBorderThickness)
            {
                cursorMovement.z += 1;
            }
            else if(cursorPosition.y <= screenBorderThickness)
            {
                cursorMovement.z -= 1;
            }

            if (cursorPosition.x >= Screen.width - screenBorderThickness)
            {
                cursorMovement.x += 1;
            }
            else if (cursorPosition.x <= screenBorderThickness)
            {
                cursorMovement.x -= 1;
            }

            pos += cursorMovement.normalized * speed * Time.deltaTime;
        }
        else
        {
            pos += new Vector3(previousInput.x, 0f, previousInput.y) * speed * Time.deltaTime;
        }

        pos.x = Mathf.Clamp(pos.x, screenXLimit.x, screenXLimit.y);
        pos.z = Mathf.Clamp(pos.z, screenZLimit.x, screenZLimit.y);

        playerCameraTransform.position = pos;
    }

    private void SetPreviousInput(InputAction.CallbackContext ctx)
    {
        previousInput = ctx.ReadValue<Vector2>();
    }
}
